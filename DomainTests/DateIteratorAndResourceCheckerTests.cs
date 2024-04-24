using Domain;
using Domain.Models;
using FluentAssertions;
using System.Reflection;

namespace DomainTests
{
    public class DateIteratorAndResourceCheckerTests
    {
        private static DateOnly _presentDate = new DateOnly(2023, 6, 10);
        private static DateOnly _pastDate = _presentDate.AddDays(-90);
        private static SeedBedStatus status;
        private static RecordGenerator _generator;
        private static MockOf _mockOf;

        public DateIteratorAndResourceCheckerTests()
        {
            if (_generator == null)
            {
                _generator = new RecordGenerator(600, _pastDate);

                _mockOf = new MockOf(_generator, _pastDate);

                status = new SeedBedStatus(_presentDate
                    , _mockOf.GreenHouseRepository.Object
                    , _mockOf.SeedTrayRepository.Object
                    , _mockOf.OrderProcessor.Object
                    , _mockOf.OrderLocationProcessor.Object
                    , _mockOf.DeliveryDetailProcessor.Object
                    , true);
            }
        }

        //TODO - Maybe make a test to verify that the clone of the SeeBedStatus was made correctly.
        [Fact]
        public void CloneSeedBedStatusObjects_ShouldBeDisconnectFromEachOther()
        {
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);

            FieldInfo fieldInfo = typeof(DateIteratorAndResourceChecker).GetField("_seedBedStatusAuxiliar"
                , BindingFlags.NonPublic | BindingFlags.Instance);

            SeedBedStatus statusOfTheIterator = (SeedBedStatus)fieldInfo.GetValue(iterator);

            statusOfTheIterator.IteratorDate = new DateOnly();
            statusOfTheIterator.PresentDate = new DateOnly();
            statusOfTheIterator.GreenHouses = null;
            statusOfTheIterator.SeedTrays = null;
            statusOfTheIterator.RemainingAmountOfSeedTrayToSowPerDay = -1;
            statusOfTheIterator.Orders = null;
            statusOfTheIterator.OrderLocations = null;
            statusOfTheIterator.DeliveryDetails = null;
            statusOfTheIterator.OrdersToDelete = null;
            statusOfTheIterator.OrderLocationsToDelete = null;
            statusOfTheIterator.DeliveryDetailsToDelete = null;
            statusOfTheIterator.OrderLocationsToAdd = null;

            iterator.SeedBedStatus.IteratorDate.Should().BeAfter(statusOfTheIterator.IteratorDate);
            iterator.SeedBedStatus.PresentDate.Should().BeAfter(statusOfTheIterator.PresentDate);
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
        public void RestartPotentialOfSowSeedTrayPerDay_ShouldResetTheVariable()
        {
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);
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
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);

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
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);

            MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
                .GetMethod("ImplementEstimateReservation"
                    , BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(iterator, null);

            int amountOfOrdersToSow = iterator.SeedBedStatus.Orders
                .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                    && order.Complete == false).Count();

            amountOfOrdersToSow.Should().Be(0);
            iterator.SeedBedStatus.OrderLocationsToAdd.Count.Should().Be(0);
        }

        [Fact]
        public void ImplementEstimateReservation_ShouldWorkWhenTheLimitOfSowPerDayIsReached()
        {
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);

            var ordersToSow = iterator.SeedBedStatus.Orders
                .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                    && order.Complete == false).ToList();

            ordersToSow[1].OrderLocations.Last().SeedTrayAmount = 650;

            int oldOrdersToSowCount = ordersToSow.Count;

            MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
                .GetMethod("ImplementEstimateReservation"
                    , BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(iterator, null);

            int amountOfOrdersToSow = iterator.SeedBedStatus.Orders
                .Where(order => order.EstimateSowDate <= iterator.SeedBedStatus.IteratorDate
                    && order.Complete == false).Count();

            amountOfOrdersToSow.Should().Be(7/*oldOrdersToSowCount - 1*/);
            iterator.SeedBedStatus.OrderLocationsToAdd.Count.Should().Be(1);
        }

        [Fact]
        public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_7_19_ThatHadOnlyCompleteOrders()
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

        //NEXT - The complete orders is ok. Lets comtimue with the partials

        [Fact]
        public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_7_25_ThatHadOnlyPartialOrders()
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

        //[Fact]
        public void DayByDayToRequestDate_ShouldEmptyTheSeedBedByThe_7_25_ThatHadOnlyEmptyOrders()
        {
            SeedBedStatus localStatus = new SeedBedStatus(_presentDate
                    , _mockOf.GreenHouseRepository.Object
                    , _mockOf.SeedTrayRepository.Object
                    , _mockOf.GetOrderMockByRecordType(TypeOfRecord.empty, _presentDate).Object
                    , _mockOf.GetOrderLocationMockByRecordType(TypeOfRecord.empty, _presentDate).Object
                    , _mockOf.GetDeliveryDetailMockByRecordType(TypeOfRecord.empty, _presentDate).Object
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
    }
}
