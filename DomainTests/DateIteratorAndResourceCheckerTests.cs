using Domain;
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
                    , MockOf.GreenHouseRepository.Object
                    , MockOf.SeedTrayRepository.Object
                    , MockOf.OrderProcessor.Object
                    , MockOf.OrderLocationProcessor.Object
                    , MockOf.DeliveryDetailProcessor.Object
                    , true);

            }

            //if (RecordGenerator.Times == 4)
            //{
            //    RecordGenerator.Reset();
            //}
        }

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
        public void ImplementEstimateReservation_ShouldWork()
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
    }
}
