using Bogus;
using DataAccess.Repositories;
using Domain;
using Moq;

namespace DomainTests;

public class SeedBedStatusTests
{
    [Fact]
    public void GetGreenHouses_ShouldReturnAllGreenHouses()
    {
        Mock<GreenHouseRepository> greenHouseRepository= new Mock<GreenHouseRepository>();
        var collection = GenerateGreenHouses(5);
        greenHouseRepository.Setup(x => x.GetAll()).Returns(collection);

    }


    public IEnumerable<GreenHouse> GenerateGreenHouses(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetGreenHouseFaker();

        return fakeRecord.Generate(count);
    }

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
}
