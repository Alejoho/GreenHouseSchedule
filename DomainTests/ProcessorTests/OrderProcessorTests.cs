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

    [Fact]
    public void UpdateOrderStatusAfterDelivery_ShouldUpdateNothing()
    {
        Order order = GetOrderRecordWithoutDelivery();

        _processor.UpdateOrderStatusAfterDelivery(order);

        order.Delivered.Should().BeFalse();
        _repoMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public void UpdateOrderStatusAfterDelivery_ShouldUpdateTheOrder()
    {
        Order order = GetOrderRecordCompletelyDelivered();

        _processor.UpdateOrderStatusAfterDelivery(order);

        order.Delivered.Should().BeTrue();
        _repoMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Once);
    }

    private Order GetOrderRecordWithoutDelivery()
    {
        Order order = new Order()
        {
            Id = 3,
            AmountOfWishedSeedlings = 40500,
            AmountOfAlgorithmSeedlings = 48600,
            RealSowDate = new DateOnly(2023, 6, 25),
            Sown = true,
            Delivered = false
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

    private Order GetOrderRecordCompletelyDelivered()
    {
        Order order = new Order()
        {
            Id = 3,
            AmountOfWishedSeedlings = 40500,
            AmountOfAlgorithmSeedlings = 48600,
            RealSowDate = new DateOnly(2023, 6, 25),
            Sown = true,
            Delivered = false
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
            OrderLocation = orderLocation1,
            DeliveryDetails = { new DeliveryDetail() { SeedTrayAmountDelivered = 100 } }
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
            OrderLocation = orderLocation2,
            DeliveryDetails = { new DeliveryDetail() { SeedTrayAmountDelivered = 50 } }
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
            OrderLocation = orderLocation3,
            DeliveryDetails = { new DeliveryDetail() { SeedTrayAmountDelivered = 60 } }
        };

        orderLocation3.Blocks.Add(block3);

        order.OrderLocations.Add(orderLocation1);
        order.OrderLocations.Add(orderLocation2);
        order.OrderLocations.Add(orderLocation3);

        return order;
    }

}
