using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests;

public class BlockProcessorTests
{

    Mock<IBlockRepository> _blockRepoMock;
    Mock<IOrderLocationRepository> _orderLocationRepoMock;
    Mock<IGreenHouseRepository> _greenHouseRepoMock;
    Mock<ILog> _logMock;
    BlockProcessor _processor;
    Block _newBlock;
    OrderLocation _newOrderLocation;
    Order _order;

    public BlockProcessorTests()
    {
        _blockRepoMock = new Mock<IBlockRepository>();

        _blockRepoMock.Setup(x => x.Insert(It.IsAny<Block>()))
            .Callback<Block>(d => _newBlock = d);
        _blockRepoMock.Setup(x => x.Remove(It.IsAny<int>()));
        _blockRepoMock.Setup(x => x.Update(It.IsAny<Block>()));

        _orderLocationRepoMock = new Mock<IOrderLocationRepository>();

        _orderLocationRepoMock.Setup(x => x.Insert(It.IsAny<OrderLocation>()))
            .Callback<OrderLocation>(ol => _newOrderLocation = ol);
        _orderLocationRepoMock.Setup(x => x.Remove(It.IsAny<int>()));
        _orderLocationRepoMock.Setup(x => x.Update(It.IsAny<OrderLocation>()));

        _greenHouseRepoMock = new Mock<IGreenHouseRepository>();

        _greenHouseRepoMock.Setup(x => x.GetAll())
            .Returns(
                new List<GreenHouse>() {
                    new GreenHouse() { Id = 3 }
                }
            );

        _logMock = new Mock<ILog>();

        _logMock.Setup(x => x.Info(It.IsAny<string>()));

        _processor = new BlockProcessor(_logMock.Object
            , _blockRepoMock.Object
            , _orderLocationRepoMock.Object
            , _greenHouseRepoMock.Object);

        _order = GetOrderRecord();
    }

    private Order GetOrderRecord()
    {
        Order order = new Order()
        {
            Id = 3,
            AmountOfWishedSeedlings = 40500,
            AmountOfAlgorithmSeedlings = 48600,
            RealSowDate = new DateOnly(2023, 6, 25),
            Sown = true
        };

        OrderLocation orderLocation1 = new OrderLocation()
        {
            Id = 5,
            Order = order,
            SeedTrayAmount = 100,
            SeedlingAmount = 26000,
            GreenHouseId = 2,
            SeedTrayId = 1,
            RealSowDate = new DateOnly(2023, 6, 25)
        };

        Block block1 = new Block()
        {
            Id = 15,
            OrderLocationId = 5,
            BlockNumber = 4,
            SeedTrayAmount = 100,
            OrderLocation = orderLocation1
        };

        orderLocation1.Blocks.Add(block1);

        OrderLocation orderLocation2 = new OrderLocation()
        {
            Id = 6,
            Order = order,
            SeedTrayAmount = 50,
            SeedlingAmount = 13000,
            GreenHouseId = 4,
            SeedTrayId = 1,
            RealSowDate = new DateOnly(2023, 6, 25)
        };

        Block block2 = new Block()
        {
            Id = 16,
            OrderLocationId = 6,
            BlockNumber = 3,
            SeedTrayAmount = 50,
            OrderLocation = orderLocation2
        };

        orderLocation2.Blocks.Add(block2);

        OrderLocation orderLocation3 = new OrderLocation()
        {
            Id = 7,
            Order = order,
            SeedTrayAmount = 60,
            SeedlingAmount = 9600,
            GreenHouseId = 3,
            SeedTrayId = 2,
            RealSowDate = new DateOnly(2023, 6, 27)
        };

        Block block3 = new Block()
        {
            Id = 17,
            OrderLocationId = 7,
            BlockNumber = 2,
            SeedTrayAmount = 60,
            OrderLocation = orderLocation3
        };

        orderLocation3.Blocks.Add(block3);

        order.OrderLocations.Add(orderLocation1);
        order.OrderLocations.Add(orderLocation2);
        order.OrderLocations.Add(orderLocation3);

        return order;
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateACompleteBlock()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = 100;
        byte newGreenHouse = 2;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(blockToRelocate.OrderLocation.Id);
        _newBlock.OrderLocation.Should().Be(orderLocation);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        orderLocation.Blocks.Should().HaveCount(1);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateAPartialBlock()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        Block blockToRelocate = orderLocation.Blocks.First();
        short originalSeedTrays = blockToRelocate.SeedTrayAmount;
        short relocatedSeedTrays = 70;
        byte newGreenHouse = 2;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(blockToRelocate.OrderLocation.Id);
        _newBlock.OrderLocation.Should().Be(orderLocation);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        blockToRelocate.SeedTrayAmount.Should().Be((short)(originalSeedTrays - relocatedSeedTrays));

        orderLocation.Blocks.Should().HaveCount(2);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Once);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateACompleteBlockOutAHouseWithBrother()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        OrderLocation orderLocationBrother = _order.OrderLocations.First(x => x.Id == 6);
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = 100;
        byte newGreenHouse = 4;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(orderLocationBrother.Id);
        _newBlock.OrderLocation.Should().Be(orderLocationBrother);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        blockToRelocate.SeedTrayAmount.Should().Be(0);

