using Domain;
using FluentAssertions;
using System.Reflection;

namespace DomainTests
{
    public class DateIteratorAndResourceCheckerTests
    {
        private static DateOnly _presentDate = new DateOnly(2023, 6, 10);
        private DateOnly _pastDate = _presentDate.AddDays(-90);
        private SeedBedStatus status;

        public DateIteratorAndResourceCheckerTests()
        {
            if (!RecordGenerator.Generated == true)
            {
                RecordGenerator.PopulateLists(600);
                RecordGenerator.FillNumberOfRecords(_pastDate);
                MockOf.GenerateMocks(_pastDate);
                RecordGenerator.Generated = true;

                status = new SeedBedStatus(_presentDate
                    , MockOf.GreenHouseRepository.Object
                    , MockOf.SeedTrayRepository.Object
                    , MockOf.OrderProcessor.Object
                    , MockOf.OrderLocationProcessor.Object
                    , MockOf.DeliveryDetailProcessor.Object
                    , true);
            }
        }

        [Fact]
        public void CloneSeedBedStatusObjects_ShouldBeDisconnectFromEachOther()
        {
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);
            status.IteratorDate = new DateOnly();
            status.PresentDate = new DateOnly();
            status.GreenHouses = null;
            status.SeedTrays = null;
            status.RemainingAmountOfSowSeedTrayPerDay = -1;
            status.Orders = null;
            status.OrderLocations = null;
            status.DeliveryDetails = null;
            status.OrdersToDelete = null;
            status.OrderLocationsToDelete = null;
            status.DeliveryDetailsToDelete = null;
            status.OrderLocationsToAdd = null;

            iterator.SeedBedStatus.IteratorDate.Should().BeAfter(status.IteratorDate);
            iterator.SeedBedStatus.PresentDate.Should().BeAfter(status.PresentDate);
            iterator.SeedBedStatus.GreenHouses.Should().NotBeNull();
            iterator.SeedBedStatus.SeedTrays.Should().NotBeNull();
            iterator.SeedBedStatus.RemainingAmountOfSowSeedTrayPerDay.Should().NotBe(-1);
            iterator.SeedBedStatus.Orders.Should().NotBeNull();
            iterator.SeedBedStatus.OrderLocations.Should().NotBeNull();
            iterator.SeedBedStatus.DeliveryDetails.Should().NotBeNull();
            iterator.SeedBedStatus.OrdersToDelete.Should().NotBeNull();
            iterator.SeedBedStatus.OrderLocationsToDelete.Should().NotBeNull();
            iterator.SeedBedStatus.DeliveryDetailsToDelete.Should().NotBeNull();
            iterator.SeedBedStatus.OrderLocationsToAdd.Should().NotBeNull();
        }

        [Fact]
        public void RestartPotentialOfSowSeedTrayPerDay_ShouldWork()
        {
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);
            iterator.SeedBedStatus.RemainingAmountOfSowSeedTrayPerDay = 100;

            MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
                .GetMethod("RestartPotentialOfSowSeedTrayPerDay"
                    , BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(iterator, null);

            iterator.SeedBedStatus.RemainingAmountOfSowSeedTrayPerDay.Should().Be(500);
        }

        [Fact]
        public void ImplementEstimateRelease_ShouldWork()
        {
            DateIteratorAndResourceChecker iterator = new DateIteratorAndResourceChecker(status);
            iterator.SeedBedStatus.RemainingAmountOfSowSeedTrayPerDay = 100;

            MethodInfo methodInfo = typeof(DateIteratorAndResourceChecker)
                .GetMethod("ImplementEstimateRelease"
                    , BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(iterator, null);

            
        }

    }
}
