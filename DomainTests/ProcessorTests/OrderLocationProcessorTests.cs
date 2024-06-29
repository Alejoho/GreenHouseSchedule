using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests
{
    public class OrderLocationProcessorTests
    {
        private Mock<IOrderLocationRepository> _orderLocationRepoMock;
        private Mock<IBlockRepository> _blockRepoMock;
        private Mock<ILog> _logMock;
        private OrderLocationProcessor _processor;
        private DateOnly _dateOfSow;
        private OrderLocation _orderLocationCopy;
        private Block _newBlock;

        public OrderLocationProcessorTests()
        {
            _orderLocationRepoMock = new Mock<IOrderLocationRepository>();

            _orderLocationRepoMock.Setup(x => x.Update(It.IsAny<OrderLocation>())).Returns(true);
            _orderLocationRepoMock.Setup(x => x.Insert(It.IsAny<OrderLocation>())).Returns(true)
                .Callback<OrderLocation>(o =>
                {
                    _orderLocationCopy = o; _orderLocationCopy.Id = 22;
                });
            _orderLocationRepoMock.Setup(x => x.Remove(It.IsAny<int>()));

            _blockRepoMock = new Mock<IBlockRepository>();

            _blockRepoMock.Setup(x => x.Insert(It.IsAny<Block>()))
                .Callback<Block>(b => _newBlock = b);

            _logMock = new Mock<ILog>();

            _logMock.Setup(x => x.Info(It.IsAny<string>()));

            _processor = new OrderLocationProcessor(_logMock.Object
                , _orderLocationRepoMock.Object
                , _blockRepoMock.Object);

            _dateOfSow = DateOnly.FromDateTime(DateTime.Now);
        }

        [Fact]
        public void SaveSownOrderLocationChange_ShouldSowACompleteOrderLocation()
        {
            OrderLocation orderLocationToSow = new OrderLocation()
            {
                SeedTrayAmount = 100
            };

            short seedTraysToSow = 100;

            _processor.SaveSownOrderLocationChange(orderLocationToSow, _dateOfSow, seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().Be(_dateOfSow);
            _orderLocationCopy.Should().BeNull();

            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _orderLocationRepoMock.Invocations.Clear();
            _logMock.Invocations.Clear();
        }

        [Fact]
        public void SaveSownOrderLocationChange_ShouldSowAPartialOrderLocation()
        {
            OrderLocation orderLocationToSow = new OrderLocation()
            {
                SeedTrayAmount = 100,
                SeedlingAmount = 26000
            };

            short alveolus = 260;
            short seedTraysToSow = 60;
            short restSeedTrays = 40;

            _processor.SaveSownOrderLocationChange(orderLocationToSow, _dateOfSow, seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().BeNull();
            _orderLocationCopy.RealSowDate.Should().Be(_dateOfSow);

            orderLocationToSow.SeedTrayAmount.Should().Be(restSeedTrays);
            _orderLocationCopy.SeedTrayAmount.Should().Be(seedTraysToSow);

            orderLocationToSow.SeedlingAmount.Should().Be(restSeedTrays * alveolus);
            _orderLocationCopy.SeedlingAmount.Should().Be(seedTraysToSow * alveolus);

            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Once);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _orderLocationRepoMock.Invocations.Clear();
            _logMock.Invocations.Clear();
        }

        [Fact]
        public void SaveSownOrderLocationChange_ShouldThrowAnArgumentExceptionOnTheDate()
        {
            OrderLocation orderLocationToSow = new OrderLocation()
            {
                SeedTrayAmount = 100
            };

            short seedTraysToSow = 100;

            Action action = () => _processor.SaveSownOrderLocationChange(orderLocationToSow, _dateOfSow.AddDays(3), seedTraysToSow);

            action.Should().Throw<ArgumentException>().WithParameterName("date")
                .WithMessage("La fecha debe ser igual o anterior que el día presente (Parameter 'date')");

            orderLocationToSow.RealSowDate.Should().BeNull();
            _orderLocationCopy.Should().BeNull();

            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void SaveSownOrderLocationChange_ShouldThrowAnArgumentExceptionOnTheSeedTrays()
        {
            OrderLocation orderLocationToSow = new OrderLocation()
            {
                SeedTrayAmount = 100
            };

            short seedTraysToSow = 125;

            Action action = () => _processor.SaveSownOrderLocationChange(orderLocationToSow, _dateOfSow, seedTraysToSow);

            action.Should().Throw<ArgumentException>().WithParameterName("sownSeedTrays")
                .WithMessage("La cantidad de bandejas sembradas debe estar entre 0 y la cantidad de bandejas de la Locación (Parameter 'sownSeedTrays')");

            orderLocationToSow.RealSowDate.Should().BeNull();
            _orderLocationCopy.Should().BeNull();

            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
        }



        [Fact]
        public void SavePlacedOrderLocationChange_ShouldSaveACompleteWithoutBrothersOrderLocation()
        {
            Order order = GetOrderRecord();
            OrderLocation orderLocationToProcess = order.OrderLocations.First();
            byte greenHouse = 2;
            byte block = 3;
            short placedSeedTrays = 100;

            _processor.SavePlacedOrderLocationChange(orderLocationToProcess, greenHouse, block, placedSeedTrays);

            orderLocationToProcess.GreenHouseId.Should().Be(greenHouse);
            _newBlock.OrderLocationId.Should().Be(orderLocationToProcess.Id);
            _newBlock.BlockNumber.Should().Be(block);
            _newBlock.SeedTrayAmount.Should().Be(placedSeedTrays);

            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);

            _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);

            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void SavePlacedOrderLocationChange_ShouldSaveAPartialWithoutBrothersOrderLocation()
        {
            Order order = GetOrderRecord();
            OrderLocation orderLocationToProcess = order.OrderLocations.First();
            short originalSeedTrays = orderLocationToProcess.SeedTrayAmount;
            int originalSeedling = orderLocationToProcess.SeedlingAmount;
            int alveolus = originalSeedling / originalSeedTrays;
            byte greenHouse = 2;
            byte block = 3;
            short placedSeedTrays = 80;

            _processor.SavePlacedOrderLocationChange(orderLocationToProcess, greenHouse, block, placedSeedTrays);

            orderLocationToProcess.SeedTrayAmount.Should().Be((short)(originalSeedTrays - placedSeedTrays));
            orderLocationToProcess.SeedlingAmount.Should().Be((originalSeedTrays - placedSeedTrays) * alveolus);

            _orderLocationCopy.GreenHouseId.Should().Be(greenHouse);
            _orderLocationCopy.SeedTrayAmount.Should().Be(placedSeedTrays);
            _orderLocationCopy.SeedlingAmount.Should().Be(placedSeedTrays * alveolus);

            _newBlock.OrderLocationId.Should().Be(_orderLocationCopy.Id);
            _newBlock.BlockNumber.Should().Be(block);
            _newBlock.SeedTrayAmount.Should().Be(placedSeedTrays);

            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Once);
            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);

            _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);

            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void SavePlacedOrderLocationChange_ShouldSaveACompleteWithBrothersOrderLocation()
        {
            Order order = GetOrderRecord();
            OrderLocation orderLocationToProcess = order.OrderLocations.First();
            OrderLocation orderLocationBrother = order.OrderLocations.First(x => x.Id == 6);
            short originalSeedTraysOfTheBrother = orderLocationBrother.SeedTrayAmount;
            int originalSeedlingOfTheBrother = orderLocationBrother.SeedlingAmount;
            int alveolus = originalSeedlingOfTheBrother / originalSeedTraysOfTheBrother;
            byte greenHouse = 4;
            byte block = 3;
            short placedSeedTrays = 100;

            _processor.SavePlacedOrderLocationChange(orderLocationToProcess, greenHouse, block, placedSeedTrays);

            orderLocationBrother.GreenHouseId.Should().Be(greenHouse);
            orderLocationBrother.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheBrother + placedSeedTrays));
            orderLocationBrother.SeedlingAmount.Should().Be(originalSeedlingOfTheBrother + (placedSeedTrays * alveolus));

            _newBlock.OrderLocationId.Should().Be(orderLocationBrother.Id);
            _newBlock.BlockNumber.Should().Be(block);
            _newBlock.SeedTrayAmount.Should().Be(placedSeedTrays);

            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Once);

            _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);

            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void SavePlacedOrderLocationChange_ShouldSaveAPartialWithBrothersOrderLocation()
        {
            Order order = GetOrderRecord();
            OrderLocation orderLocationToProcess = order.OrderLocations.First();
            short originalSeedTrays = orderLocationToProcess.SeedTrayAmount;
            int originalSeedling = orderLocationToProcess.SeedlingAmount;
            OrderLocation orderLocationBrother = order.OrderLocations.First(x => x.Id == 6);
            short originalSeedTraysOfTheBrother = orderLocationBrother.SeedTrayAmount;
            int originalSeedlingOfTheBrother = orderLocationBrother.SeedlingAmount;
            int alveolus = originalSeedlingOfTheBrother / originalSeedTraysOfTheBrother;
            byte greenHouse = 4;
            byte block = 3;
            short placedSeedTrays = 40;

            _processor.SavePlacedOrderLocationChange(orderLocationToProcess, greenHouse, block, placedSeedTrays);

            orderLocationToProcess.SeedTrayAmount.Should().Be((short)(originalSeedTrays - placedSeedTrays));
            orderLocationToProcess.SeedlingAmount.Should().Be(originalSeedling - (placedSeedTrays * alveolus));

            orderLocationBrother.SeedTrayAmount.Should().Be((short)(originalSeedTraysOfTheBrother + placedSeedTrays));
            orderLocationBrother.SeedlingAmount.Should().Be(originalSeedlingOfTheBrother + (placedSeedTrays * alveolus));

            _newBlock.OrderLocationId.Should().Be(orderLocationBrother.Id);
            _newBlock.BlockNumber.Should().Be(block);
            _newBlock.SeedTrayAmount.Should().Be(placedSeedTrays);

            _orderLocationRepoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _orderLocationRepoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Exactly(2));
            _orderLocationRepoMock.Verify(x => x.Remove(It.IsAny<int>()), Times.Never);

            _blockRepoMock.Verify(x => x.Insert(It.IsAny<Block>()), Times.Once);

            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(3));
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
                GreenHouseId = 0,
                SeedTrayId = 1,
                RealSowDate = new DateOnly(2023, 6, 25)
            };

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
                GreenHouseId = 0,
                SeedTrayId = 2,
                RealSowDate = new DateOnly(2023, 6, 27)
            };

            order.OrderLocations.Add(orderLocation1);
            order.OrderLocations.Add(orderLocation2);
            order.OrderLocations.Add(orderLocation3);

            return order;
        }

    }
}
