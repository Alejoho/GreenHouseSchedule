using DataAccess.Contracts;
using Domain;
using Moq;

namespace DomainTests
{
    internal class MockOf
    {        
        private Mock<IGreenHouseRepository> _greenHouseRepository;
        private Mock<ISeedTrayRepository> _seedTrayRepository;
        private Mock<IOrderProcessor> _orderProcessor;
        private Mock<IOrderLocationProcessor> _orderLocationProcessor;
        private Mock<IDeliveryDetailProcessor> _deliveryDetailProcessor;
        private readonly RecordGenerator _generator;

        public MockOf(RecordGenerator generator, DateOnly date)
        {
            _generator = generator;
            GenerateMocks(date);
        }

        /// <summary>
        /// Initializes and sets up the mocks for the needed repositories and processors based on a date onwards. 
        /// </summary>
        /// <param name="pastDate">The need date to filter the records to include in the collection.</param>
        internal void GenerateMocks(DateOnly pastDate)
        {
            GenerateGreenHouseMock();
            GenerateSeedTrayMock();
            GenerateOrderMock(pastDate);
            GenerateOrderLocationMock(pastDate);
            GenerateDeliveryDetailMock(pastDate);
        }

        private void GenerateGreenHouseMock()
        {
            _greenHouseRepository = new Mock<IGreenHouseRepository>();
            _greenHouseRepository.Setup(x => x.GetAll()).Returns(RecordGenerator.GreenHouses);
        }

        private void GenerateSeedTrayMock()
        {
            _seedTrayRepository = new Mock<ISeedTrayRepository>();
            _seedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator.SeedTrays);
        }

        private void GenerateOrderMock(DateOnly pastDate)
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

        private void GenerateOrderLocationMock(DateOnly pastDate)
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

        private void GenerateDeliveryDetailMock(DateOnly pastDate)
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
        internal Mock<IGreenHouseRepository> GetCustomGreenHouseMock(int numberOfRecords)
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
        internal Mock<ISeedTrayRepository> GetCustomSeedTrayMock(int numberOfRecords)
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
        internal Mock<IOrderProcessor> GetCustomOrderMock(int numberOfRecords)
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
        internal Mock<IOrderLocationProcessor> GetCustomOrderLocationMock(int numberOfRecords)
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
        internal Mock<IDeliveryDetailProcessor> GetCustomDeliveryDetailMock(int numberOfRecords)
        {
            Mock<IDeliveryDetailProcessor> output = new Mock<IDeliveryDetailProcessor>();

            var collection = RecordGenerator.GenerateDeliveryDetails(numberOfRecords);

            output.Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }

        /// <summary>
        /// The Mock of <c>GreenHouseRepository</c> with the invocations cleared.
        /// </summary>
        internal Mock<IGreenHouseRepository> GreenHouseRepository
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
        internal Mock<ISeedTrayRepository> SeedTrayRepository
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
        internal Mock<IOrderProcessor> OrderProcessor
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
        internal Mock<IOrderLocationProcessor> OrderLocationProcessor
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
        internal Mock<IDeliveryDetailProcessor> DeliveryDetailProcessor
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
