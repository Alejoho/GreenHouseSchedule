using DataAccess.Contracts;
using Domain;
using Domain.Models;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace DomainTests;
public class SeedBedStatusTests
{
    private static DateOnly _presentDate = new DateOnly(2023, 6, 10);
    private static DateOnly _pastDate = _presentDate.AddDays(-90);

    public SeedBedStatusTests()
    {
        if (!RecordGenerator.generated == true)
        {
            RecordGenerator.PopulateLists(600);
            //RecordGenerator.PopulateLists(150);
            MockOf.GenerateMocks(_pastDate);
            RecordGenerator.generated = true;            
        }
    }

    [Fact]
    public void GetGreenHouses_ShouldReturnAllGreenHouses()
    {
        int amountOfRecords = 5;
        var mockGreenHouseRepository = MockOf.GetCustomGreenHouseMock(amountOfRecords);

        SeedBedStatus status = new SeedBedStatus(
            greenHouseRepo: mockGreenHouseRepository.Object);

        status.GreenHouses.Should().HaveCount(amountOfRecords);
        status.GreenHouses[0].Should().BeOfType(typeof(GreenHouseModel));
        mockGreenHouseRepository.Verify(x => x.GetAll(), Times.Once());
    }

    [Fact]
    public void GetSeedTrays_ShouldReturnAllSeedTrays()
    {
        int amountOfRecords = 5;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.GetCustomSeedTrayMock(amountOfRecords);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object);

