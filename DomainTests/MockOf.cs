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
        /// <param name="pastDate">The needed date to filter the records to include in the collection.</param>
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
            _greenHouseRepository.Setup(x => x.GetAll()).Returns(_generator.GreenHouses);
        }

        private void GenerateSeedTrayMock()
        {
            _seedTrayRepository = new Mock<ISeedTrayRepository>();
            _seedTrayRepository.Setup(x => x.GetAll()).Returns(_generator.SeedTrays);
        }

        private void GenerateOrderMock(DateOnly pastDate)
        {
            var orderCollection = _generator.Orders
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
            var orderLocationCollection = _generator.OrderLocations
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
            var deliveryDetailCollection = _generator.DeliveryDetails
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

            var collection = _generator.GenerateGreenHouses(numberOfRecords);

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

            var collection = _generator.GenerateSeedTrays(numberOfRecords);

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

            var collection = _generator.GenerateOrders(numberOfRecords);

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

            var collection = _generator.GenerateOrderLocations(numberOfRecords);

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

            var collection = _generator.GenerateDeliveryDetails(numberOfRecords);

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

        /// <summary>
        /// Gets a mock of <c>IOrderProcessor</c> that will return only a type records after a specify date.
        /// </summary>
        /// <param name="type">The type of requested records.</param>
        /// <param name="presentDate">The  date to take as a refenrece to filter the <c>Order</c> objects to include in the 
        /// collection that will be returned by the mock.</param>
        /// <returns>A mock of <c>IOrderProcessor</c>.</returns>       
        internal Mock<IOrderProcessor> GetOrderMockByRecordType(TypeOfRecord type, DateOnly presentDate)
        {
            IOrderedEnumerable<Order> orderCollection;

            if (type == TypeOfRecord.complete)
            {
                orderCollection = _generator.Orders
                    .Where(x => x.RealSowDate >= presentDate.AddDays(-90) && x.Complete == true)
                    .OrderBy(x => x.EstimateSowDate)
                    .ThenBy(x => x.DateOfRequest);
            }
            else if (type == TypeOfRecord.partial)
            {
                orderCollection = _generator.Orders
                    .Where(x => x.RealSowDate <= presentDate && x.Complete == false)
                    .OrderBy(x => x.EstimateSowDate)
                    .ThenBy(x => x.DateOfRequest);
            }
            else
            {
                orderCollection = _generator.Orders
                    .Where(x => x.EstimateSowDate >= presentDate)
                    .OrderBy(x => x.EstimateSowDate)
                    .ThenBy(x => x.DateOfRequest);
            }

            Mock<IOrderProcessor> output = new Mock<IOrderProcessor>();

            output.Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
                .Returns(orderCollection);

            return output;
        }

        /// <summary>
        /// Gets a mock of <c>IOrderLocationProcessor</c> that will return only complete records after a specify date.
        /// </summary>
        /// <param name="type">The type of requested records.</param>
        /// <param name="presentDate">The  date to filter the <c>OrderLocation</c> objects to include in the 
        /// collection that will be returned by the mock.</param>
        /// <returns>A mock of <c>IOrderLocationProcessor</c>.</returns>
        internal Mock<IOrderLocationProcessor> GetOrderLocationMockByRecordType(TypeOfRecord type, DateOnly presentDate)
        {
            IOrderedEnumerable<OrderLocation> orderLocationCollection;

            if (type == TypeOfRecord.complete)
            {
                orderLocationCollection = _generator.OrderLocations
                .Where(x => x.Order.RealSowDate >= presentDate.AddDays(-90)
                    && x.Order.Complete == true)
                .OrderBy(x => x.SowDate)
                .ThenBy(x => x.Id);
            }
            else if (type == TypeOfRecord.partial)
            {
                orderLocationCollection = _generator.OrderLocations
                .Where(x => x.Order.RealSowDate <= presentDate
                    && x.Order.Complete == false)
                .OrderBy(x => x.SowDate)
                .ThenBy(x => x.Id);
            }
            else
            {
                orderLocationCollection = _generator.OrderLocations
                .Where(x => x.Order.EstimateSowDate >= presentDate)
                .OrderBy(x => x.SowDate)
                .ThenBy(x => x.Id);
            }

            Mock<IOrderLocationProcessor> output = new Mock<IOrderLocationProcessor>();

            output.Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
                .Returns(orderLocationCollection);

            return output;
        }

        /// <summary>
        /// Gets a mock of <c>IDeliveryDetailProcessor</c> that will return only complete records after a specify date.
        /// </summary>
        /// <param name="type">The type of requested records.</param>
        /// <param name="presentDate">The  date to filter the <c>DeliveryDetail</c> objects to include in the 
        /// collection that will be returned by the mock.</param>
        /// <returns>A mock of <c>IDeliveryDetailProcessor</c>.</returns>
        internal Mock<IDeliveryDetailProcessor> GetDeliveryDetailMockByRecordType(TypeOfRecord type, DateOnly presentDate)
        {
            IOrderedEnumerable<DeliveryDetail> deliveryDetailCollection;

            if (type == TypeOfRecord.complete)
            {
                deliveryDetailCollection = _generator.DeliveryDetails
                .Where(x => x.Block.OrderLocation.Order.RealSowDate >= presentDate.AddDays(-90)
                    && x.Block.OrderLocation.Order.Complete == true)
                .OrderBy(x => x.DeliveryDate);
            }
            else if (type == TypeOfRecord.partial)
            {
                deliveryDetailCollection = _generator.DeliveryDetails.Where(x=>x.Id==-1).OrderBy(x =>x.Id);
            }
            else
            {
                deliveryDetailCollection = _generator.DeliveryDetails.Where(x => x.Id == -1).OrderBy(x => x.Id);
            }

            Mock<IDeliveryDetailProcessor> output = new Mock<IDeliveryDetailProcessor>();

            output.Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
                .Returns(deliveryDetailCollection);

            return output;
        }
    }
}