        orderLocation.Blocks.Should().HaveCount(0);
        orderLocation.SeedTrayAmount.Should().Be(0);
        orderLocation.SeedlingAmount.Should().Be(0);
        orderLocationBrother.Blocks.Should().HaveCount(2);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateAPartialBlockOutAHouseWithBrother()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        OrderLocation orderLocationBrother = _order.OrderLocations.First(x => x.Id == 6);
        short originalSeedTraysOfTheBrother = orderLocationBrother.SeedTrayAmount;
        int originalSeedlingsOfTheBrother = orderLocationBrother.SeedlingAmount;
        Block blockToRelocate = orderLocation.Blocks.First();
        int alveolus = orderLocation.SeedlingAmount / orderLocation.SeedTrayAmount;
        short originalSeedTraysOfTheBlock = blockToRelocate.SeedTrayAmount;
        short relocatedSeedTrays = 30;
        byte newGreenHouse = 4;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(orderLocationBrother.Id);
        _newBlock.OrderLocation.Should().Be(orderLocationBrother);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        blockToRelocate.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheBlock - relocatedSeedTrays));

        orderLocation.Blocks.Should().HaveCount(1);
        orderLocation.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheBlock - relocatedSeedTrays));
        orderLocation.SeedlingAmount.Should().Be((originalSeedTraysOfTheBlock - relocatedSeedTrays) * alveolus);

        orderLocationBrother.Blocks.Should().HaveCount(2);
        orderLocationBrother.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheBrother + relocatedSeedTrays));
        orderLocationBrother.SeedlingAmount.Should().Be(originalSeedlingsOfTheBrother + (relocatedSeedTrays * alveolus));

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Once);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Exactly(2));

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateACompleteBlockOutAHouseWithOutBrother()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = 100;
        byte newGreenHouse = 3;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(_newOrderLocation.Id);
        _newBlock.OrderLocation.Should().Be(_newOrderLocation);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        blockToRelocate.SeedTrayAmount.Should().Be(0);

        orderLocation.Blocks.Should().HaveCount(0);
        orderLocation.SeedTrayAmount.Should().Be(0);
        orderLocation.SeedlingAmount.Should().Be(0);

        _newOrderLocation.Blocks.Should().HaveCount(1);
        _newOrderLocation.SeedTrayAmount.Should().Be(relocatedSeedTrays);
        _newOrderLocation.Order.Should().Be(_order);
        _newOrderLocation.GreenHouseId.Should().Be(newGreenHouse);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Once);
        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);

        _greenHouseRepoMock.Verify(x => x.GetAll(), Times.Once);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateAPartialBlockOutAHouseWithOutBrother()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        short originalSeedTraysOfTheOrderLocation = orderLocation.SeedTrayAmount;
        int originalSeedlingOfTheOrderLocation = orderLocation.SeedlingAmount;
        int alveolus = originalSeedlingOfTheOrderLocation / originalSeedTraysOfTheOrderLocation;
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = 70;
        byte newGreenHouse = 3;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(_newOrderLocation.Id);
        _newBlock.OrderLocation.Should().Be(_newOrderLocation);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        blockToRelocate.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheOrderLocation - relocatedSeedTrays));

        orderLocation.Blocks.Should().HaveCount(1);
        orderLocation.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheOrderLocation - relocatedSeedTrays));
        orderLocation.SeedlingAmount.Should().Be((originalSeedTraysOfTheOrderLocation - relocatedSeedTrays) * alveolus);

        _newOrderLocation.Blocks.Should().HaveCount(1);
        _newOrderLocation.SeedTrayAmount.Should().Be(relocatedSeedTrays);
        _newOrderLocation.Order.Should().Be(_order);
        _newOrderLocation.GreenHouseId.Should().Be(newGreenHouse);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Once);

        _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Once);
        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Exactly(2));

        _greenHouseRepoMock.Verify(x => x.GetAll(), Times.Once);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldThrowAnArgumentExceptionOnrelocatedSeedTraysLessThanZero()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = -20;
        byte newGreenHouse = 2;
        byte newBlockNumber = 4;

        Action action = () => _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        action.Should().Throw<ArgumentException>()
            .WithParameterName("relocatedSeedTrays")
            .WithMessage("La cantidad de bandejas reubicadas debe estar entre 0 y la cantidad de bandejas sin " +
                "entregar de la locación (Parameter 'relocatedSeedTrays')");

        _newBlock.Should().BeNull();

        orderLocation.Blocks.Should().HaveCount(1);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Never);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldThrowAnArgumentExceptionOnrelocatedSeedTraysGreaterThanTheActualAmount()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = 120;
        byte newGreenHouse = 2;
        byte newBlockNumber = 4;

        Action action = () => _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        action.Should().Throw<ArgumentException>()
            .WithParameterName("relocatedSeedTrays")
            .WithMessage("La cantidad de bandejas reubicadas debe estar entre 0 y la cantidad de bandejas sin " +
                "entregar de la locación (Parameter 'relocatedSeedTrays')");

        _newBlock.Should().BeNull();

        orderLocation.Blocks.Should().HaveCount(1);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Never);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);

        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
    }
}
