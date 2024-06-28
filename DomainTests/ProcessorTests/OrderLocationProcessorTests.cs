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

            string capturedMessage = "";

            void SetString(string message)
            {
                capturedMessage = message;
            }

            logMock.Setup(x => x.Info(It.IsAny<string>())).Callback<string>((m) => SetString(m));

            OrderLocationProcessor processor = new OrderLocationProcessor(logMock.Object, repoMock.Object);

            processor.SaveSownOrderLocationChange(orderLocationToSow, dateOfSow, seedTraysToSow);

            orderLocationToSow.RealSowDate.Should().Be(dateOfSow);
            capturedMessage.Should().Be("An OrderLocation was sown and updated to the DB");

            repoMock.Verify(x => x.Update(It.IsAny<OrderLocation>()), Times.Once);
            repoMock.Verify(x => x.Insert(It.IsAny<OrderLocation>()), Times.Never);
        }

{
    internal class OrderLocationProcessorTests
    {
    }
}
