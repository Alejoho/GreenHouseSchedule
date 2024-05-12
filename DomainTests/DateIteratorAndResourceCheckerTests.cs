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
    public void AreThereFreeSeedTraysOfTheTypesInUse_ShouldReturnTheCorrectResultWithASimplePermutation(
        int firstSeedtrayId, int firstAmount, bool result)
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("AreThereFreeSeedTraysOfTheTypesInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

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
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount
            , secondSeedtrayId, secondAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("AreThereFreeSeedTraysOfTheTypesInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

    [Theory]
    [InlineData(1, 500, 2, 1500, 3, 1800, true)]
    [InlineData(4, 1473, 5, 1718, 6, 709, true)]
    [InlineData(3, 2000, 7, 500, 5, 800, false)]
    [InlineData(2, 500, 5, 1800, 3, 1800, false)]
    [InlineData(7, 100, 6, 700, 5, 2000, false)]
    [InlineData(5, 2300, 3, 1950, 4, 2150, false)]
    public void AreThereFreeSeedTraysOfTheTypesInUse_ShouldReturnTheCorrectResultWithATriplePermutation(
        int firstSeedtrayId, int firstAmount, int secondSeedtrayId, int secondAmount
        , int thirdSeedtrayId, int thirdAmount, bool result)
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount
            , secondSeedtrayId, secondAmount, thirdSeedtrayId, thirdAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("AreThereFreeSeedTraysOfTheTypesInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

    [Theory]
    [InlineData(1, 8000, false)]
    [InlineData(1, 7000, true)]
    [InlineData(3, 11000, true)]
    [InlineData(6, 5000, true)]
    [InlineData(2, 9400, false)]
    [InlineData(7, 9500, true)]
    public void IsThereAreaForTheSeedTraysInUse_ShouldReturnTheCorrectResultWithASimplePermutation(
        int firstSeedtrayId, int firstAmount, bool result)
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("IsThereAreaForTheSeedTraysInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

    [Theory]
    [InlineData(1, 5000, 2, 7000, false)]
    [InlineData(3, 13000, 4, 5525, false)]
    [InlineData(5, 7000, 6, 6000, false)]
    [InlineData(2, 2850, 7, 3000, true)]
    [InlineData(4, 4000, 1, 3000, true)]
    public void IsThereAreaForTheSeedTraysInUse_ShouldReturnTheCorrectResultWithADoublePermutation(
        int firstSeedtrayId, int firstAmount, int secondSeedtrayId, int secondAmount, bool result)
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount
            , secondSeedtrayId, secondAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("IsThereAreaForTheSeedTraysInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

    [Theory]
    [InlineData(1, 1000, 2, 700, 3, 500, true)]
    [InlineData(4, 1500, 5, 7000, 6, 5000, false)]
    public void IsThereAreaForTheSeedTraysInUse_ShouldReturnTheCorrectResultWithATriplePermutation(
        int firstSeedtrayId, int firstAmount, int secondSeedtrayId, int secondAmount
        , int thirdSeedtrayId, int thirdAmount, bool result)
    {
        SeedTrayPermutation permutation = new SeedTrayPermutation(firstSeedtrayId, firstAmount
            , secondSeedtrayId, secondAmount, thirdSeedtrayId, thirdAmount);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, null, true);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("IsThereAreaForTheSeedTraysInUse"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool output = (bool)methodInfo.Invoke(iterator, new object[] { permutation });

        output.Should().Be(result);
    }

    [Theory]
    [InlineData(200000, 5)]
    [InlineData(100000, 7)]
    [InlineData(800000, 0)]
    [InlineData(300000, 0)]
    [InlineData(320000, 0)]
    [InlineData(250000, 2)]
    [InlineData(50000, 7)]
    [InlineData(5000, 7)]
    [InlineData(1000, 7)]
    public void GenerateAndAddSimplePermutations_ShouldCreateTheCorrectAmountOfPermutations(int seedlingAmount, int permutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("GenerateAndAddSimplePermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Count.Should().Be(permutationAmount);
    }

    [Theory]
    [InlineData(1000, 0)]
    [InlineData(100000, 0)]
    [InlineData(900000, 0)]
    [InlineData(163000, 6)]
    [InlineData(318000, 7)]
    [InlineData(350000, 6)]
    [InlineData(280000, 19)]
    [InlineData(220000, 12)]
    [InlineData(244000, 18)]
    [InlineData(355000, 6)]
    public void GenerateAndAddDoublePermutations_ShouldCreateTheCorrectAmountOfPermutations(int seedlingAmount, int permutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("GenerateAndAddDoublePermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Count.Should().Be(permutationAmount);
    }

    [Theory]
    [InlineData(162000, 0)]
    [InlineData(200000, 0)]
    [InlineData(300000, 0)]
    [InlineData(360000, 0)]
    [InlineData(370000, 10)]
    [InlineData(380000, 10)]
    [InlineData(390000, 10)]
    [InlineData(400000, 10)]
    [InlineData(450000, 6)]
    [InlineData(453000, 4)]
    [InlineData(459000, 2)]
    [InlineData(460000, 0)]
    public void GenerateAndAddTriplePermutations_ShouldCreateTheCorrectAmountOfPermutations(int seedlingAmount, int permutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("GenerateAndAddTriplePermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Count.Should().Be(permutationAmount);
    }

    [Theory]
    [InlineData(5000, 7)]
    [InlineData(162000, 7)]
    [InlineData(39000, 7)]
    [InlineData(500000, 0)]
    public void SeedTrayAndAreaResources_ShouldGiveOnlySimplePermutations(
        int seedlingAmount, int permutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("SeedTrayAndAreaResources"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Where(x => x.FirstSeedTrayID > 0 && x.SecondSeedTrayID == 0 && x.ThirdSeedTrayID == 0)
            .Count().Should().Be(permutationAmount);
        permutations.Count().Should().Be(permutationAmount);
    }

    [Theory]
    [InlineData(280000, 19)]
    [InlineData(300000, 10)]
    [InlineData(360000, 5)]
    [InlineData(600000, 0)]
    public void SeedTrayAndAreaResources_ShouldGiveOnlyDoublePermutations(
        int seedlingAmount, int permutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("SeedTrayAndAreaResources"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Where(x => x.FirstSeedTrayID > 0 && x.SecondSeedTrayID > 0 && x.ThirdSeedTrayID == 0)
            .Count().Should().Be(permutationAmount);
        permutations.Count.Should().Be(permutationAmount);
    }

    [Theory]
    [InlineData(400000, 10)]
    [InlineData(430000, 8)]
    [InlineData(440000, 6)]
    [InlineData(455000, 4)]
    public void SeedTrayAndAreaResources_ShouldGiveOnlyTriplePermutations(
        int seedlingAmount, int permutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("SeedTrayAndAreaResources"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Where(x => x.FirstSeedTrayID > 0 && x.SecondSeedTrayID > 0 && x.ThirdSeedTrayID > 0)
    .Count().Should().Be(permutationAmount);
        permutations.Count.Should().Be(permutationAmount);
    }

    [Theory]
    [InlineData(10000, 7, 0, 0)]
    [InlineData(15000, 7, 0, 0)]
    [InlineData(20000, 5, 12, 0)]
    [InlineData(25000, 4, 18, 0)]
    [InlineData(30000, 2, 30, 0)]
    [InlineData(50000, 1, 20, 80)]
    [InlineData(65000, 0, 12, 120)]
    [InlineData(70000, 0, 12, 102)]
    [InlineData(75000, 0, 12, 86)]
    [InlineData(80000, 0, 10, 88)]
    [InlineData(85000, 0, 8, 87)]
    [InlineData(90000, 0, 2, 102)]
    [InlineData(95000, 0, 0, 94)]
    [InlineData(255000, 0, 0, 0)]
    public void SeedTrayAndAreaResources_ShouldGiveMixedPermutations(int seedlingAmount
        , int simplePermutationAmount
        , int doublePermutationAmount
        , int triplePermutationAmount)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var seedTray in iterator.SeedBedStatus.SeedTrays)
        {
            seedTray.FreeAmount = Convert.ToInt32(seedTray.FreeAmount * 0.1);
        }

        iterator.SeedBedStatus.SeedTrays.Last().FreeAmount = 500;

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("SeedTrayAndAreaResources"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Where(x => x.FirstSeedTrayID > 0 && x.SecondSeedTrayID == 0 && x.ThirdSeedTrayID == 0)
            .Count().Should().Be(simplePermutationAmount);
        permutations.Where(x => x.FirstSeedTrayID > 0 && x.SecondSeedTrayID > 0 && x.ThirdSeedTrayID == 0)
            .Count().Should().Be(doublePermutationAmount);
        permutations.Where(x => x.FirstSeedTrayID > 0 && x.SecondSeedTrayID > 0 && x.ThirdSeedTrayID > 0)
            .Count().Should().Be(triplePermutationAmount);
        permutations.Count.Should().Be(simplePermutationAmount + doublePermutationAmount + triplePermutationAmount);
    }

    [Fact]
    public void SeedTrayAndAreaResources_ShouldGiveNonePermutations()
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , 163000
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        foreach (var greenHouse in iterator.SeedBedStatus.GreenHouses)
        {
            greenHouse.SeedTrayAvailableArea *= 0.1640m;
        }

        iterator.SeedBedStatus.SeedTrays.RemoveAll(x => x.ID != 1);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("SeedTrayAndAreaResources"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker)
            .GetField("_seedTrayPermutations"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        var permutations = (LinkedList<SeedTrayPermutation>)fieldInfo.GetValue(iterator);

        permutations.Count().Should().Be(0);
    }

    [Theory]
    [InlineData(75000, 1, 268, 0, 0, 0, 0, 1)]
    [InlineData(75000, 1, 179, 2, 174, 0, 0, 2)]
    [InlineData(75000, 1, 90, 2, 174, 4, 116, 3)]
    public void InsertOrderInProcessIntoSeedBedStatusAuxiliar_ShouldInsertAnOrderAndItsOrderLocation(
        int seedlingAmount
        , int seedTrayId1, int amount1
        , int seedTrayId2, int amount2
        , int seedTrayId3, int amount3
        , int newOrderLocations)
    {
        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly()
            , new DateOnly()
            , null
            , null
            , null
            , false);

        SeedTrayPermutation permutation = new SeedTrayPermutation(
            seedTrayId1, amount1
            , seedTrayId2, amount2
            , seedTrayId3, amount3);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        int oldOrderCount = iterator.SeedBedStatus.Orders.Count;

        int oldOrderLocationsCount = iterator.SeedBedStatus.OrderLocations.Count;

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("InsertOrderInProcessIntoSeedBedStatus"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, new object[] { permutation });

        iterator.SeedBedStatus.Orders.Count.Should().Be(oldOrderCount + 1);
        iterator.SeedBedStatus.OrderLocations.Count.Should().Be(oldOrderLocationsCount + newOrderLocations);
    }

    [Theory]
    [InlineData(75000, 1, 268, 0, 0, 0, 0)]
    [InlineData(129228, 7, 1068, 0, 0, 0, 0)]
    [InlineData(205056, 4, 496, 2, 680, 0, 0)]
    [InlineData(238733, 5, 1210, 7, 533, 0, 0)]
    [InlineData(336600, 2, 950, 4, 625, 5, 450)]
    [InlineData(89700, 7, 100, 3, 150, 1, 200)]
    [InlineData(154250, 6, 250, 4, 250, 7, 250)]
    [InlineData(173728, 4, 300, 2, 282, 6, 244)]
    public void DoesItDisplaceFollowingOrders_ShouldReturnTrueAndNonePermutationBecauseOfLackOfSeedTrays(
        int seedlingAmount
        , int seedTrayId1, int amount1
        , int seedTrayId2, int amount2
        , int seedTrayId3, int amount3)
    {
        DateOnly wishedDate = new DateOnly(2023, 6, 13);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly(2023, 6, 10)
            , wishedDate
            , null
            , null
            , null
            , false);

        SeedTrayPermutation permutation = new SeedTrayPermutation(
            seedTrayId1, amount1
            , seedTrayId2, amount2
            , seedTrayId3, amount3);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        iterator.SeedTrayPermutations.AddLast(permutation);

        GreenHouseModel tempGreenHouse = new GreenHouseModel(-1, "TempGreenHouse", 0, 0, true);
        iterator.SeedBedStatus.GreenHouses.Add(tempGreenHouse);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        MethodInfo methodInfo2 = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DoesItDisplaceFollowingOrders"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo2.Invoke(iterator, null);

        result.Should().BeTrue();
        iterator.SeedTrayPermutations.Count.Should().Be(0);
        iterator.SeedBedStatus.IteratorDate.Should().BeBefore(wishedDate.AddDays(30));
        iterator.SeedBedStatus.ThereAreNonNegattiveValuesOfSeedTray().Should().BeFalse();
        iterator.SeedBedStatus.ThereAreNonNegattiveValuesOfArea().Should().BeTrue();
    }

    [Theory]
    [InlineData(129228, 7, 1068, 0, 0, 0, 0)]
    [InlineData(157936, 7, 496, 2, 680, 0, 0)]
    [InlineData(238733, 5, 610, 7, 533, 0, 0)]
    [InlineData(336600, 2, 950, 4, 625, 5, 450)]
    [InlineData(70170, 6, 50, 4, 120, 7, 250)]
    public void DoesItDisplaceFollowingOrders_ShouldReturnTrueAndNonePermutationBecauseOfLackOfArea(
        int seedlingAmount
        , int seedTrayId1, int amount1
        , int seedTrayId2, int amount2
        , int seedTrayId3, int amount3)
    {
        DateOnly wishedDate = new DateOnly(2023, 6, 13);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly(2023, 6, 10)
            , wishedDate
            , null
            , null
            , null
            , false);

        SeedTrayPermutation permutation = new SeedTrayPermutation(
            seedTrayId1, amount1
            , seedTrayId2, amount2
            , seedTrayId3, amount3);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        iterator.SeedTrayPermutations.AddLast(permutation);

        GreenHouseModel tempGreenHouse = new GreenHouseModel(-1, "TempGreenHouse", 0, 0, true);
        iterator.SeedBedStatus.GreenHouses.Add(tempGreenHouse);

        SeedTrayModel seedtray = iterator.SeedBedStatus.SeedTrays.First(x => x.ID == 7);

        iterator.SeedBedStatus.SeedTrays.Remove(seedtray);

        iterator.SeedBedStatus.SeedTrays.Add(new SeedTrayModel(7, "121 alveolos", 2.2m, 121, 2550, true));

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        MethodInfo methodInfo2 = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DoesItDisplaceFollowingOrders"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo2.Invoke(iterator, null);

        result.Should().BeTrue();
        iterator.SeedTrayPermutations.Count.Should().Be(0);
        iterator.SeedBedStatus.IteratorDate.Should().BeBefore(wishedDate.AddDays(30));
        iterator.SeedBedStatus.ThereAreNonNegattiveValuesOfSeedTray().Should().BeTrue();
        iterator.SeedBedStatus.ThereAreNonNegattiveValuesOfArea().Should().BeFalse();
    }

    [Theory]
    [InlineData(3600000, 3, 25000, 0, 0, 0, 0)]
    [InlineData(7200000, 5, 17000, 2, 33000, 0, 0)]
    [InlineData(8288000, 1, 8000, 3, 19000, 5, 23000)]
    [InlineData(11953000, 3, 50000, 2, 12000, 7, 25000)]
    public void DoesItDisplaceFollowingOrders_ShouldReturnTrueAndNonePermutationBecauseOfLackOfWorkForceAndTime(
        int seedlingAmount
        , int seedTrayId1, int amount1
        , int seedTrayId2, int amount2
        , int seedTrayId3, int amount3)
    {
        DateOnly wishedDate = new DateOnly(2023, 6, 13);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly(2023, 6, 10)
            , wishedDate
            , null
            , null
            , null
            , false);

        SeedTrayPermutation permutation = new SeedTrayPermutation(
            seedTrayId1, amount1
            , seedTrayId2, amount2
            , seedTrayId3, amount3);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        iterator.SeedTrayPermutations.AddLast(permutation);

        GreenHouseModel tempGreenHouse = new GreenHouseModel(-1, "TempGreenHouse", 0, 0, true);
        iterator.SeedBedStatus.GreenHouses.Add(tempGreenHouse);

        iterator.SeedBedStatus.SeedTrays.ForEach(x => x.FreeAmount *= 100);
        iterator.SeedBedStatus.GreenHouses.ForEach(x => x.SeedTrayAvailableArea *= 100);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        MethodInfo methodInfo2 = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DoesItDisplaceFollowingOrders"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo2.Invoke(iterator, null);

        result.Should().BeTrue();
        iterator.SeedTrayPermutations.Count.Should().Be(0);
        iterator.SeedBedStatus.IteratorDate.Should().Be(wishedDate.AddDays(30));
        iterator.SeedBedStatus.Orders.Last.Value.Complete.Should().BeFalse();
        iterator.SeedBedStatus.ThereAreNonNegattiveValuesOfSeedTray().Should().BeTrue();
        iterator.SeedBedStatus.ThereAreNonNegattiveValuesOfArea().Should().BeTrue();
    }

    [Theory]
    [InlineData(14000, 1, 50, 0, 0, 0, 0)]
    [InlineData(15120, 5, 105, 0, 0, 0, 0)]
    [InlineData(40880, 6, 110, 3, 70, 0, 0)]
    [InlineData(32550, 7, 150, 2, 100, 0, 0)]
    [InlineData(52320, 1, 50, 3, 130, 6, 70)]
    [InlineData(44320, 3, 120, 5, 110, 1, 40)]
    public void DoesItDisplaceFollowingOrders_ShouldReturnFalseAndOnePermutation(
        int seedlingAmount
        , int seedTrayId1, int amount1
        , int seedTrayId2, int amount2
        , int seedTrayId3, int amount3)
    {
        DateOnly wishedDate = new DateOnly(2023, 6, 13);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , seedlingAmount
            , new DateOnly(2023, 6, 10)
            , wishedDate
            , null
            , null
            , null
            , false);

        SeedTrayPermutation permutation = new SeedTrayPermutation(
            seedTrayId1, amount1
            , seedTrayId2, amount2
            , seedTrayId3, amount3);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        iterator.SeedTrayPermutations.AddLast(permutation);

        GreenHouseModel tempGreenHouse = new GreenHouseModel(-1, "TempGreenHouse", 0, 0, true);
        iterator.SeedBedStatus.GreenHouses.Add(tempGreenHouse);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        MethodInfo methodInfo2 = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DoesItDisplaceFollowingOrders"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo2.Invoke(iterator, null);

        result.Should().BeFalse();
        iterator.SeedTrayPermutations.Count.Should().Be(1);
    }

    [Fact]
    public void DoesItDisplaceFollowingOrders_ShouldWorkProperly()
    {
        DateOnly wishedDate = new DateOnly(2023, 6, 13);

        OrderModel newOrder = new OrderModel(1
            , new ClientModel(1, "", "")
            , new ProductModel(1, "", "", 30)
            , 55000
            , new DateOnly(2023, 6, 10)
            , wishedDate
            , null
            , null
            , null
            , false);

        DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(_status, newOrder, true);

        SeedTrayPermutation permutation = new SeedTrayPermutation(4, 255);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(7, 455);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(3, 144, 5, 132);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(7, 200, 4, 143);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(2, 75, 4, 100, 7, 187);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(1, 20, 3, 194, 5, 150);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(1, 197);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(1, 100, 6, 97);
        iterator.SeedTrayPermutations.AddLast(permutation);

        permutation = new SeedTrayPermutation(2, 10, 3, 36, 6, 155);
        iterator.SeedTrayPermutations.AddLast(permutation);

        GreenHouseModel tempGreenHouse = new GreenHouseModel(-1, "TempGreenHouse", 0, 0, true);
        iterator.SeedBedStatus.GreenHouses.Add(tempGreenHouse);

        MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DayByDayToRequestDate"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(iterator, null);

        MethodInfo methodInfo2 = typeof(DateIteratorAndResourceChecker)
            .GetMethod("DoesItDisplaceFollowingOrders"
                , BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo2.Invoke(iterator, null);

        result.Should().BeFalse();
        iterator.SeedTrayPermutations.Count.Should().Be(6);
    }

    #endregion
}
