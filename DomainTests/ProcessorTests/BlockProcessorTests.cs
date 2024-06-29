using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests;

public class BlockProcessorTests
{
    #region coments
    //LATER - Make all these tests. and add a little more to test the exceptions
    // agregar un boton para reducir un bloque y otro para ampliarlo

    //these 4 are OK
    //in house | complete | with delivery OK

    //in house | complete | without delivery OK

    //in house | partial | with delivery OK

    //in house | partial | without delivery OK

    //si un block tiene delivery lo que quiere decir es que la transferencia va a ser parcial.


    //out house no brother | block  complete | OL with 1 block | with delivery OK

    //out house no brother | block  complete | OL with 1 block | without delivery OK

    //out house no brother | block  partial | OL with 1 or more blocks | with delivery OK

    //out house no brother | block  partial | OL with 1 or more blocks | without delivery OK



    //out house with brother | block  complete | OL with 1 block 

    //out house with brother | block  partial | OL with 1 block OK

    //out house with brother | block  complete | OL with 1 or more blocks

    //out house with brother | block  partial | OL with 1 or more blocks 

    #endregion

    Mock<IBlockRepository> _blockRepoMock;
    Mock<IOrderLocationRepository> _orderLocationRepoMock;
    Mock<ILog> _logMock;
    BlockProcessor _processor;
    Block _newBlock;
    Order _order;

    public BlockProcessorTests()
    {
        _blockRepoMock = new Mock<IBlockRepository>();

        _blockRepoMock.Setup(x => x.Insert(It.IsAny<Block>()))
            .Callback<Block>(d => _newBlock = d);
        _blockRepoMock.Setup(x => x.Remove(It.IsAny<int>()));
        _blockRepoMock.Setup(x => x.Update(It.IsAny<Block>()));

        _orderLocationRepoMock = new Mock<IOrderLocationRepository>();

        _orderLocationRepoMock.Setup(x => x.Remove(It.IsAny<int>()));
        _orderLocationRepoMock.Setup(x => x.Update(It.IsAny<OrderLocation>()));

        _logMock = new Mock<ILog>();

        _logMock.Setup(x => x.Info(It.IsAny<string>()));

        _processor = new BlockProcessor(_logMock.Object, _blockRepoMock.Object, _orderLocationRepoMock.Object);

        _order = GenerateOrderRecord();
    }

    private Order GenerateOrderRecord()
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
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        orderLocation.Blocks.Should().HaveCount(1);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
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
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        blockToRelocate.SeedTrayAmount.Should().Be((short)(originalSeedTrays - relocatedSeedTrays));

        orderLocation.Blocks.Should().HaveCount(2);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Once);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateABlockOutAHouseWithBrother()
    {
        OrderLocation orderLocation = _order.OrderLocations.First();
        Block blockToRelocate = orderLocation.Blocks.First();
        short relocatedSeedTrays = 100;
        byte newGreenHouse = 2;
        byte newBlockNumber = 4;

        _processor.SaveRelocateBlockChange(blockToRelocate, newGreenHouse, newBlockNumber, relocatedSeedTrays);

        _newBlock.OrderLocationId.Should().Be(blockToRelocate.OrderLocation.Id);
        _newBlock.BlockNumber.Should().Be(newBlockNumber);
        _newBlock.SeedTrayAmount.Should().Be(relocatedSeedTrays);

        orderLocation.Blocks.Should().HaveCount(1);

        _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);
        _blockRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);
        _blockRepoMock.Verify(x => x.Update(It.IsAny<Block>()), Times.Never);

        _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);
        _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
    }

    [Fact]
    public void SaveRelocateBlockChange_ShouldRelocateABlockOutAHouseWithOutBrother()
    {
        //La cantidad de bandejas reubicadas debe estar entre 0 y la cantidad de bandejas sin entregar de la locación  (Parameter 'relocatedSeedTrays')
        //La cantidad de bandejas reubicadas debe estar entre 0 y la cantidad de bandejas sin entregar de la locación (Parameter 'relocatedSeedTrays')
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
    }
}
