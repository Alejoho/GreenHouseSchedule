using Domain;
using Domain.Models;
using Domain.ValuableObjects;
using FluentAssertions;
using System.Reflection;

namespace DomainTests;

public class DateIteratorAndResourceCheckerTests
{
    private static DateOnly _presentDate = new DateOnly(2023, 6, 10);
    private static DateOnly _pastDate = _presentDate.AddDays(-90);
    private static SeedBedStatus _status;
    private static RecordGenerator _generator;
    private static MockOf _mockOf;

    public DateIteratorAndResourceCheckerTests()
    {
        if (_generator == null)
        {
            _generator = new RecordGenerator(150, _pastDate);

            _mockOf = new MockOf(_generator, _pastDate);

            _status = new SeedBedStatus(_presentDate
                , _mockOf.GreenHouseRepository.Object
                , _mockOf.SeedTrayRepository.Object
                , _mockOf.OrderProcessor.Object
                , _mockOf.OrderLocationProcessor.Object
                , _mockOf.DeliveryDetailProcessor.Object
                , true);
        }
    }

    #region Tests for the iteration to the request date
    [Fact]
    public void CloneSeedBedStatusObjects_ShouldBeDisconnectFromEachOther()
    {
        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker).GetField("_auxiliarSeedBedStatus"
            , BindingFlags.NonPublic | BindingFlags.Instance);

        SeedBedStatus auxiliarStatusOfTheIterator = (SeedBedStatus)fieldInfo.GetValue(iterator);

        auxiliarStatusOfTheIterator = new SeedBedStatus(iterator.SeedBedStatus);

        auxiliarStatusOfTheIterator.IteratorDate = new DateOnly();
        auxiliarStatusOfTheIterator.PresentDate = new DateOnly();
        auxiliarStatusOfTheIterator.GreenHouses = null;
        auxiliarStatusOfTheIterator.SeedTrays = null;
        auxiliarStatusOfTheIterator.RemainingAmountOfSeedTrayToSowPerDay = -1;
        auxiliarStatusOfTheIterator.Orders = null;
        auxiliarStatusOfTheIterator.OrderLocations = null;
        auxiliarStatusOfTheIterator.DeliveryDetails = null;
        auxiliarStatusOfTheIterator.OrdersToDelete = null;
        auxiliarStatusOfTheIterator.OrderLocationsToDelete = null;
        auxiliarStatusOfTheIterator.DeliveryDetailsToDelete = null;
        auxiliarStatusOfTheIterator.OrderLocationsToAdd = null;

