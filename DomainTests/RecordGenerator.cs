using Bogus;
using FluentAssertions;

namespace DomainTests;

//LATER - Make some variable to change the present date and don't have to make changes in a lots of places to change 
//that date
internal class RecordGenerator
{
    private List<Order> _orders;
    private List<OrderLocation> _orderLocations;
    private List<Block> _blocks;
    private List<DeliveryDetail> _deliveryDetails;
    private List<SeedTray> _seedTrays;
    private List<GreenHouse> _greenHouses;
    private int _numberOfSelectedOrders;
    private int _numberOfSelectedOrderLocations;
    private int _numberOfSelectedDeliveryDetails;
    private int _orderLocationIndex = 1;
    private int _blockIndex = 1;
    private int _deliveryDetailIndex = 1;

    public RecordGenerator(int amountOfRecords, DateOnly date)
    {
        PopulateLists(amountOfRecords);
        FillNumberOfRecords(date);
    }

    internal void Reset()
    {
        _orders = null;
        _orderLocations = null;
        _blocks = null;
        _deliveryDetails = null;
        _seedTrays = null;
        _greenHouses = null;
        _numberOfSelectedOrders = 0;
        _numberOfSelectedOrderLocations = 0;
        _numberOfSelectedDeliveryDetails = 0;
        _orderLocationIndex = 1;
        _blockIndex = 1;
        _deliveryDetailIndex = 1;
    }

    /// <summary>
    /// Fills three variables with the amount of order, order locations and delivery details from a given date onwards.
    /// </summary>
    /// <param name="date">The date from which start the selection.</param>
    internal void FillNumberOfRecords(DateOnly date)
    {
        _numberOfSelectedOrders = _orders
            .Where(x => x.RealSowDate >= date || x.RealSowDate == null)
            .Count();

        _numberOfSelectedOrderLocations = _orderLocations
            .Where(x => x.Order.RealSowDate >= date
                || x.RealSowDate == null)
            .Count();

        _numberOfSelectedDeliveryDetails = _deliveryDetails
            .Where(x => x.Block.OrderLocation.Order.RealSowDate >= date && x.DeliveryDate >= date)
            .Count();
    }

    private int[] GenerateBalanceOfOrderTypes(int totalOfOrders)
    {
        int[] output = new int[3];
        Random random = new Random(59);

        output[0] = random.Next(Convert.ToInt32(totalOfOrders * 0.2), Convert.ToInt32(totalOfOrders * 0.5));

        output[1] = 8;

        output[2] = totalOfOrders - output[0] - output[1];

        return output;
    }

