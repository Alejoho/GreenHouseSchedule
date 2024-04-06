using Bogus;
using DataAccess.Contracts;
using Domain;
using Domain.Models;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace DomainTests;
//TODO - Maybe implement a foreach to iterate through the actual variable and make sure
// that none value was changed.
public class SeedBedStatusTests
{

    private List<Order> _orders;
    private List<OrderLocation> _orderLocations;
    private List<Block> _blocks;
    private List<DeliveryDetail> _deliveryDetails;
    private List<SeedTray> _seedTrays;
    private List<GreenHouse> _greenHouses;
    private int _orderLocationIndex = 1;
    private int _blockIndex = 1;
    private int _deliveryDetailIndex = 1;

    public SeedBedStatusTests()
    {
        PopulateLists(150);
    }

    [Fact]
    public void GetGreenHouses_ShouldReturnAllGreenHouses()
    {
        var collection = GenerateGreenHouses(5);
        Mock<IGreenHouseRepository> mockGreenHouseRepository =
            new Mock<IGreenHouseRepository>();
        mockGreenHouseRepository.Setup(x => x.GetAll()).Returns(collection);

        SeedBedStatus status = new SeedBedStatus(
            greenHouseRepo: mockGreenHouseRepository.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetGreenHouses",
            BindingFlags.NonPublic | BindingFlags.Instance);

        List<GreenHouseModel> actual =
            (List<GreenHouseModel>)methodInfo.Invoke(status, null);

        actual.Should().HaveCount(5);
        actual[0].Should().BeOfType(typeof(GreenHouseModel));
        mockGreenHouseRepository.Verify(x => x.GetAll(), Times.Once());
    }

    public IEnumerable<GreenHouse> GenerateGreenHouses(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetGreenHouseModelFaker();

        return fakeRecord.Generate(count);
    }

