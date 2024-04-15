using DataAccess.Contracts;
using Domain;
using Moq;

namespace DomainTests
{
    internal static class MockOf
    {
        //TODO - Make the documentaion of this class
        private static Mock<IGreenHouseRepository> _greenHouseRepository;
        private static Mock<ISeedTrayRepository> _seedTrayRepository;
        private static Mock<IOrderProcessor> _orderProcessor;
        private static Mock<IOrderLocationProcessor> _orderLocationProcessor;
        private static Mock<IDeliveryDetailProcessor> _deliveryDetailProcessor;

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
            _greenHouseRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._greenHouses);
        }

        private static void GenerateSeedTrayMock()
        {
            _seedTrayRepository = new Mock<ISeedTrayRepository>();
            _seedTrayRepository.Setup(x => x.GetAll()).Returns(RecordGenerator._seedTrays);
        }

        private static void GenerateOrderMock(DateOnly pastDate)
        {
            var orderCollection = RecordGenerator._orders
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
            var orderLocationCollection = RecordGenerator._orderLocations
                .Where(x => x.Order.RealSowDate >= pastDate
                    && (x.SowDate >= pastDate || x.SowDate == null))
                .OrderBy(x => x.SowDate)
                .ThenBy(x => x.Id);

            _orderLocationProcessor = new Mock<IOrderLocationProcessor>();

            _orderLocationProcessor
                .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
                .Returns(orderLocationCollection);
        }

        private static void GenerateDeliveryDetailMock(DateOnly pastDate)
        {
            var deliveryDetailCollection = RecordGenerator._deliveryDetails
                .Where(x => x.Block.OrderLocation.Order.RealSowDate >= pastDate
                    && x.DeliveryDate >= pastDate)
                .OrderBy(x => x.DeliveryDate);

            _deliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

            _deliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(deliveryDetailCollection);
        }

        internal static Mock<IGreenHouseRepository> GetCustomGreenHouseMock(int numberOfRecords)
        {
            Mock<IGreenHouseRepository> output = new Mock<IGreenHouseRepository>();

            var collection = RecordGenerator.GenerateGreenHouses(numberOfRecords);

            output.Setup(x => x.GetAll()).Returns(collection);

            return output;
        }

        internal static Mock<ISeedTrayRepository> GetCustomSeedTrayMock(int numberOfRecords)
        {
            Mock<ISeedTrayRepository> output = new Mock<ISeedTrayRepository>();

            var collection = RecordGenerator.GenerateSeedTrays(numberOfRecords);

            output.Setup(x => x.GetAll()).Returns(collection);

            return output;
        }

        internal static Mock<IOrderProcessor> GetCustomOrderMock(int numberOfRecords)
        {
            Mock<IOrderProcessor> output = new Mock<IOrderProcessor>();

            var collection = RecordGenerator.GenerateOrders(numberOfRecords);

            //var filteredCollection = collection.Where(x => x.RealSowDate >= date || x.RealSowDate == null);

            output.Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }

        internal static Mock<IOrderLocationProcessor> GetCustomOrderLocationMock(int numberOfRecords)
        {
            Mock<IOrderLocationProcessor> output = new Mock<IOrderLocationProcessor>();

            var collection = RecordGenerator.GenerateOrderLocations(numberOfRecords);

            //var filteredCollection = collection.Where(x => x.SowDate >= date || x.SowDate == null);            

            output.Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }

        internal static Mock<IDeliveryDetailProcessor> GetCustomDeliveryDetailMock(int numberOfRecords)
        {
            Mock<IDeliveryDetailProcessor> output = new Mock<IDeliveryDetailProcessor>();

            var collection = RecordGenerator.GenerateDeliveryDetails(numberOfRecords);

            //var filteredCollection = collection.Where(x => x.DeliveryDate >= date);

            output.Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>())).Returns(collection);

            return output;
        }


        internal static Mock<IGreenHouseRepository> GreenHouseRepository
        {
            get
            {
                _greenHouseRepository.Invocations.Clear();
                return _greenHouseRepository;
            }
            set => _greenHouseRepository = value;
        }

        internal static Mock<ISeedTrayRepository> SeedTrayRepository
        {
            get
            {
                _seedTrayRepository.Invocations.Clear();
                return _seedTrayRepository;
            }
            set => _seedTrayRepository = value;
        }

        internal static Mock<IOrderProcessor> OrderProcessor
        {
            get
            {
                _orderProcessor.Invocations.Clear();
                return _orderProcessor;
            }
            set => _orderProcessor = value;
        }

        internal static Mock<IOrderLocationProcessor> OrderLocationProcessor
        {
            get
            {
                _orderLocationProcessor.Invocations.Clear();
                return _orderLocationProcessor;
            }
            set => _orderLocationProcessor = value;
        }

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