        status.SeedTrays.Should().HaveCount(amountOfRecords);
        status.SeedTrays[0].Should().BeOfType(typeof(SeedTrayModel));
        mockSeedTrayRepository.Verify(x => x.GetAll(), Times.Once());
    }

    [Fact]
    public void GetMajorityDataOfOrders_ShouldRetrieveTheOrders()
    {
        int amountOfRecords = 50;
        Mock<IOrderProcessor> mockOrderProcessor = MockOf.GetCustomOrderMock(amountOfRecords);

        SeedBedStatus status = new SeedBedStatus(
            orderProcessor: mockOrderProcessor.Object);
      
        status.Orders.Count.Should().Be(amountOfRecords);
        status.Orders.First.Should().BeOfType(typeof(LinkedListNode<OrderModel>));

        mockOrderProcessor.Verify(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>())
            , Times.Once());
    }

    [Fact]
    public void GetOrderLocations_ShouldRetrieveTheOrderLocations()
    {
        int amountOfRecords = 20;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.GetCustomOrderLocationMock(amountOfRecords);

        SeedBedStatus status = new SeedBedStatus(
            orderLocationProcessor: mockOrderLocationProcessor.Object);
        
        status.OrderLocations.Count.Should().Be(amountOfRecords);
        status.OrderLocations.First.Should().BeOfType(typeof(LinkedListNode<OrderLocationModel>));

        mockOrderLocationProcessor.
            Verify(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>())
            , Times.Once());
    }

    [Fact]
    public void GetDeliveryDetails_ShouldRetrieveTheDeliveryDetails()
    {
        int amountOfRecords = 20;

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = MockOf.GetCustomDeliveryDetailMock(amountOfRecords);

        SeedBedStatus status = new SeedBedStatus(
            deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);
       
        status.DeliveryDetails.Count.Should().Be(amountOfRecords);
        status.DeliveryDetails.First().Should().BeOfType(typeof(DeliveryDetailModel));

        mockDeliveryDetailProcessor.
            Verify(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>())
            , Times.Once());
    }

    [Fact]
    public void FillDeliveryDetails_ShouldPopulateTheDeliveryDetailsOfTheOrderLocations()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var orderLocationCollection = RecordGenerator._orderLocations
            .Where(x => x.SowDate >= pastDate || x.SowDate == null)
            .OrderBy(x => x.SowDate)
            .ThenBy(x => x.Id);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderLocationCollection);

        var deliveryDetailCollection = RecordGenerator._deliveryDetails.Where(x => x.DeliveryDate > pastDate)
            .OrderBy(x => x.DeliveryDate);

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

        mockDeliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(deliveryDetailCollection);

        SeedBedStatus status = new SeedBedStatus(presentDate
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        MethodInfo methodInfo_FillDeliveryDetails = typeof(SeedBedStatus)
            .GetMethod("FillDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_FillDeliveryDetails.Invoke(status, null);

        status.OrderLocations.Count.Should().Be(orderLocationCollection.Count());

        int deliveryDetailModelsCount = status.OrderLocations.Sum(x => x.DeliveryDetails.Count);

        deliveryDetailModelsCount.Should().BeLessThan(deliveryDetailCollection.Count());

        foreach (var orderLocationModel in status.OrderLocations)
        {
            if (orderLocationModel.RealDeliveryDate != null)
            {
                orderLocationModel.RealDeliveryDate.Should().Be(orderLocationModel.DeliveryDetails.First().DeliveryDate);
            }
            else
            {
                orderLocationModel.DeliveryDetails.Count.Should().Be(0);
            }
        }

        mockOrderLocationProcessor.Verify(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>())
            , Times.Once);

        mockDeliveryDetailProcessor.Verify(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>())
            , Times.Once);
    }

    [Fact]
    public void FillOrderLocations_ShouldPopulateTheOrderLocationsOfTheOrders()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var orderCollection = RecordGenerator._orders.Where(x => x.RealSowDate >= pastDate || x.RealSowDate == null)
            .OrderBy(x => x.EstimateSowDate)
            .ThenBy(x => x.DateOfRequest);

        Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();

        mockOrderProcessor
            .Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderCollection);

        var orderLocationCollection = RecordGenerator._orderLocations.Where(x => x.SowDate > pastDate || x.SowDate == null)
            .OrderBy(x => x.SowDate)
            .ThenBy(x => x.Id);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderLocationCollection);

        SeedBedStatus status = new SeedBedStatus(presentDate
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        MethodInfo methodInfo_FillOrderLocations = typeof(SeedBedStatus)
            .GetMethod("FillOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_FillOrderLocations.Invoke(status, null);

        status.Orders.Count.Should().Be(orderCollection.Count());

        status.OrderLocations.Count.Should().Be(orderLocationCollection.Count());

        status.OrderLocations.Count.Should().BeLessThan(RecordGenerator._orderLocations.Count);

        mockOrderProcessor.Verify(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>())
            , Times.Once);

        mockOrderLocationProcessor.Verify(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>())
            , Times.Once);
    }

    [Theory]
    [InlineData(250, 3)]
    [InlineData(50, 1)]
    [InlineData(86, 5)]
    [InlineData(310, 7)]
    public void ReleaseSeedTray_ShouldWork(int amount, int seedTrayType)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();
        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._seedTrays);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object);

        status.ReleaseSeedTray(amount, seedTrayType);

        SeedTrayModel seedTray = status.SeedTrays.Where(x => x.ID == seedTrayType).First();
        seedTray.FreeAmount.Should().Be(seedTray.TotalAmount + amount);
        seedTray.UsedAmount.Should().Be(0 - amount);

        mockSeedTrayRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Theory]
    [InlineData(125, 2)]
    [InlineData(64, 4)]
    [InlineData(245, 1)]
    [InlineData(166, 6)]
    public void ReserveSeedTray_ShouldWork(int amount, int seedTrayType)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();
        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._seedTrays);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object);

        status.ReserveSeedTray(amount, seedTrayType);

        SeedTrayModel seedTray = status.SeedTrays.Where(x => x.ID == seedTrayType).First();
        seedTray.FreeAmount.Should().Be(seedTray.TotalAmount - amount);
        seedTray.UsedAmount.Should().Be(0 + amount);

        mockSeedTrayRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Theory]
    [InlineData(123, 3, 4)]
    [InlineData(321, 5, 8)]
    [InlineData(222, 1, 2)]
    [InlineData(307, 6, 6)]
    public void ReleaseArea_ShouldWork(int amount, int seedTrayType, int greenHouse)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();
        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._seedTrays);

        Mock<IGreenHouseRepository> mockGreenHouseRepository =
            new Mock<IGreenHouseRepository>();
        mockGreenHouseRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._greenHouses);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object,
            greenHouseRepo: mockGreenHouseRepository.Object);

        status.ReleaseArea(amount, seedTrayType, greenHouse);

        GreenHouseModel selectedGreenHouse = status.GreenHouses.Where(x => x.ID == greenHouse).First();
        SeedTrayModel selectedSeedTray = status.SeedTrays.Where(x => x.ID == seedTrayType).First();

        selectedGreenHouse.SeedTrayAvailableArea.Should()
            .Be(selectedGreenHouse.SeedTrayTotalArea + (selectedSeedTray.Area * amount));

        selectedGreenHouse.SeedTrayUsedArea.Should()
            .Be(0 - (selectedSeedTray.Area * amount));

        mockSeedTrayRepository.Verify(x => x.GetAll(), Times.Once);
        mockGreenHouseRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Theory]
    [InlineData(76, 1, 7)]
    [InlineData(263, 3, 6)]
    [InlineData(111, 6, 5)]
    [InlineData(47, 5, 3)]
    public void ReserveArea_ShouldWork(int amount, int seedTrayType, int greenHouse)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();
        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._seedTrays);

        Mock<IGreenHouseRepository> mockGreenHouseRepository =
            new Mock<IGreenHouseRepository>();
        mockGreenHouseRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._greenHouses);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object,
            greenHouseRepo: mockGreenHouseRepository.Object);

        status.ReserveArea(amount, seedTrayType, greenHouse);

        GreenHouseModel selectedGreenHouse = status.GreenHouses.Where(x => x.ID == greenHouse).First();
        SeedTrayModel selectedSeedTray = status.SeedTrays.Where(x => x.ID == seedTrayType).First();

        selectedGreenHouse.SeedTrayAvailableArea.Should()
            .Be(selectedGreenHouse.SeedTrayTotalArea - (selectedSeedTray.Area * amount));

        selectedGreenHouse.SeedTrayUsedArea.Should()
            .Be(0 + (selectedSeedTray.Area * amount));

        mockSeedTrayRepository.Verify(x => x.GetAll(), Times.Once);
        mockGreenHouseRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public void RemoveDeliveryDetails_ShouldRemoveAllDeliveriesOfTheDay()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var collection = RecordGenerator._deliveryDetails
            .Where(x => x.DeliveryDate >= pastDate);

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

        mockDeliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(collection);

        SeedBedStatus status = new SeedBedStatus(presentDate: presentDate
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        MethodInfo methodInfo_GetDeliveryDetails = typeof(SeedBedStatus)
            .GetMethod("GetDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        status.DeliveryDetails = (List<DeliveryDetailModel>)methodInfo_GetDeliveryDetails.Invoke(status, null);

        MethodInfo methodInfo_RemoveDeliveryDetails = typeof(SeedBedStatus)
            .GetMethod("RemoveDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_RemoveDeliveryDetails.Invoke(status, null);

        status.DeliveryDetails.Count.Should().Be(collection.Where(x => x.DeliveryDate != pastDate).Count());
    }

    [Fact]
    public void RemoveOrderLocations_ShouldRemoveAllOrderLocationsOfTheDay()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var collection = RecordGenerator._orderLocations
            .Where(x => x.SowDate >= pastDate || x.SowDate == null);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(collection);

        SeedBedStatus status = new SeedBedStatus(presentDate: presentDate
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        status.OrderLocations = (LinkedList<OrderLocationModel>)methodInfo.Invoke(status, null);

        var orderLocationsToDelete = status.OrderLocations.Where(x => x.SowDate == pastDate).ToList();

        status.OrderLocationsToDelete = new System.Collections.ArrayList(orderLocationsToDelete);

        MethodInfo methodInfo_RemoveOrderLocations = typeof(SeedBedStatus)
            .GetMethod("RemoveOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_RemoveOrderLocations.Invoke(status, null);

        status.OrderLocations.Count.Should()
            .Be(collection.Where(x => x.SowDate != pastDate || x.SowDate == null).Count());
    }

    [Fact]
    public void RemoveOrder_ShouldRemoveAllOrdersOfTheDay()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var collection = RecordGenerator._orders
            .Where(x => x.RealSowDate >= pastDate || x.RealSowDate == null);

        Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();

        mockOrderProcessor
            .Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
            .Returns(collection);

        SeedBedStatus status = new SeedBedStatus(presentDate: presentDate
            , orderProcessor: mockOrderProcessor.Object);

        MethodInfo methodInfo_GetMajorityDataOfOrders = typeof(SeedBedStatus)
            .GetMethod("GetMajorityDataOfOrders",
            BindingFlags.NonPublic | BindingFlags.Instance);

        status.Orders = (LinkedList<OrderModel>)methodInfo_GetMajorityDataOfOrders.Invoke(status, null);

        var ordersToDelete = status.Orders.Where(x => x.RealSowDate == pastDate).ToList();

        status.OrdersToDelete = new System.Collections.ArrayList(ordersToDelete);

        MethodInfo methodInfo_RemoveOrders = typeof(SeedBedStatus)
            .GetMethod("RemoveOrders",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_RemoveOrders.Invoke(status, null);

        status.Orders.Count.Should()
            .Be(collection.Where(x => x.RealSowDate != pastDate || x.RealSowDate == null).Count());
    }

    [Fact]
    public void AddOrderLocations_ShouldAddNewOrderLocationsToTheirOrders()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var orderCollection = RecordGenerator._orders;

        Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();

        mockOrderProcessor
            .Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderCollection);

        var orderLocationCollection = RecordGenerator._orderLocations;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderLocationCollection);

        SeedBedStatus status = new SeedBedStatus(presentDate
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        MethodInfo methodInfo_GetMajorityDataOfOrders = typeof(SeedBedStatus)
            .GetMethod("GetMajorityDataOfOrders",
            BindingFlags.NonPublic | BindingFlags.Instance);

        status.Orders = (LinkedList<OrderModel>)methodInfo_GetMajorityDataOfOrders.Invoke(status, null);

        MethodInfo methodInfo_GetOrderLocations = typeof(SeedBedStatus)
            .GetMethod("GetOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        status.OrderLocations =
                (LinkedList<OrderLocationModel>)methodInfo_GetOrderLocations.Invoke(status, null);

        MethodInfo methodInfo_FillOrderLocations = typeof(SeedBedStatus)
            .GetMethod("FillOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_FillOrderLocations.Invoke(status, null);

        int newOrderLocationAmount = 24;

        var newOrderLocations = RecordGenerator.GenerateOrderLocations(newOrderLocationAmount);

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(newOrderLocations);

        status.OrderLocationsToAdd = new System.Collections.ArrayList(
        (LinkedList<OrderLocationModel>)methodInfo_GetOrderLocations.Invoke(status, null));

        MethodInfo methodInfo_AddOrderLocations = typeof(SeedBedStatus)
            .GetMethod("AddOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_AddOrderLocations.Invoke(status, null);

        int sumOrderLocations = status.Orders.Sum(x => x.OrderLocations.Count);

        sumOrderLocations.Should().Be(orderLocationCollection.Count + newOrderLocationAmount);
    }

    [Fact]
    //TODO - When I raise the orders to 600 this test failed
    public void ImplementReservation_ShouldWork()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var greenHouseCollection = RecordGenerator._greenHouses;

        Mock<IGreenHouseRepository> mockGreenHouseRepository =
            new Mock<IGreenHouseRepository>();

        mockGreenHouseRepository.Setup(x => x.GetAll()).Returns(greenHouseCollection);

        var seedTrayCollection = RecordGenerator._seedTrays;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();

        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(seedTrayCollection);

        var orderCollection = RecordGenerator._orders
            .Where(x => x.RealSowDate >= pastDate || x.RealSowDate == null)
            .OrderBy(x => x.EstimateSowDate)
            .ThenBy(x => x.DateOfRequest);

        Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();

        mockOrderProcessor
            .Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderCollection);

        var orderLocationCollection = RecordGenerator._orderLocations
            .Where(x => x.Order.RealSowDate >= pastDate
                && (x.SowDate >= pastDate || x.SowDate == null))
            .OrderBy(x => x.SowDate)
            .ThenBy(x => x.Id);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderLocationCollection);

        SeedBedStatus status = new SeedBedStatus(presentDate: presentDate
            , greenHouseRepo: mockGreenHouseRepository.Object
            , seedTrayRepo: mockSeedTrayRepository.Object
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("ImplementReservation",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(status, null);

        var orderLocationsSelected = status.OrderLocations.Where(x => x.SowDate == status.IteratorDate);

        foreach (var orderLocation in orderLocationsSelected)
        {
            var bandejas = status.SeedTrays
                .Where(x => x.ID == orderLocation.SeedTrayType)
                .First();
            int usado = bandejas.UsedAmount;
            usado.Should()
                .BeGreaterThanOrEqualTo(orderLocation.SeedTrayAmount);
        }
    }

    [Fact]
    public void ImplementRelease_ShouldWork()
    {
        var greenHouseCollection = RecordGenerator._greenHouses;

        Mock<IGreenHouseRepository> mockGreenHouseRepository =
            new Mock<IGreenHouseRepository>();

        mockGreenHouseRepository.Setup(x => x.GetAll()).Returns(greenHouseCollection);

        var seedTrayCollection = RecordGenerator._seedTrays;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();

        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(seedTrayCollection);

        var orderCollection = RecordGenerator._orders
            .Where(x => x.RealSowDate >= _pastDate || x.RealSowDate == null)
            .OrderBy(x => x.EstimateSowDate)
            .ThenBy(x => x.DateOfRequest);

        Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();

        mockOrderProcessor
            .Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderCollection);

        var orderLocationCollection = RecordGenerator._orderLocations
            .Where(x => x.SowDate >= _pastDate || x.SowDate == null)
            .OrderBy(x => x.SowDate)
            .ThenBy(x => x.Id);

        //int count = orderLocationCollection.Where(x => x.SowDate != null).Sum(x => x.SeedTrayAmount);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderLocationCollection);

        var deliveryDetailCollection = RecordGenerator._deliveryDetails
            .Where(x => x.DeliveryDate >= _pastDate);

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

        mockDeliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(deliveryDetailCollection);

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , greenHouseRepo: mockGreenHouseRepository.Object
            , seedTrayRepo: mockSeedTrayRepository.Object
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        status.IteratorDate = new DateOnly(2023, 4, 11);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("ImplementRelease",
            BindingFlags.NonPublic | BindingFlags.Instance);
        //NEXT - I've run this method 5 times and the arraylists to delete are empty. This shouldn't be this way
        for (int i = 0; i < 5; i++)
        {
            methodInfo.Invoke(status, null);
            status.IteratorDate.AddDays(1);
        }
        status.OrderLocationsToDelete.Count.Should().Be(2);
        status.OrdersToDelete.Count.Should().Be(1);
    }
}
