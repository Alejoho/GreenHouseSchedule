using Bogus;
using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain;
using Domain.Models;
using Domain.Processors;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace DomainTests;

public class SeedBedStatusTests
{
    [Fact]
    public void GetGreenHouses_ShouldReturnAllGreenHouses()
    {
        Mock<IGreenHouseRepository> greenHouseRepository= new Mock<IGreenHouseRepository>();
        var collection = GenerateGreenHouses(5);
        greenHouseRepository.Setup(x => x.GetAll()).Returns(collection);

        SeedBedStatus status = new SeedBedStatus(
            greenHouseRepo: greenHouseRepository.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetGreenHouses", BindingFlags.NonPublic | BindingFlags.Instance);

        List<GreenHouseModel> actual = (List<GreenHouseModel>)methodInfo.Invoke(status,null);

        actual.Should().HaveCount(5);
    }

    [Fact]
    public void GetSeedTrays_ShouldReturnAllSeedTrays()
    {
        Mock<ISeedTrayRepository> seedTrayRepository = new Mock<ISeedTrayRepository>();
        var collection = GenerateSeedTrays(5);
        seedTrayRepository.Setup(x => x.GetAll()).Returns(collection);

        SeedBedStatus status = new SeedBedStatus(
            seedTrayRepo: seedTrayRepository.Object);

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("GetSeedTrays", BindingFlags.NonPublic | BindingFlags.Instance);

        List<SeedTrayModel> actual = (List<SeedTrayModel>)methodInfo.Invoke(status, null);

        actual.Should().HaveCount(5);
    }

    [Fact]
    public void GetMajorityDataOfOrders_ShouldRetrieveTheOrders()
    {
        var algo = new List<Order>();
        Mock<OrderProcessor> orderProcessor = new Mock<OrderProcessor>();
        orderProcessor.Setup(x => x.GetAllOrders()).Returns(algo);

        var tests = "string";

    }


    public IEnumerable<GreenHouse> GenerateGreenHouses(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetGreenHouseFaker();

        return fakeRecord.Generate(count);
    }
    //NEXT  - Round the decimal values to 2 precision digits.
    private Faker<GreenHouse> GetGreenHouseFaker()
    {
        byte index = 1;
        return new Faker<GreenHouse>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.Name, f => f.Commerce.Department())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Width, f => f.Random.Decimal(10, 25))
            .RuleFor(x => x.Length, f => f.Random.Decimal(20, 60))
            .RuleFor(x => x.GreenHouseArea, (f, x) => x.Width * x.Length)
            .RuleFor(x => x.SeedTrayArea,
                (f, x) => x.GreenHouseArea * f.Random.Decimal((decimal)0.7, (decimal)0.9))
            .RuleFor(x => x.AmountOfBlocks, f => f.Random.Byte(1, 4))
            .RuleFor(x => x.Active, f => f.Random.Bool());
    }

    //NEXT - Implement these methods
    public IEnumerable<SeedTray> GenerateSeedTrays(int count)
    {
        throw new NotImplementedException();
    }

    private Faker<SeedTray> GetSeedTrayFaker()
    {
        throw new NotImplementedException();
    }
}
