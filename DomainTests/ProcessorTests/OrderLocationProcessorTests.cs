using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests
{
    public class OrderLocationProcessorTests
    {
        [Fact]
        public void SaveSownOrderLocationChange_ShouldSowACompleteOrderLocation()
        {
            OrderLocation orderLocationToSow = new OrderLocation()
            {
                SeedTrayAmount = 100
            };

            DateOnly dateOfSow = new DateOnly(2023, 6, 25);

            short seedTraysToSow = 100;

            Mock<IOrderLocationRepository> repoMock = new Mock<IOrderLocationRepository>();

            repoMock.Setup(x => x.Update(It.IsAny<OrderLocation>())).Returns(true);
            repoMock.Setup(x => x.Insert(It.IsAny<OrderLocation>())).Returns(true);

            Mock<ILog> logMock = new Mock<ILog>();

            logMock.Setup(x => x.Info(It.IsAny<string>()));

            OrderLocationProcessor processor = new OrderLocationProcessor(logMock.Object, repoMock.Object);

            processor.SaveSownOrderLocationChange(orderLocationToSow, dateOfSow, seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().Be(dateOfSow);

            repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
            logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SaveSownOrderLocationChange_ShouldSowAPartialOrderLocation()
        {
            OrderLocation orderLocationToSow = new OrderLocation()
            {
                SeedTrayAmount = 100,
                SeedlingAmount = 26000
            };

            DateOnly dateOfSow = new DateOnly(2023, 6, 25);

            short alveolus = 260;
            short seedTraysToSow = 60;
            short restSeedTrays = 40;

            Mock<IOrderLocationRepository> repoMock = new Mock<IOrderLocationRepository>();

            OrderLocation orderLocationCopy = null;

            repoMock.Setup(x => x.Update(It.IsAny<OrderLocation>())).Returns(true);
            repoMock.Setup(x => x.Insert(It.IsAny<OrderLocation>())).Returns(true)
                .Callback<OrderLocation>(o => orderLocationCopy = o);

            Mock<ILog> logMock = new Mock<ILog>();

            logMock.Setup(x => x.Info(It.IsAny<string>()));

            OrderLocationProcessor processor = new OrderLocationProcessor(logMock.Object, repoMock.Object);

            processor.SaveSownOrderLocationChange(orderLocationToSow, dateOfSow, seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().Be(null);
            orderLocationCopy.RealSowDate.Should().Be(dateOfSow);

            orderLocationToSow.SeedTrayAmount.Should().Be(restSeedTrays);
            orderLocationCopy.SeedTrayAmount.Should().Be(seedTraysToSow);

            orderLocationToSow.SeedlingAmount.Should().Be(restSeedTrays * alveolus);
            orderLocationCopy.SeedlingAmount.Should().Be(seedTraysToSow * alveolus);

            repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Once);
            logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        }
    }
}
