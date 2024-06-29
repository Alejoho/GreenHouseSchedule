using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests
{
    public class OrderLocationProcessorTests
    {
        private Mock<IOrderLocationRepository> _repoMock;
        private Mock<ILog> _logMock;
        private OrderLocationProcessor _processor;
        private DateOnly _dateOfSow;
        private OrderLocation _orderLocationCopy;

        public OrderLocationProcessorTests()
        {
            _repoMock = new Mock<IOrderLocationRepository>();

            _repoMock.Setup(x => x.Update(It.IsAny<OrderLocation>())).Returns(true);
            _repoMock.Setup(x => x.Insert(It.IsAny<OrderLocation>())).Returns(true)
                .Callback<OrderLocation>(o => _orderLocationCopy = o);

            _logMock = new Mock<ILog>();

            _logMock.Setup(x => x.Info(It.IsAny<string>()));

            _processor = new OrderLocationProcessor(_logMock.Object, _repoMock.Object);

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

            _repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            _repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _repoMock.Invocations.Clear();
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

            _repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            _repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Once);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _repoMock.Invocations.Clear();
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

            _processor.SaveSownOrderLocationChange(orderLocationToSow, _dateOfSow.AddDays(3), seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().BeNull();
            _orderLocationCopy.RealSowDate.Should().BeNull();

            _repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
            _repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
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

            _processor.SaveSownOrderLocationChange(orderLocationToSow, _dateOfSow.AddDays(3), seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().BeNull();
            _orderLocationCopy.RealSowDate.Should().BeNull();

            _repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Never);
            _repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
        }
    }
}
