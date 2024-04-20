using DataAccess.Contracts;
using Domain;
using Moq;

namespace DomainTests
{
    internal static class MockOf
    {        
        private static Mock<IGreenHouseRepository> _greenHouseRepository;
        private static Mock<ISeedTrayRepository> _seedTrayRepository;
        private static Mock<IOrderProcessor> _orderProcessor;
        private static Mock<IOrderLocationProcessor> _orderLocationProcessor;
        private static Mock<IDeliveryDetailProcessor> _deliveryDetailProcessor;

        /// <summary>
        /// Initializes and sets up the mocks for the needed repositories and processors based on a date onwards. 
        /// </summary>
        /// <param name="pastDate">The need date to filter the records to include in the collection.</param>
        internal static void GenerateMocks(DateOnly pastDate)
        {
            GenerateGreenHouseMock();
            GenerateSeedTrayMock();
            GenerateOrderMock(pastDate);
            GenerateOrderLocationMock(pastDate);
            GenerateDeliveryDetailMock(pastDate);
        }

        private static void GenerateGreenHouseMock()
        {
            _greenHouseRepository = new Mock<IGreenHouseRepository>();
            _greenHouseRepository.Setup(x => x.GetAll()).Returns(RecordGenerator.GreenHouses);
        }

        private static void GenerateSeedTrayMock()
        {
            _seedTrayRepository = new Mock<ISeedTrayRepository>();
            _seedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator.SeedTrays);
        }

        private static void GenerateOrderMock(DateOnly pastDate)
        {
            var orderCollection = RecordGenerator.Orders
            .Where(x => x.RealSowDate >= pastDate || x.RealSowDate == null)
            .OrderBy(x => x.EstimateSowDate)
            .ThenBy(x => x.DateOfRequest);

            _orderProcessor = new Mock<IOrderProcessor>();

            _orderProcessor
                .Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
                .Returns(orderCollection);
        }

        private static void GenerateOrderLocationMock(DateOnly pastDate)
        {
            var orderLocationCollection = RecordGenerator.OrderLocations
                .Where(x => x.Order.RealSowDate >= pastDate
                    || x.SowDate == null)
                .OrderBy(x => x.SowDate)
                .ThenBy(x => x.Id);

            _orderLocationProcessor = new Mock<IOrderLocationProcessor>();

            _orderLocationProcessor
                .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
                .Returns(orderLocationCollection);
        }

        private static void GenerateDeliveryDetailMock(DateOnly pastDate)
        {
            var deliveryDetailCollection = RecordGenerator.DeliveryDetails
                .Where(x => x.Block.OrderLocation.Order.RealSowDate >= pastDate
                    && x.DeliveryDate >= pastDate)
                .OrderBy(x => x.DeliveryDate);

            _deliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

            _deliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(deliveryDetailCollection);
        }

        /// <summary>
        /// Initializes and sets up a custom mock for the <c>GreenHouseRepository</c>.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to include in the collection.</param>
        /// <returns>A <c>Mock<IGreenHouseRepository></c>.</returns>
        internal static Mock<IGreenHouseRepository> GetCustomGreenHouseMock(int numberOfRecords)
        {
            Mock<IGreenHouseRepository> output = new Mock<IGreenHouseRepository>();

            var collection = RecordGenerator.GenerateGreenHouses(numberOfRecords);

            output.Setup(x => x.GetAll()).Returns(collection);

            return output;
        }

        /// <summary>
        /// Initializes and sets up a custom mock for the <c>SeedTrayRepository</c>.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to include in the collection.</param>
        /// <returns>A <c>Mock<ISeedTrayRepository></c>.</returns>
        internal static Mock<ISeedTrayRepository> GetCustomSeedTrayMock(int numberOfRecords)
        {
            Mock<ISeedTrayRepository> output = new Mock<ISeedTrayRepository>();

            var collection = RecordGenerator.GenerateSeedTrays(numberOfRecords);

            output.Setup(x => x.GetAll()).Returns(collection);

            return output;
        }

        /// <summary>
        /// /// Initializes and sets up a custom mock for the <c>OrderProcessor</c>.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to include in the collection.</param>
        /// <returns>A <c>Mock<IOrderProcessor></c>.</returns>
        internal static Mock<IOrderProcessor> GetCustomOrderMock(int numberOfRecords)
        {
            Mock<IOrderProcessor> output = new Mock<IOrderProcessor>();

            var collection = RecordGenerator.GenerateOrders(numberOfRecords);

            output.Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }

        /// <summary>
        /// /// Initializes and sets up a custom mock for the <c>OrderLocationProcessor</c>.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to include in the collection.</param>
        /// <returns>A <c>Mock<IOrderLocationProcessor></c>.</returns>
        internal static Mock<IOrderLocationProcessor> GetCustomOrderLocationMock(int numberOfRecords)
        {
            Mock<IOrderLocationProcessor> output = new Mock<IOrderLocationProcessor>();

            var collection = RecordGenerator.GenerateOrderLocations(numberOfRecords);         

            output.Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }

        /// <summary>
        /// /// Initializes and sets up a custom mock for the <c>DeliveryDetailProcessor</c>.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to include in the collection.</param>
        /// <returns>A <c>Mock<IDeliveryDetailProcessor></c>.</returns>
        internal static Mock<IDeliveryDetailProcessor> GetCustomDeliveryDetailMock(int numberOfRecords)
        {
            Mock<IDeliveryDetailProcessor> output = new Mock<IDeliveryDetailProcessor>();

            var collection = RecordGenerator.GenerateDeliveryDetails(numberOfRecords);

            output.Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }

        /// <summary>
        /// The Mock of <c>GreenHouseRepository</c> with the invocations cleared.
        /// </summary>
        internal static Mock<IGreenHouseRepository> GreenHouseRepository
        {
            get
            {
                _greenHouseRepository.Invocations.Clear();
                return _greenHouseRepository;
            }
            set => _greenHouseRepository = value;
        }

        /// <summary>
        /// The Mock of <c>SeedTrayRepository</c> with the invocations cleared.
        /// </summary>
        internal static Mock<ISeedTrayRepository> SeedTrayRepository
        {
            get
            {
                _seedTrayRepository.Invocations.Clear();
                return _seedTrayRepository;
            }
            set => _seedTrayRepository = value;
        }

        /// <summary>
        /// The Mock of <c>OrderProcessor</c> with the invocations cleared.
        /// </summary>
        internal static Mock<IOrderProcessor> OrderProcessor
        {
            get
            {
                _orderProcessor.Invocations.Clear();
                return _orderProcessor;
            }
            set => _orderProcessor = value;
        }

        /// <summary>
        /// The Mock of <c>OrderLocationProcessor</c> with the invocations cleared.
        /// </summary>
        internal static Mock<IOrderLocationProcessor> OrderLocationProcessor
        {
            get
            {
                _orderLocationProcessor.Invocations.Clear();
                return _orderLocationProcessor;
            }
            set => _orderLocationProcessor = value;
        }

        /// <summary>
        /// The Mock of <c>DeliveryDetailProcessor</c> with the invocations cleared.
        /// </summary>
        internal static Mock<IDeliveryDetailProcessor> DeliveryDetailProcessor
        {
            get
            {
                _deliveryDetailProcessor.Invocations.Clear();
                return _deliveryDetailProcessor;
            }
            set => _deliveryDetailProcessor = value;
        }
    }
}