        iterator.SeedBedStatus.IteratorDate.Should().BeAfter(auxiliarStatusOfTheIterator.IteratorDate);
        iterator.SeedBedStatus.PresentDate.Should().BeAfter(auxiliarStatusOfTheIterator.PresentDate);
        iterator.SeedBedStatus.GreenHouses.Should().NotBeNull();
        iterator.SeedBedStatus.SeedTrays.Should().NotBeNull();
        iterator.SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay.Should().NotBe(-1);
        iterator.SeedBedStatus.Orders.Should().NotBeNull();
        iterator.SeedBedStatus.OrderLocations.Should().NotBeNull();
        iterator.SeedBedStatus.DeliveryDetails.Should().NotBeNull();
        iterator.SeedBedStatus.OrdersToDelete.Should().NotBeNull();
        iterator.SeedBedStatus.OrderLocationsToDelete.Should().NotBeNull();
        iterator.SeedBedStatus.DeliveryDetailsToDelete.Should().NotBeNull();
        iterator.SeedBedStatus.OrderLocationsToAdd.Should().NotBeNull();
    }

    [Fact]
    public void CloneSeedBedStatusObjects_ShouldGiveAnIdenticalSeedBedStatus()
    {
        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker).GetField("_auxiliarSeedBedStatus"
            , BindingFlags.NonPublic | BindingFlags.Instance);

        SeedBedStatus auxStatusOfTheIterator = (SeedBedStatus)fieldInfo.GetValue(iterator);

        auxStatusOfTheIterator = new SeedBedStatus(iterator.SeedBedStatus);

        iterator.SeedBedStatus.IteratorDate.Should().Be(auxStatusOfTheIterator.IteratorDate);
        iterator.SeedBedStatus.PresentDate.Should().Be(auxStatusOfTheIterator.PresentDate);
        iterator.SeedBedStatus.GreenHouses.Count.Should().Be(auxStatusOfTheIterator.GreenHouses.Count);
        iterator.SeedBedStatus.SeedTrays.Count.Should().Be(auxStatusOfTheIterator.SeedTrays.Count);
        iterator.SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay.Should()
            .Be(auxStatusOfTheIterator.RemainingAmountOfSeedTrayToSowPerDay);
        iterator.SeedBedStatus.Orders.Count.Should()
            .Be(auxStatusOfTheIterator.Orders.Count);
        iterator.SeedBedStatus.OrderLocations.Count.Should()
            .Be(auxStatusOfTheIterator.OrderLocations.Count);
        iterator.SeedBedStatus.DeliveryDetails.Count.Should()
            .Be(auxStatusOfTheIterator.DeliveryDetails.Count);
        iterator.SeedBedStatus.OrdersToDelete.Count.Should()
            .Be(auxStatusOfTheIterator.OrdersToDelete.Count);
        iterator.SeedBedStatus.OrderLocationsToDelete.Count.Should()
            .Be(auxStatusOfTheIterator.OrderLocationsToDelete.Count);
        iterator.SeedBedStatus.DeliveryDetailsToDelete.Count.Should()
            .Be(auxStatusOfTheIterator.DeliveryDetailsToDelete.Count);
        iterator.SeedBedStatus.OrderLocationsToAdd.Count.Should()
            .Be(auxStatusOfTheIterator.OrderLocationsToAdd.Count);
        iterator.SeedBedStatus.MaxAmountOfSeedTrayToSowPerDay.Should()
            .Be(auxStatusOfTheIterator.MaxAmountOfSeedTrayToSowPerDay);
        iterator.SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay.Should()
            .Be(auxStatusOfTheIterator.RemainingAmountOfSeedTrayToSowPerDay);
        iterator.SeedBedStatus.MinimumLimitOfSeedTrayToSow.Should()
            .Be(auxStatusOfTheIterator.MinimumLimitOfSeedTrayToSow);
    }

    [Fact]
    public void RestartPotentialOfSowSeedTrayPerDay_ShouldResetTheVariable()
    {
        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status);
        iterator.SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay = 100;

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("RestartPotentialOfSowSeedTrayPerDay"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        iterator.SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay.Should().Be(500);
    }

    [Fact]
    public void ImplementEstimateRelease_ShouldAddOrdersAndOrderLocationsToTheArrayListsToDelete()
    {
        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status);

        int countOfOrderLocationsToDelete = iterator.SeedBedStatus.OrderLocations
            .Where(x => x.EstimateDeliveryDate == iterator.SeedBedStatus.IteratorDate).Count();

        int countOfOrdersToDelete = iterator.SeedBedStatus.Orders
            .Where(x => x.OrderLocations.Count() > 0
            && x.OrderLocations.All(y => y.EstimateDeliveryDate == iterator.SeedBedStatus.IteratorDate))
            .Count();

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("ImplementEstimateRelease"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        iterator.SeedBedStatus.OrderLocationsToDelete.Count.Should().Be(countOfOrderLocationsToDelete);
        iterator.SeedBedStatus.OrdersToDelete.Count.Should().Be(countOfOrdersToDelete);
    }

    [Fact]
    public void ImplementEstimateReservation_ShouldWorkWhenTheLimitOfSowPerDayIsNotReached()
    {
        //NEXT - Make the assert of this test
        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status);

        var ordersToSow = iterator.SeedBedStatus.Orders
            .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                && order.Complete == false);

        foreach (var order in ordersToSow)
        {
            foreach (var orderLocation in order.OrderLocations)
            {
                orderLocation.SeedTrayAmount = 120;
            }
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("ImplementEstimateReservation"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        int amountOfOrdersToSow = iterator.SeedBedStatus.Orders
            .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                && order.Complete == false).Count();

        amountOfOrdersToSow.Should().Be(6);
        iterator.SeedBedStatus.OrderLocationsToAdd.Count.Should().Be(0);
    }

    [Fact]
    public void ImplementEstimateReservation_ShouldWorkWhenTheLimitOfSowPerDayIsReached()
    {
        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status);

        var ordersToSow = iterator.SeedBedStatus.Orders
            .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                && order.Complete == false).ToList();

        ordersToSow[0].OrderLocations.First(x => x.Sown == false).SeedTrayAmount = 650;

        int oldOrderLocationsToSowCount = ordersToSow
            .Sum(x => x.OrderLocations.Count);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("ImplementEstimateReservation"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        int amountOfOrdersToSow = iterator.SeedBedStatus.Orders
            .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                && order.Complete == false).Count();

        int newOrderLocationsToSowCount = ordersToSow
            .Sum(x => x.OrderLocations.Count);

        newOrderLocationsToSowCount.Should().Be(oldOrderLocationsToSowCount);
        iterator.SeedBedStatus.OrderLocationsToAdd.Count.Should().Be(1);
    }

    [Fact]
    public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_7_18_ThatHadOnlyCompleteOrders()
    {
        SeedBedStatus localStatus = new SeedBedStatus(_presentDate
                , _mockOf.GreenHouseRepository.Object
                , _mockOf.SeedTrayRepository.Object
                , _mockOf.GetOrderMockByRecordType(TypeOfRecord.complete, _presentDate).Object
                , _mockOf.GetOrderLocationMockByRecordType(TypeOfRecord.complete, _presentDate).Object
                , _mockOf.GetDeliveryDetailMockByRecordType(TypeOfRecord.complete, _presentDate).Object
                , true);

        DateOnly wishedDate = new DateOnly(2023, 7, 18);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , 1234
            , new DateOnly()
            , wishedDate
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(localStatus, newOrder, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        iterator.SeedBedStatus.IteratorDate.Should().Be(wishedDate);

        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.UsedAmount.Should().Be(0));
        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.FreeAmount.Should().Be(x.TotalAmount));

        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayUsedArea.Should()
                .BeApproximately(0m, 0.001m));
        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayAvailableArea.Should()
                .BeApproximately(x.SeedTrayTotalArea, 0.001m));

        iterator.SeedBedStatus.Orders.Count.Should().Be(0);
        iterator.SeedBedStatus.OrderLocations.Count.Should().Be(0);
        iterator.SeedBedStatus.DeliveryDetails.Count.Should().Be(0);
    }        

    [Fact]
    public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_8_18_ThatHadOnlyPartialOrders()
    {
        SeedBedStatus localStatus = new SeedBedStatus(_presentDate
                , _mockOf.GreenHouseRepository.Object
                , _mockOf.SeedTrayRepository.Object
                , _mockOf.GetOrderMockByRecordType(TypeOfRecord.partial, _presentDate).Object
                , _mockOf.GetOrderLocationMockByRecordType(TypeOfRecord.partial, _presentDate).Object
                , _mockOf.GetDeliveryDetailMockByRecordType(TypeOfRecord.partial, _presentDate).Object
                , true);

        DateOnly wishedDate = new DateOnly(2023, 8, 18);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , 1234
            , new DateOnly()
            , wishedDate
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(localStatus, newOrder, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        iterator.SeedBedStatus.IteratorDate.Should().Be(wishedDate);

        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.UsedAmount.Should().Be(0));
        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.FreeAmount.Should().Be(x.TotalAmount));

        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayUsedArea.Should()
                .BeApproximately(0m, 0.001m));
        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayAvailableArea.Should()
                .BeApproximately(x.SeedTrayTotalArea, 0.001m));

        iterator.SeedBedStatus.Orders.Count.Should().Be(0);
        iterator.SeedBedStatus.OrderLocations.Count.Should().Be(0);
        iterator.SeedBedStatus.DeliveryDetails.Count.Should().Be(0);
    }

    [Fact]
    public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_6_10_2024_ThatHadOnlyEmptyOrders()
    {
        SeedBedStatus localStatus = new SeedBedStatus(_presentDate
                , _mockOf.GreenHouseRepository.Object
                , _mockOf.SeedTrayRepository.Object
                , _mockOf.GetOrderMockByRecordType(TypeOfRecord.empty, _presentDate).Object
                , _mockOf.GetOrderLocationMockByRecordType(TypeOfRecord.empty, _presentDate).Object
                , _mockOf.GetDeliveryDetailMockByRecordType(TypeOfRecord.empty, _presentDate).Object
                , true);

        DateOnly wishedDate = new DateOnly(2024, 6, 10);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , 1234
            , new DateOnly()
            , wishedDate
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(localStatus, newOrder, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        iterator.SeedBedStatus.IteratorDate.Should().Be(wishedDate);

        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.UsedAmount.Should().Be(0));
        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.FreeAmount.Should().Be(x.TotalAmount));

        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayUsedArea.Should()
                .BeApproximately(0m, 0.001m));
        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayAvailableArea.Should()
                .BeApproximately(x.SeedTrayTotalArea, 0.001m));

        iterator.SeedBedStatus.Orders.Count.Should().Be(0);
        iterator.SeedBedStatus.OrderLocations.Count.Should().Be(0);
        iterator.SeedBedStatus.DeliveryDetails.Count.Should().Be(0);
    }

    [Fact]
    public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_6_10_2024_ThatHadAllTypeOfOrders()
    {
        DateOnly wishedDate = new DateOnly(2024, 6, 10);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , 1234
            , new DateOnly()
            , wishedDate
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        iterator.SeedBedStatus.IteratorDate.Should().Be(wishedDate);

        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.UsedAmount.Should().Be(0));
        iterator.SeedBedStatus.SeedTrays
            .ForEach(x => x.FreeAmount.Should().Be(x.TotalAmount));

        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayUsedArea.Should()
                .BeApproximately(0m, 0.001m));
        iterator.SeedBedStatus.GreenHouses
            .ForEach(x => x.SeedTrayAvailableArea.Should()
                .BeApproximately(x.SeedTrayTotalArea, 0.001m));

        iterator.SeedBedStatus.Orders.Count.Should().Be(0);
        iterator.SeedBedStatus.OrderLocations.Count.Should().Be(0);
        iterator.SeedBedStatus.DeliveryDetails.Count.Should().Be(0);
    }

    #endregion

    #region Tests for the resource checker

    [Theory]
    [InlineData(1, 300, true)]
    [InlineData(2, 2000, false)]
    [InlineData(3, 1500, true)]
    [InlineData(4, 1480, false)]
    [InlineData(5, 930, true)]
    [InlineData(6, 500, true)]
    [InlineData(7, 1909, false)]
    public void AreThereFreeSeedTraysOfTheTypesInUse_ShouldReturnTrueWithASimplePermutation(int seedtrayId, int amount, bool result)
    [Theory]
    [InlineData(3, 800, 5, 1500, true)]
    [InlineData(2, 2000, 1, 150, false)]
    [InlineData(1, 82, 6, 900, false)]
    [InlineData(3, 1900, 4, 1473, true)]
    [InlineData(2, 1769, 5, 1719, false)]
    [InlineData(6, 700, 4, 1400, true)]
    [InlineData(3, 1, 7, 1, true)]
    public void AreThereFreeSeedTraysOfTheTypesInUse_ShouldReturnTheCorrectResultWithADoublePermutation(
        int firstSeedtrayId, int firstAmount, int secondSeedtrayId, int secondAmount, bool result)
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount, secondSeedtrayId, secondAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("AreThereFreeSeedTraysOfTheTypesInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(seedtrayId, amount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("AreThereFreeSeedTraysOfTheTypesInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

    #endregion
}