    private int[] GetAmountDivision(int amountToDivide, int amountOfDivisions)
    {
        int[] output = new int[amountOfDivisions];
        Random random = new Random(5986279);
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

    //LATER - Split this method into a few smaller ones.
    /// <summary>
    /// Generates the desired amount the orders and its derive types
    /// </summary>
    /// <param name="count">The desired amount of orders</param>
    internal void PopulateLists(int count)
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

            //int[] seedlingDivision = amount > 1 ?
            //    GetAmountDivision(order.AmountOfAlgorithmSeedlings, amount) :
            //    new int[1] { order.AmountOfAlgorithmSeedlings };

            int[] seedlingDivision = GetAmountDivision(order.AmountOfAlgorithmSeedlings, amount);

            var fakeOrderLocationRecord = GetOrderLocationFaker(order, seedlingDivision, amount);

            //fakeOrderLocationRecord.UseSeed(2467);

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
            int completedOrderLocations = amount / 2;

            int[] seedlingDivision = amount > 1 ?
                GetAmountDivision(order.AmountOfAlgorithmSeedlings, amount) :
                new int[1] { order.AmountOfAlgorithmSeedlings };

            var fakeOrderLocationRecord = GetOrderLocationFaker(order, seedlingDivision, completedOrderLocations);

            //fakeOrderLocationRecord.UseSeed(2467);

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

            //fakeOrderLocationRecord.UseSeed(2467);

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

            if (orderLocation.RealSowDate != null && orderLocation.RealSowDate < new DateOnly(2023, 6, 8))
            {
                int amount = random.Next(1, 4);

                int[] seedTrayDivision = GetAmountDivision(orderLocation.SeedTrayAmount, amount);

                var fakeBlockRecord = GetBlockFaker(orderLocation, seedTrayDivision);

                //fakeBlockRecord.UseSeed(2467);

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

                    //fakeDeliveryDetail.UseSeed(2467);

                    List<DeliveryDetail> newDeliveryDetails = fakeDeliveryDetail.Generate(amount);

                    block.DeliveryDetails = newDeliveryDetails;

                    _deliveryDetails.AddRange(newDeliveryDetails);
                }
                else
                {
                    if (random.Next(1, 3) == 1 || block.OrderLocation.RealDeliveryDate != null)
                    {
                        int amount = random.Next(2, 3);

                        int[] seedlingDivision = GetAmountDivision(block.SeedTrayAmount, amount);

                        var fakeDeliveryDetail = GetDeliveryDetailFaker(block, seedlingDivision);

                        //fakeDeliveryDetail.UseSeed(2467);

                        List<DeliveryDetail> newDeliveryDetails = fakeDeliveryDetail.Generate(amount - 1);

                        block.DeliveryDetails = newDeliveryDetails;

                        _deliveryDetails.AddRange(newDeliveryDetails);
                    }
                }
            }
        }

        List<Order> monthSelection = completeOrders.Where(x => x.RealDeliveryDate != null && x.RealDeliveryDate > (new DateOnly(2023, 6, 10)).AddDays(-30)).ToList();

        int CompleteOrdersSeedTrayAmount = completeOrders.Sum(x => x.OrderLocations.Sum(y => y.SeedTrayAmount));

        int seedTrayAmountSown = partialOrders.Sum(x => x.OrderLocations.Where(y => y.RealSowDate != null).Sum(y => y.SeedTrayAmount));
        int seedTrayAmountDelayed = partialOrders.Sum(x => x.OrderLocations.Where(y => y.RealSowDate == null).Sum(y => y.SeedTrayAmount));

        int EmptyOrdersSeedTrayAmount = emptyOrders.Sum(x => x.OrderLocations.Sum(y => y.SeedTrayAmount));
    }
    /// <summary>
    /// Generates a list of greenhouses.
    /// </summary>
    /// <param name="count">The amount of greenhouse to include in the list.</param>
    /// <returns></returns>
    internal IEnumerable<GreenHouse> GenerateGreenHouses(int count)
    {
        //Randomizer.Seed = new Random(123);
        var fakeRecord = GetGreenHouseModelFaker();
        fakeRecord.UseSeed(321);

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
            .RuleFor(x => x.SeedTrayArea, f => f.Random.Short(600, 1200)) /* 300 - 1500 600 - 1100 */
            .RuleFor(x => x.AmountOfBlocks, f => f.Random.Byte(2, 4))
            .RuleFor(x => x.Active, f => f.Random.Bool());
    }

    /// <summary>
    /// Generates a list of seedtrays.
    /// </summary>
    /// <param name="count">The amount of seedtrays to include in the list.</param>
    /// <returns></returns>
    internal IEnumerable<SeedTray> GenerateSeedTrays(int count)
    {
        //Randomizer.Seed = new Random(123);
        var fakeRecord = GetSeedTrayFaker();
        fakeRecord.UseSeed(123);

        return fakeRecord.Generate(count);
    }

    private Faker<SeedTray> GetSeedTrayFaker()
    {
        //TODO - Add the selected field to the generator of seedtrays
        byte index = 1;
        short[] amounts = { 1100, 2100, 2450, 1800, 2600, 1050, 2550 };
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
            .RuleFor(x => x.TotalAmount, f =>
            {
                short something = f.Random.Short(1000, 3500);

                if (index <= 8)
                {
                    return amounts[index - 2];
                }
                else
                {
                    return f.Random.Short(900, 2500);
                }
            }
            )
            .RuleFor(x => x.Material, f => f.Vehicle.Type())
            .RuleFor(x => x.Active, f => f.Random.Bool())
            .RuleFor(x => x.IsSelected, (f, u) => u.Id % 2 == 0 ? false : true)
            .RuleFor(x => x.Selected, (f, u) => u.Id % 2 == 0 ? false : true);
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
            .RuleFor(x => x.Sown, () => true);
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
            .RuleFor(x => x.Sown, () => false);
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
                    f.Date.Between(new DateTime(2023, 6, 10),
                        new DateTime(2023, 9, 1))
                    )
                )
            .RuleFor(x => x.RealSowDate, () => null)
            .RuleFor(x => x.WishDate, (f, u) => u.EstimateSowDate.AddDays(u.Product.Specie.ProductionDays))
            .RuleFor(x => x.DateOfRequest, (f, u) => u.WishDate.AddDays(-f.Random.Int(80, 180)))
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.WishDate)
            .RuleFor(x => x.RealDeliveryDate, () => null)
            .RuleFor(x => x.Sown, () => false);
    }

    private Faker<OrderLocation> GetOrderLocationFaker(Order order, int[] seedlingDivision, int completedAmount)
    {
        int indexOfSeedlingDivision = 0;
        DateTime? actualSowDate = order.RealSowDate != null ? order.RealSowDate?.ToDateTime(TimeOnly.MinValue) : null;
        return new Faker<OrderLocation>()
            .RuleFor(x => x.Id, () => _orderLocationIndex++)
            .RuleFor(x => x.GreenHouseId, f =>
            {
                byte greenHouseId = f.Random.Byte(1, 8);

                if (completedAmount > 0)
                {
                    return greenHouseId;
                }
                else
                {
                    return (byte)0;
                }
            })
            .RuleFor(x => x.GreenHouse, (f, u) => _greenHouses.FirstOrDefault(x => x.Id == u.GreenHouseId, null))
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

            .RuleFor(x => x.RealSowDate, (f, u) =>
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
                (f, u) => u.RealSowDate?.AddDays(order.Product.Specie.ProductionDays))
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

    /// <summary>
    /// Generates a list of orders without order locations.
    /// </summary>
    /// <param name="count">The amount of orders to include in the list.</param>
    /// <returns></returns>
    internal IEnumerable<Order> GenerateOrders(int count)
    {
        //Randomizer.Seed = new Random(123);
        var fakeRecord = GetOrderFaker();
        fakeRecord.UseSeed(32128);

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

            .RuleFor(x => x.Sown,
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

    /// <summary>
    /// Generates a list of order locations without delivery details and without belonging to an order.
    /// </summary>
    /// <param name="count">The amount of order locations to include in the list.</param>
    /// <returns></returns>
    internal IEnumerable<OrderLocation> GenerateOrderLocations(int count)
    {
        //Randomizer.Seed = new Random(765);
        var fakeRecord = GetOrderLocationFaker();
        fakeRecord.UseSeed(765);

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
            .RuleFor(x => x.RealSowDate,
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
                (f, u) => u.RealSowDate?.AddDays(f.PickRandom(productionDays)))
            .RuleFor(x => x.RealDeliveryDate, (f, u) => u.EstimateDeliveryDate);
    }

    /// <summary>
    /// Generates a list of delivery details without belonging to an order location.
    /// </summary>
    /// <param name="count">The amount of delivery details to include in the list.</param>
    /// <returns></returns>
    internal IEnumerable<DeliveryDetail> GenerateDeliveryDetails(int count)
    {
        //Randomizer.Seed = new Random(834);
        var fakeRecord = GetDeliveryDetailFaker();
        fakeRecord.UseSeed(834);

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


    /// <summary>
    /// A list of <c>Order</c> completely populated.
    /// </summary>
    internal List<Order> Orders { get => _orders; set => _orders = value; }
    /// <summary>
    /// A list of <c>OrderLocation</c> completely populated.
    /// </summary>
    internal List<OrderLocation> OrderLocations { get => _orderLocations; set => _orderLocations = value; }
    /// <summary>
    /// A list of <c>Block</c> completely populated.
    /// </summary>
    internal List<Block> Blocks { get => _blocks; set => _blocks = value; }
    /// <summary>
    /// A list of <c>DeliveryDetail</c> completely populated.
    /// </summary>
    internal List<DeliveryDetail> DeliveryDetails { get => _deliveryDetails; set => _deliveryDetails = value; }
    /// <summary>
    /// A list of <c>SeedTray</c>.
    /// </summary>
    internal List<SeedTray> SeedTrays { get => _seedTrays; set => _seedTrays = value; }
    /// <summary>
    /// A list of <c>GreenHouse</c>.
    /// </summary>
    internal List<GreenHouse> GreenHouses { get => _greenHouses; set => _greenHouses = value; }
    /// <summary>
    /// The amount of items in the list of orders from a date onwards.
    /// </summary>
    internal int NumberOfSelectedOrders { get => _numberOfSelectedOrders; set => _numberOfSelectedOrders = value; }
    /// <summary>
    /// The amount of items in the list of order locations from a date onwards.
    /// </summary>
    internal int NumberOfSelectedOrderLocations { get => _numberOfSelectedOrderLocations; set => _numberOfSelectedOrderLocations = value; }
    /// <summary>
    /// The amount of items in the list of delivery details from a date onwards.
    /// </summary>
    internal int NumberOfSelectedDeliveryDetails { get => _numberOfSelectedDeliveryDetails; set => _numberOfSelectedDeliveryDetails = value; }
}
