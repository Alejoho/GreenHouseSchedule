using DataAccess.Contracts;
using Domain;
using Domain.Models;
using FluentAssertions;
using Moq;
using System.Reflection;
//LATER - improve the name of some tests. Mainly the last ones.
namespace DomainTests;
public class SeedBedStatusTests
{
    private static DateOnly _presentDate = new DateOnly(2023, 6, 10);
    private static DateOnly _pastDate = _presentDate.AddDays(-90);

    public SeedBedStatusTests()
    {
        if (!RecordGenerator.Generated == true)
        {
            RecordGenerator.PopulateLists(600);
            RecordGenerator.FillNumberOfRecords(_pastDate);
            MockOf.GenerateMocks(_pastDate);
            RecordGenerator.Generated = true;
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
        mockGreenHouseRepository.VerifyAll();
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
        mockSeedTrayRepository.VerifyAll();
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

        mockOrderProcessor.VerifyAll();
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

        mockOrderLocationProcessor.VerifyAll();
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

        mockDeliveryDetailProcessor.VerifyAll();
    }

    [Fact]
    public void FillDeliveryDetails_ShouldPopulateTheDeliveryDetailsOfTheOrderLocations()
    {
        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = MockOf.DeliveryDetailProcessor;

        SeedBedStatus status = new SeedBedStatus(_presentDate
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        MethodInfo methodInfo_FillDeliveryDetails = typeof(SeedBedStatus)
            .GetMethod("FillDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_FillDeliveryDetails.Invoke(status, null);

        status.OrderLocations.Count.Should().Be(RecordGenerator.NumberOfSelectedOrderLocations);

        int deliveryDetailModelsCount = status.OrderLocations.Sum(x => x.DeliveryDetails.Count);

        deliveryDetailModelsCount.Should().BeLessThan(RecordGenerator.DeliveryDetails.Count);

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

        mockOrderLocationProcessor.VerifyAll();

        mockDeliveryDetailProcessor.VerifyAll();
    }

    [Fact]
    public void FillOrderLocations_ShouldPopulateTheOrderLocationsOfTheOrders()
    {
        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        SeedBedStatus status = new SeedBedStatus(_presentDate
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        MethodInfo methodInfo_FillOrderLocations = typeof(SeedBedStatus)
            .GetMethod("FillOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_FillOrderLocations.Invoke(status, null);

        status.Orders.Count.Should().Be(RecordGenerator.NumberOfSelectedOrders);

        status.OrderLocations.Count.Should().Be(RecordGenerator.NumberOfSelectedOrderLocations);

        status.OrderLocations.Count.Should().BeLessThan(RecordGenerator.OrderLocations.Count);

        mockOrderProcessor.VerifyAll();

        mockOrderLocationProcessor.VerifyAll();
    }

    [Theory]
    [InlineData(250, 3)]
    [InlineData(50, 1)]
    [InlineData(86, 5)]
    [InlineData(310, 7)]
    public void ReleaseSeedTray_ShouldWork(int amount, int seedTrayType)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object);

        status.ReleaseSeedTray(amount, seedTrayType);

        SeedTrayModel seedTray = status.SeedTrays.Where(x => x.ID == seedTrayType).First();
        seedTray.FreeAmount.Should().Be(seedTray.TotalAmount + amount);
        seedTray.UsedAmount.Should().Be(0 - amount);

        mockSeedTrayRepository.VerifyAll();
    }

    [Theory]
    [InlineData(125, 2)]
    [InlineData(64, 4)]
    [InlineData(245, 1)]
    [InlineData(166, 6)]
    public void ReserveSeedTray_ShouldWork(int amount, int seedTrayType)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object);

        status.ReserveSeedTray(amount, seedTrayType);

        SeedTrayModel seedTray = status.SeedTrays.Where(x => x.ID == seedTrayType).First();
        seedTray.FreeAmount.Should().Be(seedTray.TotalAmount - amount);
        seedTray.UsedAmount.Should().Be(0 + amount);

        mockSeedTrayRepository.VerifyAll();
    }

    [Theory]
    [InlineData(123, 3, 4)]
    [InlineData(321, 5, 8)]
    [InlineData(222, 1, 2)]
    [InlineData(307, 6, 6)]
    public void ReleaseArea_ShouldWork(int amount, int seedTrayType, int greenHouse)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

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

        mockSeedTrayRepository.VerifyAll();
        mockGreenHouseRepository.VerifyAll();
    }