    private Faker<GreenHouse> GetGreenHouseModelFaker()
    {
        byte index = 1;

        return new Faker<GreenHouse>()
            .RuleFor(x => x.Name, () => $"Casa {index}")
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Width, f => f.Random.Short(6, 20))
            .RuleFor(x => x.Length, f => f.Random.Short(50, 100))
            .RuleFor(x => x.GreenHouseArea, (f, u) => u.Width * u.Length)
            .RuleFor(x => x.SeedTrayArea, f => f.Random.Short(200, 1500))
            .RuleFor(x => x.AmountOfBlocks, f => f.Random.Byte(2, 4))
            .RuleFor(x => x.Active, f => f.Random.Bool());
    }

    [Fact]
    public void GetSeedTrays_ShouldReturnAllSeedTrays()
    {
        var collection = GenerateSeedTrays(5);
        Mock<ISeedTrayRepository> mockSeedTrayRepository = new Mock<ISeedTrayRepository>();
        mockSeedTrayRepository.Setup(x => x.GetAll()).Returns(collection);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: mockSeedTrayRepository.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetSeedTrays",
            BindingFlags.NonPublic | BindingFlags.Instance);

        List<SeedTrayModel> actual = (List<SeedTrayModel>)methodInfo.Invoke(status, null);

        actual.Should().HaveCount(5);
        actual[0].Should().BeOfType(typeof(SeedTrayModel));
        mockSeedTrayRepository.Verify(x => x.GetAll(), Times.Once());
    }

    public IEnumerable<SeedTray> GenerateSeedTrays(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetSeedTrayFaker();

        return fakeRecord.Generate(count);
    }

    private Faker<SeedTray> GetSeedTrayFaker()
    {
        byte index = 1;
        byte preference = 1;
        return new Faker<SeedTray>()
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.AlveolusLength, f => f.Random.Byte(10, 20))
            .RuleFor(x => x.AlveolusWidth, f => f.Random.Byte(8, 14))
            .RuleFor(x => x.TotalAlveolus, (f, u) => Convert.ToInt16(u.AlveolusLength * u.AlveolusWidth))
            .RuleFor(x => x.Name, (f, u) => $"Badejas de {u.TotalAlveolus}")
            .RuleFor(x => x.TrayLength, f => Convert.ToDecimal(f.Random.Double(0.6, 1.0)))
            .RuleFor(x => x.TrayWidth, f => Convert.ToDecimal(f.Random.Double(0.3, 0.5)))
            .RuleFor(x => x.TrayArea, (f, u) => u.TrayLength * u.TrayWidth)
            .RuleFor(x => x.LogicalTrayArea, (f, u) => u.TrayArea * f.Random.Decimal(1, 1.2M))
            .RuleFor(x => x.TotalAmount, f => f.Random.Short(300, 1500))
            .RuleFor(x => x.Material, f => f.Vehicle.Type())
            .RuleFor(x => x.Preference, f => preference++)
            .RuleFor(x => x.Active, f => f.Random.Bool());
    }

    [Fact]
    public void GetMajorityDataOfOrders_ShouldRetrieveTheOrders()
    {
        var collection = GenerateOrders(200);

        DateOnly date = new DateOnly(2023, 7, 1);

        var filteredCollection = collection
            .Where(x => x.RealSowDate > date || x.RealSowDate == null);

        Mock<IOrderProcessor> mockOrderProcessor = new Mock<IOrderProcessor>();

        mockOrderProcessor.Setup(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()))
            .Returns(filteredCollection);

        SeedBedStatus status = new SeedBedStatus(
            orderProcessor: mockOrderProcessor.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetMajorityDataOfOrders",
            BindingFlags.NonPublic | BindingFlags.Instance);

        LinkedList<OrderModel> actual =
            (LinkedList<OrderModel>)methodInfo.Invoke(status, null);

        int count = filteredCollection.Count();
        actual.Count.Should().Be(count);
        actual.First.Should().BeOfType(typeof(LinkedListNode<OrderModel>));
        mockOrderProcessor.Verify(x => x.GetOrdersFromADateOn(It.IsAny<DateOnly>()),
            Times.Once());

    }

    public IEnumerable<Order> GenerateOrders(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetOrderFaker();

        return fakeRecord.Generate(count);
    }

    private Faker<Order> GetOrderFaker()
    {
        byte[] productionDays = new byte[] { 30, 45 };
        short index = 1;
        return new Faker<Order>()
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.ClientId, f => f.Random.Short(1, 300))
            .RuleFor(x => x.ProductId, f => f.Random.Byte(1, 60))
            .RuleFor(x => x.Client, () => new Client())
            .RuleFor(x => x.Product,
                f => new Product()
                {
                    Specie = new Species()
                    { ProductionDays = f.PickRandom(productionDays) }
                })
            .RuleFor(x => x.AmountOfWishedSeedlings, f => f.Random.Int(20000, 80000))
            .RuleFor(x => x.AmountOfAlgorithmSeedlings, (f, u) => Convert.ToInt32(u.AmountOfWishedSeedlings * 1.2))
            .RuleFor(x => x.WishDate, f =>
                DateOnly.FromDateTime(
                    f.Date.Between(new DateTime(2023, 1, 1),
                        new DateTime(2023, 12, 31))
                    )
                )
            .RuleFor(x => x.DateOfRequest, (f, u) => u.WishDate.AddDays(-f.Random.Int(50, 180)))
            .RuleFor(x => x.EstimateSowDate, (f, u) => u.WishDate.AddDays(-u.Product.Specie.ProductionDays))
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.WishDate)
            .RuleFor(x => x.RealSowDate, (f, u) =>
                f.Random.Bool() ? u.EstimateSowDate.AddDays(f.Random.Int(0, 7)) : null
                )

            .RuleFor(x => x.RealDeliveryDate,
                (f, u) => u.RealSowDate == null ? null :
                    u.RealSowDate?.AddDays(u.Product.Specie.ProductionDays)
                )

            .RuleFor(x => x.Complete,
                (f, u) =>
                {
                    if (u.RealSowDate != null)
                    {
                        return f.Random.Bool();
                    }
                    return false;
                }
            );
    }

    [Fact]
    public void GetOrderLocations_ShouldRetrieveTheOrderLocations()
    {
        var collection = GenerateOrderLocations(20);

        DateOnly date = new DateOnly(2023, 7, 1);

        var filteredCollection = collection
            .Where(x => x.SowDate > date || x.SowDate == null);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(filteredCollection);

        SeedBedStatus status = new SeedBedStatus(
            orderLocationProcessor: mockOrderLocationProcessor.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        LinkedList<OrderLocationModel> actual =
                (LinkedList<OrderLocationModel>)methodInfo.Invoke(status, null);

        int count = filteredCollection.Count();
        actual.Count.Should().Be(count);
        actual.First.Should().BeOfType(typeof(LinkedListNode<OrderLocationModel>));
        mockOrderLocationProcessor.
            Verify(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()),
            Times.Once());
    }

    private IEnumerable<OrderLocation> GenerateOrderLocations(int count)
    {
        Randomizer.Seed = new Random(765);
        var fakeRecord = GetOrderLocationFaker();

        return fakeRecord.Generate(count);
    }

    private Faker<OrderLocation> GetOrderLocationFaker()
    {
        int[] productionDays = new[] { 30, 45 };
        short index = 1;
        return new Faker<OrderLocation>()
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.GreenHouseId, f => f.Random.Byte(1, 8))
            .RuleFor(x => x.SeedTrayId, f => f.Random.Byte(1, 7))
            .RuleFor(x => x.OrderId, f => f.Random.Short(1, 12))
            .RuleFor(x => x.SeedTrayAmount, f => f.Random.Short(50, 500))
            .RuleFor(x => x.SeedlingAmount, f => f.Random.Int(5000, 35000))
            .RuleFor(x => x.SowDate,
                f => f.Random.Bool() == true ?
                    DateOnly.FromDateTime(
                        f.Date.Between(
                            new DateTime(2023, 1, 1),
                            new DateTime(2023, 12, 31)
                            )
                        )
                    : null
                    )
            .RuleFor(x => x.EstimateDeliveryDate,
                (f, u) => u.SowDate?.AddDays(f.PickRandom(productionDays)))
            .RuleFor(x => x.RealDeliveryDate, (f, u) => u.EstimateDeliveryDate);
    }

    [Fact]
    public void GetDeliveryDetails_ShouldRetrieveTheDeliveryDetails()
    {
        var collection = GenerateDeliveryDetails(20);

        DateOnly date = new DateOnly(2023, 7, 1);

        var filteredCollection = collection
            .Where(x => x.DeliveryDate > date);

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

        mockDeliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(filteredCollection);

        SeedBedStatus status = new SeedBedStatus(
            deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        List<DeliveryDetailModel> actual =
                (List<DeliveryDetailModel>)methodInfo.Invoke(status, null);

        int count = filteredCollection.Count();
        actual.Count.Should().Be(count);
        actual.First().Should().BeOfType(typeof(DeliveryDetailModel));
        mockDeliveryDetailProcessor.
            Verify(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()),
            Times.Once());
    }

    private IEnumerable<DeliveryDetail> GenerateDeliveryDetails(int count)
    {
        Randomizer.Seed = new Random(834);
        var fakeRecord = GetDeliveryDetailFaker();

        return fakeRecord.Generate(count);
    }

    private Faker<DeliveryDetail> GetDeliveryDetailFaker()
    {
        short index = 1;
        return new Faker<DeliveryDetail>()
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.Block,
            f => new Block() { OrderLocationId = f.Random.Int(1, 15) })
            .RuleFor(x => x.DeliveryDate,
            f => DateOnly.FromDateTime(
                f.Date.Between(
                    new DateTime(2023, 1, 1),
                    new DateTime(2023, 12, 31)
                    )
                )
            )
            .RuleFor(x => x.SeedTrayAmountDelivered, f => f.Random.Short(50, 500));
    }
    //NEXT - I have an error in the generator. There are order location sown within a month form the presente date and
    //beside that they have delivery details objects and they shouldn't
    [Fact]
    public void FillDeliveryDetails_ShouldPopulateTheDeliveryDetailsOfTheOrderLocations()
    {
        DateOnly presentDate = new DateOnly(2023, 6, 10);
        DateOnly pastDate = presentDate.AddDays(-90);

        var orderLocationCollection = _orderLocations.Where(x => x.SowDate > pastDate || x.SowDate == null)
            .OrderBy(x => x.SowDate)
            .ThenBy(x => x.Id);

        Mock<IOrderLocationProcessor> mockOrderLocationProcessor = new Mock<IOrderLocationProcessor>();

        mockOrderLocationProcessor
            .Setup(x => x.GetOrderLocationsFromADateOn(It.IsAny<DateOnly>()))
            .Returns(orderLocationCollection);

        var deliveryDetailCollection = _deliveryDetails.Where(x => x.DeliveryDate > pastDate)
            .OrderBy(x => x.DeliveryDate);

        Mock<IDeliveryDetailProcessor> mockDeliveryDetailProcessor = new Mock<IDeliveryDetailProcessor>();

        mockDeliveryDetailProcessor
            .Setup(x => x.GetDeliveryDetailFromADateOn(It.IsAny<DateOnly>()))
            .Returns(deliveryDetailCollection);


        SeedBedStatus status = new SeedBedStatus(presentDate
            , orderLocationProcessor: mockOrderLocationProcessor.Object
            , deliveryDetailProcessor: mockDeliveryDetailProcessor.Object);


        MethodInfo methodInfo_GetOrderLocations = typeof(SeedBedStatus)
            .GetMethod("GetOrderLocations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        LinkedList<OrderLocationModel> orderLocationModel =
                (LinkedList<OrderLocationModel>)methodInfo_GetOrderLocations.Invoke(status, null);


        MethodInfo methodInfo_FillDeliveryDetails = typeof(SeedBedStatus)
            .GetMethod("FillDeliveryDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo_FillDeliveryDetails.Invoke(status, new object[] { orderLocationModel });

        orderLocationModel.Count.Should().Be(orderLocationCollection.Count());

        int deliveryDetailCount = orderLocationModel.Sum(x => x.DeliveryDetails.Count);
        deliveryDetailCollection.Count().Should().Be(deliveryDetailCount);
    }

    private int[] GenerateBalanceOfOrderTypes(int totalOfOrders)
    {
        int[] output = new int[3];
        Random random = new Random(59);

        output[0] = random.Next(Convert.ToInt32(totalOfOrders * 0.2), Convert.ToInt32(totalOfOrders * 0.5));

        output[1] = 8;

        output[2] = totalOfOrders - output[0] - output[1];

        //output = new int[3] { 92, 8, 130 };

        return output;
    }

    private int[] GetAmountDivision(int amountToDivide, int amountOfDivisions)
    {
        int[] output = new int[amountOfDivisions];
        Random random = new Random(59);
        int sum = 0;

        for (int i = 0; i < output.Length; i++)
        {
            if (i != output.Length - 1)
            {
                output[i] = random.Next(Convert.ToInt32(amountToDivide * 0.40), Convert.ToInt32(amountToDivide * 0.58));
                amountToDivide -= output[i];
            }
            else
            {
                output[i] = amountToDivide;
            }
        }

        return output;
    }
    //TODO - Make some variable to change the present date and don't have to make changes in a lots of place to change 
    //that date
    private void PopulateLists(int count)
    {
        _orders = new List<Order>();
        _orderLocations = new List<OrderLocation>();
        _blocks = new List<Block>();
        _deliveryDetails = new List<DeliveryDetail>();
        _seedTrays = GenerateSeedTrays(7).ToList();
        _greenHouses = GenerateGreenHouses(8).ToList();

        Randomizer.Seed = new Random(2467);
        Random random = new Random(95);

        int[] balanceOfOrders = GenerateBalanceOfOrderTypes(count);

        const int numberOfCompletedOrder = 0;
        const int numberOfPartialOrder = 1;
        const int numberOfEmptyOrder = 2;

        List<Order> completeOrders = GetCompleteOrderFaker().Generate(balanceOfOrders[numberOfCompletedOrder]);

        foreach (var order in completeOrders)
        {
            int amount = random.Next(2, 5);

            int[] seedlingDivision = amount > 1 ?
                GetAmountDivision(order.AmountOfAlgorithmSeedlings, amount) :
                new int[1] { order.AmountOfAlgorithmSeedlings };

            var fakeOrderLocationRecord = GetOrderLocationFaker(order, seedlingDivision, amount);

            List<OrderLocation> newOrderLocations = fakeOrderLocationRecord.Generate(amount);

            int totalSeedling = newOrderLocations.Sum(x => x.SeedTrayAmount * x.SeedTray.TotalAlveolus);

            order.AmountOfAlgorithmSeedlings = totalSeedling;
            order.AmountOfWishedSeedlings = Convert.ToInt32(totalSeedling / 1.2);

            order.OrderLocations = newOrderLocations;

            _orderLocations.AddRange(newOrderLocations);
        }

        _orders.AddRange(completeOrders);

        List<Order> partialOrders = GetPartialOrderFaker().Generate(balanceOfOrders[numberOfPartialOrder]);

        foreach (Order order in partialOrders)
        {
            int amount = random.Next(2, 5);
            //int completedOrderLocations = amount == 2 ? amount - 1 : amount - 2;
            //int completedOrderLocations = amount - 1;
            int completedOrderLocations = amount / 2;

            int[] seedlingDivision = amount > 1 ?
                GetAmountDivision(order.AmountOfAlgorithmSeedlings, amount) :
                new int[1] { order.AmountOfAlgorithmSeedlings };

            var fakeOrderLocationRecord = GetOrderLocationFaker(order, seedlingDivision, completedOrderLocations);

            List<OrderLocation> newOrderLocations = fakeOrderLocationRecord.Generate(amount);

            int totalSeedling = newOrderLocations.Sum(x => x.SeedTrayAmount * x.SeedTray.TotalAlveolus);

            order.AmountOfAlgorithmSeedlings = totalSeedling;
            order.AmountOfWishedSeedlings = Convert.ToInt32(totalSeedling / 1.2);

            order.OrderLocations = newOrderLocations;

            _orderLocations.AddRange(newOrderLocations);
        }

        _orders.AddRange(partialOrders);


        List<Order> emptyOrders = GetEmptyOrderFaker().Generate(balanceOfOrders[numberOfEmptyOrder]);

        foreach (Order order in emptyOrders)
        {
            int amount = random.Next(1, 3); ;

            int[] seedlingDivision = amount > 1 ?
                GetAmountDivision(order.AmountOfAlgorithmSeedlings, amount) :
                new int[1] { order.AmountOfAlgorithmSeedlings };

            var fakeOrderLocationRecord = GetOrderLocationFaker(order, seedlingDivision, 0);

            List<OrderLocation> newOrderLocations = fakeOrderLocationRecord.Generate(amount);

            int totalSeedling = newOrderLocations.Sum(x => x.SeedTrayAmount * x.SeedTray.TotalAlveolus);

            order.AmountOfAlgorithmSeedlings = totalSeedling;
            order.AmountOfWishedSeedlings = Convert.ToInt32(totalSeedling / 1.2);

            order.OrderLocations = newOrderLocations;

            _orderLocations.AddRange(newOrderLocations);
        }

        _orders.AddRange(emptyOrders);

        foreach (var orderLocation in _orderLocations)
        {
            orderLocation.SeedlingAmount = orderLocation.SeedTrayAmount * orderLocation.SeedTray.TotalAlveolus;

            if (orderLocation.SowDate != null && orderLocation.SowDate < new DateOnly(2023, 6, 8))
            {
                int amount = random.Next(1, 4);

                int[] seedTrayDivision = GetAmountDivision(orderLocation.SeedTrayAmount, amount);

                var fakeBlockRecord = GetBlockFaker(orderLocation, seedTrayDivision);

                List<Block> newBlocks = fakeBlockRecord.Generate(amount);

                orderLocation.Blocks = newBlocks;

                _blocks.AddRange(newBlocks);
            }
        }

        foreach (var block in _blocks)
        {
            if (block.OrderLocation.RealDeliveryDate != null)
            {
                if (block.OrderLocation.RealDeliveryDate < (new DateOnly(2023, 6, 10)).AddDays(-30))
                {
                    int amount = random.Next(1, 3);

                    int[] seedTrayDivision = GetAmountDivision(block.SeedTrayAmount, amount);

                    var fakeDeliveryDetail = GetDeliveryDetailFaker(block, seedTrayDivision);

                    List<DeliveryDetail> newDeliveryDetails = fakeDeliveryDetail.Generate(amount);

                    block.DeliveryDetails = newDeliveryDetails;

                    _deliveryDetails.AddRange(newDeliveryDetails);
                }
                else
                {
                    if (random.Next(1, 3) == 1)
                    {
                        int amount = random.Next(2, 3);

                        int[] seedlingDivision = GetAmountDivision(block.SeedTrayAmount, amount);

                        var fakeDeliveryDetail = GetDeliveryDetailFaker(block, seedlingDivision);

                        List<DeliveryDetail> newDeliveryDetails = fakeDeliveryDetail.Generate(amount - 1);

                        block.DeliveryDetails = newDeliveryDetails;

                        _deliveryDetails.AddRange(newDeliveryDetails);
                    }
                }
            }
        }

        List<Order> monthSelection = completeOrders.Where(x => x.RealDeliveryDate != null && x.RealDeliveryDate > (new DateOnly(2023, 6, 10)).AddDays(-30)).ToList();

        int CompleteOrdersSeedTrayAmount = completeOrders.Sum(x => x.OrderLocations.Sum(y => y.SeedTrayAmount));

        int seedTrayAmountSown = partialOrders.Sum(x => x.OrderLocations.Where(y => y.SowDate != null).Sum(y => y.SeedTrayAmount));
        int seedTrayAmountDelayed = partialOrders.Sum(x => x.OrderLocations.Where(y => y.SowDate == null).Sum(y => y.SeedTrayAmount));

        int EmptyOrdersSeedTrayAmount = emptyOrders.Sum(x => x.OrderLocations.Sum(y => y.SeedTrayAmount));
    }

    private Faker<Order> GetCompleteOrderFaker()
    {
        short index = 1;
        byte[] productionDays = new byte[] { 30, 45 };
        return new Faker<Order>()
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.ClientId, f => f.Random.Short(1, 300))
            .RuleFor(x => x.ProductId, f => f.Random.Byte(1, 60))
            .RuleFor(x => x.Client, () => new Client())
            .RuleFor(x => x.Product,
                f => new Product()
                {
                    Specie = new Species()
                    { ProductionDays = f.PickRandom(productionDays) }
                })
            .RuleFor(x => x.AmountOfWishedSeedlings, f => f.Random.Int(10000, 50000))
            .RuleFor(x => x.AmountOfAlgorithmSeedlings, (f, u) => Convert.ToInt32(u.AmountOfWishedSeedlings * 1.2))
            .RuleFor(x => x.EstimateSowDate, f =>
                DateOnly.FromDateTime(
                    f.Date.Between(new DateTime(2023, 2, 3),
                        new DateTime(2023, 6, 3))
                    )
                )
            .RuleFor(x => x.RealSowDate, (f, u) => u.EstimateSowDate)
            .RuleFor(x => x.WishDate, (f, u) => u.RealSowDate?.AddDays(u.Product.Specie.ProductionDays))
            .RuleFor(x => x.DateOfRequest, (f, u) => u.WishDate.AddDays(-f.Random.Int(60, 180)))
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.WishDate)
            .RuleFor(x => x.RealDeliveryDate, (f, u) =>
                u.EstimateDeliveryDate < new DateOnly(2023, 6, 10) ? u.EstimateDeliveryDate : null)
            .RuleFor(x => x.Complete, () => true);
    }

    private Faker<Order> GetPartialOrderFaker()
    {
        byte[] productionDays = new byte[] { 30, 45 };
        short index = (short)(_orders.OrderByDescending(x => x.Id).First().Id + 1); ;
        return new Faker<Order>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.ClientId, f => f.Random.Short(1, 300))
            .RuleFor(x => x.ProductId, f => f.Random.Byte(1, 60))
            .RuleFor(x => x.Client, () => new Client())
            .RuleFor(x => x.Product,
                f => new Product()
                {
                    Specie = new Species()
                    { ProductionDays = f.PickRandom(productionDays) }
                })
            .RuleFor(x => x.AmountOfWishedSeedlings, f => f.Random.Int(10000, 50000))
            .RuleFor(x => x.AmountOfAlgorithmSeedlings, (f, u) => Convert.ToInt32(u.AmountOfWishedSeedlings * 1.2))
            .RuleFor(x => x.EstimateSowDate, f =>
                DateOnly.FromDateTime(
                    f.Date.Between(new DateTime(2023, 6, 8),
                        new DateTime(2023, 6, 10))
                    )
                )
            .RuleFor(x => x.RealSowDate, (f, u) => u.EstimateSowDate)
            .RuleFor(x => x.WishDate, (f, u) => u.RealSowDate?.AddDays(u.Product.Specie.ProductionDays))
            .RuleFor(x => x.DateOfRequest, (f, u) => u.WishDate.AddDays(-f.Random.Int(60, 180)))
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.WishDate)
            .RuleFor(x => x.RealDeliveryDate, () => null)
            .RuleFor(x => x.Complete, () => false);
    }

    private Faker<Order> GetEmptyOrderFaker()
    {
        byte[] productionDays = new byte[] { 30, 45 };
        short index = (short)(_orders.OrderByDescending(x => x.Id).First().Id + 1);
        return new Faker<Order>()
            .RuleFor(x => x.Id, () => index++)
            .RuleFor(x => x.ClientId, f => f.Random.Short(1, 300))
            .RuleFor(x => x.ProductId, f => f.Random.Byte(1, 60))
            .RuleFor(x => x.Client, () => new Client())
            .RuleFor(x => x.Product,
                f => new Product()
                {
                    Specie = new Species()
                    { ProductionDays = f.PickRandom(productionDays) }
                })
            .RuleFor(x => x.AmountOfWishedSeedlings, f => f.Random.Int(10000, 50000))
            .RuleFor(x => x.AmountOfAlgorithmSeedlings, (f, u) => Convert.ToInt32(u.AmountOfWishedSeedlings * 1.2))
            .RuleFor(x => x.EstimateSowDate, f =>
                DateOnly.FromDateTime(
                    f.Date.Between(new DateTime(2023, 6, 11),
                        new DateTime(2023, 9, 1))
                    )
                )
            .RuleFor(x => x.RealSowDate, () => null)
            .RuleFor(x => x.WishDate, (f, u) => u.EstimateSowDate.AddDays(u.Product.Specie.ProductionDays))
            .RuleFor(x => x.DateOfRequest, (f, u) => u.WishDate.AddDays(-f.Random.Int(80, 180)))
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.WishDate)
            .RuleFor(x => x.RealDeliveryDate, () => null)
            .RuleFor(x => x.Complete, () => false);
    }

    private Faker<OrderLocation> GetOrderLocationFaker(Order order, int[] seedlingDivision, int completedAmount)
    {
        int indexOfSeedlingDivision = 0;
        DateTime? actualSowDate = order.RealSowDate != null ? order.RealSowDate?.ToDateTime(TimeOnly.MinValue) : null;
        return new Faker<OrderLocation>()
            .RuleFor(x => x.Id, () => _orderLocationIndex++)
            .RuleFor(x => x.GreenHouseId, f => f.Random.Byte(1, 8))
            .RuleFor(x => x.GreenHouse, (f, u) => _greenHouses.Where(x => x.Id == u.GreenHouseId).First())
            .RuleFor(x => x.SeedTrayId, f => f.Random.Byte(1, 7))
            .RuleFor(x => x.SeedTray, (f, u) => _seedTrays.Where(x => x.Id == u.SeedTrayId).First())
            .RuleFor(x => x.OrderId, () => order.Id)
            .RuleFor(x => x.Order, () => order)
            .RuleFor(x => x.SeedlingAmount, () => seedlingDivision[indexOfSeedlingDivision++])
            .RuleFor(x => x.SeedTrayAmount, (f, u) =>
            {
                int seedTrayAlveolus = _seedTrays.Where(x => x.Id == u.SeedTrayId).First().TotalAlveolus;
                short seedTrayAmount = (short)Math.Ceiling((double)(u.SeedlingAmount / seedTrayAlveolus));
                return seedTrayAmount;
            })

            .RuleFor(x => x.SowDate, (f, u) =>
            {
                if (order.RealSowDate != null && completedAmount > 0 && actualSowDate < new DateTime(2023, 6, 10))
                {
                    DateOnly sowDate = DateOnly.FromDateTime((DateTime)actualSowDate);
                    actualSowDate = actualSowDate?.AddDays(f.Random.Int(0, 1));
                    completedAmount--;
                    return sowDate;
                }
                else if (order.RealSowDate != null && completedAmount == 0)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            })

            .RuleFor(x => x.EstimateDeliveryDate,
                (f, u) => u.SowDate?.AddDays(order.Product.Specie.ProductionDays))
            .RuleFor(x => x.RealDeliveryDate, (f, u) =>
                u.EstimateDeliveryDate < new DateOnly(2023, 6, 10) ? u.EstimateDeliveryDate : null);
    }

    private Faker<Block> GetBlockFaker(OrderLocation orderLocation, int[] seedTrayDivision)
    {
        int indexOfSeedlingDivision = 0;
        return new Faker<Block>()
            .RuleFor(x => x.Id, () => _blockIndex++)
            .RuleFor(x => x.OrderLocationId, () => orderLocation.Id)
            .RuleFor(x => x.OrderLocation, () => orderLocation)
            .RuleFor(x => x.BlockNumber, f => f.Random.Byte(1, orderLocation.GreenHouse.AmountOfBlocks))
            .RuleFor(x => x.SeedTrayAmount, () => Convert.ToInt16(seedTrayDivision[indexOfSeedlingDivision++]));
    }

    private Faker<DeliveryDetail> GetDeliveryDetailFaker(Block block, int[] seedTrayDivision)
    {
        int indexOfSeedlingDivision = 0;

        DateOnly actualDeliveryDate = (DateOnly)block.OrderLocation.RealDeliveryDate;

        DateOnly AddOneDay(ref DateOnly date)
        {
            DateOnly output = new DateOnly(date.Year, date.Month, date.Day);
            if (date <= new DateOnly(2023, 6, 10))
            {
                date = date.AddDays(1);
            }
            return output;
        }

        return new Faker<DeliveryDetail>()
            .RuleFor(x => x.Id, () => _deliveryDetailIndex++)
            .RuleFor(x => x.BlockId, () => block.Id)
            .RuleFor(x => x.Block, () => block)
            .RuleFor(x => x.DeliveryDate, () => AddOneDay(ref actualDeliveryDate))
            .RuleFor(x => x.SeedTrayAmountDelivered, () => Convert.ToInt16(seedTrayDivision[indexOfSeedlingDivision++]));
    }
}
