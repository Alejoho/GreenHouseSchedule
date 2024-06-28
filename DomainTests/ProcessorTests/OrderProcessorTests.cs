using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests;

public class OrderProcessorTests
{
    private Mock<IOrderRepository> _repoMock;
    private Mock<ILog> _logMock;
    private OrderProcessor _processor;
    private DateOnly _date;

    public OrderProcessorTests()
    {
        _repoMock = new Mock<IOrderRepository>();

        _repoMock.Setup(x => x.Update(It.IsAny<Order>()));

        _logMock = new Mock<ILog>();

        _logMock.Setup(x => x.Info(It.IsAny<string>()));

        _processor = new OrderProcessor(_logMock.Object, _repoMock.Object);

        _date = new DateOnly(2023, 6, 25);
    }
    [Fact]
    public void UpdateOrderStatusAfterSow_ShouldUpdateTheDateAndTheSownProperty()
    {
        Order order = new Order()
        {
            OrderLocations = {
                new OrderLocation() { RealSowDate = new DateOnly(2023, 6, 25) }
            }
        };

        _processor.UpdateOrderStatusAfterSow(order, _date);

        order.RealSowDate.Should().Be(_date);
        order.Sown.Should().BeTrue();

        _repoMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Once);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        _repoMock.Invocations.Clear();
        _logMock.Invocations.Clear();
    }

    [Fact]
    public void UpdateOrderStatusAfterSow_ShouldUpdateOnlyTheDate()
    {
        Order order = new Order()
        {
            OrderLocations = {
                new OrderLocation() { RealSowDate = new DateOnly(2023, 6, 25) },
                new OrderLocation()
            }
        };

        _processor.UpdateOrderStatusAfterSow(order, _date);

        order.RealSowDate.Should().Be(_date);
        order.Sown.Should().BeFalse();

        _repoMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Once);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        _repoMock.Invocations.Clear();
        _logMock.Invocations.Clear();
    }

    [Fact]
    public void UpdateOrderStatusAfterSow_ShouldUpdateOnlyTheSownProperty()
    {
        Order order = new Order()
        {
            RealSowDate = _date,
            OrderLocations = {
                new OrderLocation() { RealSowDate = _date },
                new OrderLocation() { RealSowDate = new DateOnly(2023, 6, 27) }
            }
        };

        _processor.UpdateOrderStatusAfterSow(order, new DateOnly(2023, 6, 27));

        order.RealSowDate.Should().Be(_date);
        order.Sown.Should().BeTrue();

        _repoMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Once);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        _repoMock.Invocations.Clear();
        _logMock.Invocations.Clear();
    }

    [Fact]
    public void UpdateOrderStatusAfterSow_ShouldUpdateNothing()
    {
        Order order = new Order()
        {
            RealSowDate = _date,
            OrderLocations = {
                new OrderLocation() { RealSowDate = _date },
                new OrderLocation() { RealSowDate = new DateOnly(2023, 6, 27) },
                new OrderLocation()
            }
        };

        _processor.UpdateOrderStatusAfterSow(order, new DateOnly(2023, 6, 27));

        order.RealSowDate.Should().Be(_date);
        order.Sown.Should().BeFalse();

        _repoMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Never);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
        _repoMock.Invocations.Clear();
        _logMock.Invocations.Clear();
    }
}