    [Theory]
    [InlineData(76, 1, 7)]
    [InlineData(263, 3, 6)]
    [InlineData(111, 6, 5)]
    [InlineData(47, 5, 3)]
    public void ReserveArea_ShouldWork(int amount, int seedTrayType, int greenHouse)
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

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

        mockSeedTrayRepository.VerifyAll();
        mockGreenHouseRepository.VerifyAll();
    }

    [Fact]
    public void RemoveDeliveryDetails_ShouldRemoveAllDeliveriesOfTheDay()
    {
        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;
        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = MockOf.DeliveryDetailProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        status.IteratorDate = new DateOnly(2023, 4, 11);

        var deliveryDetailsToDelete = status.DeliveryDetails.Where(x => x.DeliveryDate == status.IteratorDate).ToList();

        status.DeliveryDetailsToDelete = new System.Collections.ArrayList(deliveryDetailsToDelete);

        int oldDeliveryDetailCount = status.DeliveryDetails.Count;

        MethodInfo methodInfo_RemoveDeliveryDetails = typeof(SeedBedStatus)
            .GetMethod("RemoveDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_RemoveDeliveryDetails.Invoke(status, null);

        status.DeliveryDetails.Count.Should().Be(oldDeliveryDetailCount - deliveryDetailsToDelete.Count);
        status.DeliveryDetails.Count.Should().Be(status.OrderLocations.Sum(x => x.DeliveryDetails.Count));
        mockOrderLocationProcessor.VerifyAll();
        mockDeliveryDetailProcessor.VerifyAll();
    }

    [Fact]
    public void RemoveOrderLocations_ShouldRemoveAllOrderLocationsWithoutDeliveryDetails()
    {
        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;
        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        var orderLocationsToDelete = status.OrderLocations.Where(x => x.SowDate == status.IteratorDate).ToList();

        status.OrderLocationsToDelete = new System.Collections.ArrayList(orderLocationsToDelete);

        int oldOrderLocationCount = status.OrderLocations.Count;

        MethodInfo methodInfo_RemoveOrderLocations = typeof(SeedBedStatus)
            .GetMethod("RemoveOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_RemoveOrderLocations.Invoke(status, null);

        status.OrderLocations.Count.Should().Be(oldOrderLocationCount - orderLocationsToDelete.Count);
        status.OrderLocations.Count.Should().Be(status.Orders.Sum(x => x.OrderLocations.Count));
        mockOrderLocationProcessor.VerifyAll();
        mockOrderProcessor.VerifyAll();
    }

    [Fact]
    public void RemoveOrder_ShouldRemoveAllOrdersWithoutOrderLocations()
    {
        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , orderProcessor: mockOrderProcessor.Object);

        var ordersToDelete = status.Orders.Where(x => x.RealSowDate == status.IteratorDate).ToList();

        status.OrdersToDelete = new System.Collections.ArrayList(ordersToDelete);

        int oldOrderCount = status.Orders.Count;

        MethodInfo methodInfo_RemoveOrders = typeof(SeedBedStatus)
            .GetMethod("RemoveOrders",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_RemoveOrders.Invoke(status, null);

        status.Orders.Count.Should().Be(oldOrderCount - ordersToDelete.Count);
        mockOrderProcessor.VerifyAll();
    }

    [Fact]
    public void AddOrderLocations_ShouldAddNewOrderLocationsToTheirOrders()
    {
        int orderAmount = 11;
        Mock<IOrderProcessor> mockOrderProcessor = MockOf.GetCustomOrderMock(orderAmount);

        int orderLocationAmount = 36;
        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.GetCustomOrderLocationMock(orderLocationAmount);

        SeedBedStatus status = new SeedBedStatus(_presentDate
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        int newOrderLocationAmount = 24;

        var newOrderLocations = RecordGenerator.GenerateOrderLocations(newOrderLocationAmount);

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(newOrderLocations);

        MethodInfo methodInfo_GetOrderLocations = typeof(SeedBedStatus)
            .GetMethod("GetOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        status.OrderLocationsToAdd = new System.Collections.ArrayList(
        (LinkedList<OrderLocationModel>)methodInfo_GetOrderLocations.Invoke(status, null));

        MethodInfo methodInfo_AddOrderLocations = typeof(SeedBedStatus)
            .GetMethod("AddOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_AddOrderLocations.Invoke(status, null);

        int sumOrderLocations = status.Orders.Sum(x => x.OrderLocations.Count);

        sumOrderLocations.Should().Be(orderLocationAmount + newOrderLocationAmount);

        status.OrderLocations.Count.Should().Be(orderLocationAmount + newOrderLocationAmount);

        mockOrderProcessor.VerifyAll();
        mockOrderLocationProcessor.VerifyAll();
    }

    [Fact]

    public void ImplementReservation_ShouldWork()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
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

        mockGreenHouseRepository.VerifyAll();
        mockSeedTrayRepository.VerifyAll();
        mockOrderProcessor.VerifyAll();
        mockOrderLocationProcessor.VerifyAll();
    }

    [Fact]
    public void ImplementRelease_ShouldWork()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = MockOf.DeliveryDetailProcessor;

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

        const int amountOfDays = 2;

        for (int i = 0; i < amountOfDays; i++)
        {
            methodInfo.Invoke(status, null);
            status.IteratorDate = status.IteratorDate.AddDays(1);
        }
        status.OrderLocationsToDelete.Count.Should().Be(2);
        status.OrdersToDelete.Count.Should().Be(1);

        mockGreenHouseRepository.VerifyAll();
        mockSeedTrayRepository.VerifyAll();
        mockOrderProcessor.VerifyAll();
        mockOrderLocationProcessor.VerifyAll();
        mockDeliveryDetailProcessor.VerifyAll();
    }

    [Fact]
    public void ImplementDelayRelease_ShouldWork()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , greenHouseRepo: mockGreenHouseRepository.Object
            , seedTrayRepo: mockSeedTrayRepository.Object
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object);

        status.IteratorDate = new DateOnly(2023, 4, 13);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("ImplementDelayRelease",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(status, null);

        status.OrderLocationsToDelete.Count.Should().Be(2);
        status.OrdersToDelete.Count.Should().Be(1);

        mockGreenHouseRepository.VerifyAll();
        mockSeedTrayRepository.VerifyAll();
        mockOrderProcessor.VerifyAll();
        mockOrderLocationProcessor.VerifyAll();
    }

    [Fact]
    public void DayByDayToCurrentDate_ShouldWork()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = MockOf.DeliveryDetailProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , greenHouseRepo: mockGreenHouseRepository.Object
            , seedTrayRepo: mockSeedTrayRepository.Object
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("DayByDayToCurrentDate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(status, null);

        status.OrderLocations.Where(x => x.RealDeliveryDate < _presentDate).Count().Should().Be(0);
        status.DeliveryDetails.Where(x => x.DeliveryDate < _presentDate).Count().Should().Be(0);
        status.OrdersToDelete.Count.Should().Be(0);
        status.OrderLocationsToDelete.Count.Should().Be(0);
        status.OrderLocationsToAdd.Count.Should().Be(0);
        status.IteratorDate.Should().Be(status.PresentDate);


        mockGreenHouseRepository.VerifyAll();
        mockSeedTrayRepository.VerifyAll();
        mockOrderProcessor.VerifyAll();
        mockOrderLocationProcessor.VerifyAll();
        mockDeliveryDetailProcessor.VerifyAll();
    }

    [Fact]
    public void CalculateTotalAvailableArea_ShouldGiveTheTotalArea()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        SeedBedStatus status = new SeedBedStatus(greenHouseRepo: mockGreenHouseRepository.Object);

        decimal availableArea = status.CalculateTotalAvailableArea();

        availableArea.Should().Be(
            status.GreenHouses
            .Where(x => x.Active == true)
            .Sum(x => x.SeedTrayAvailableArea)
            );
        mockGreenHouseRepository.VerifyAll();
    }

    [Fact]
    public void CalculateTotalAvailableArea_ShouldGiveTheFreeArea()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        SeedBedStatus status = new SeedBedStatus(greenHouseRepo: mockGreenHouseRepository.Object);

        decimal usedArea = 100;

        foreach (GreenHouseModel greenHouse in status.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea -= usedArea;
        }

        decimal availableArea = status.CalculateTotalAvailableArea();

        int amountOfActiveGreenHouses = status.GreenHouses.Count(x => x.Active == true);

        decimal totalArea = status.GreenHouses.Where(x => x.Active == true).Sum(x => x.SeedTrayTotalArea);

        availableArea.Should().Be(totalArea - (usedArea * amountOfActiveGreenHouses));
        mockGreenHouseRepository.VerifyAll();
    }

    [Fact]
    public void ThereAreNonNegattiveValuesOfSeedTray_ShouldReturnTrue()
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        SeedBedStatus status = new SeedBedStatus(seedTrayRepo: mockSeedTrayRepository.Object);

        bool result = status.ThereAreNonNegattiveValuesOfSeedTray();

        result.Should().Be(true);
        mockSeedTrayRepository.VerifyAll();
    }

    [Fact]
    public void ThereAreNonNegattiveValuesOfSeedTray_ShouldReturnFalse()
    {
        Mock<ISeedTrayRepository> mockSeedTrayRepository = MockOf.SeedTrayRepository;

        SeedBedStatus status = new SeedBedStatus(seedTrayRepo: mockSeedTrayRepository.Object);

        status.SeedTrays.First().FreeAmount = -1;

        bool result = status.ThereAreNonNegattiveValuesOfSeedTray();

        result.Should().Be(false);
        mockSeedTrayRepository.VerifyAll();
    }

    [Fact]
    public void ThereAreNonNegattiveValuesOfArea_ShouldReturnTrue()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        SeedBedStatus status = new SeedBedStatus(greenHouseRepo: mockGreenHouseRepository.Object);

        bool result = status.ThereAreNonNegattiveValuesOfArea();

        result.Should().Be(true);
        mockGreenHouseRepository.VerifyAll();
    }

    [Fact]
    public void ThereAreNonNegattiveValuesOfArea_ShouldReturnFalse()
    {
        Mock<IGreenHouseRepository> mockGreenHouseRepository = MockOf.GreenHouseRepository;

        SeedBedStatus status = new SeedBedStatus(greenHouseRepo: mockGreenHouseRepository.Object);

        status.GreenHouses.First(x => x.Active == true).SeedTrayAvailableArea = -100000;

        bool result = status.ThereAreNonNegattiveValuesOfArea();

        result.Should().Be(false);
        mockGreenHouseRepository.VerifyAll();
    }

    [Fact]
    public void UpdateObjects_ShouldWork()
    {
        Mock<IOrderProcessor> mockOrderProcessor = MockOf.OrderProcessor;

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = MockOf.OrderLocationProcessor;

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = MockOf.DeliveryDetailProcessor;

        SeedBedStatus status = new SeedBedStatus(presentDate: _presentDate
            , orderProcessor: mockOrderProcessor.Object
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        status.DeliveryDetailsToDelete.Add(status.DeliveryDetails.Last());
        status.OrderLocationsToDelete.Add(status.OrderLocations.Last());
        status.OrdersToDelete.Add(status.Orders.Last());
        status.OrderLocationsToAdd.Add(new OrderLocationModel(9999, 1, 108, 1, 1));

        int oldGeneralOrderLocationCount = status.OrderLocations.Count;
        int oldOrderLocationOfOrdersCount = status.Orders.Sum(x => x.OrderLocations.Count);

        status.UpdateObjects();

        status.DeliveryDetailsToDelete.Count.Should().Be(1);
        status.OrderLocationsToDelete.Count.Should().Be(1);
        status.OrdersToDelete.Count.Should().Be(1);
        oldGeneralOrderLocationCount.Should().Be(oldGeneralOrderLocationCount);
        oldOrderLocationOfOrdersCount.Should().Be(oldOrderLocationOfOrdersCount);
        mockOrderProcessor.VerifyAll();
        mockOrderLocationProcessor.VerifyAll();
        mockDeliveryDetailProcessor.VerifyAll();
    }
}
