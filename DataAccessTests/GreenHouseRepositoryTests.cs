using Bogus;
using DataAccess;
using DataAccess.Contracts;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class GreenHouseRepositoryTests
{

    List<GreenHouse> _greenHouses;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    GreenHouseRepository _greenHouseRepository;

    public GreenHouseRepositoryTests()
    {
        _greenHouses = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.GreenHouses).Returns((MockGenerator.GetQueryableMockDbSet<GreenHouse>(_greenHouses)));
        _greenHouseRepository = new GreenHouseRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _greenHouseRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _greenHouseRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _greenHouses.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.GreenHouses.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _greenHouses.Last().Id;
        var recordToRemove = _greenHouses.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.GreenHouses.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _greenHouseRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _greenHouses.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.GreenHouses.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.GreenHouses.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _greenHouses.Last().Id;
        var recordToUpdate = _greenHouses.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.GreenHouses.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _greenHouseRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _greenHouses.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.GreenHouses.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.Description.Should().Be(newRecordData.Description);
        recordUpdated.Width.Should().Be(newRecordData.Width);
        recordUpdated.Length.Should().Be(newRecordData.Length);
        recordUpdated.GreenHouseArea.Should().Be(newRecordData.GreenHouseArea);
        recordUpdated.SeedTrayArea.Should().Be(newRecordData.SeedTrayArea);
        recordUpdated.AmountOfBlocks.Should().Be(newRecordData.AmountOfBlocks);
        recordUpdated.Active.Should().Be(newRecordData.Active);
    }

    public List<GreenHouse> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public GreenHouse GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<GreenHouse> GetFaker()
    {
        byte index = 1;
        return new Faker<GreenHouse>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Width, f => f.Random.Short(6, 20))
            .RuleFor(x => x.Length, f => f.Random.Short(50, 100))
            .RuleFor(x => x.GreenHouseArea, (f, u) => u.Width * u.Length)
            .RuleFor(x => x.SeedTrayArea, f => f.Random.Short(200, 1500))
            .RuleFor(x => x.AmountOfBlocks, f => f.Random.Byte(2, 4))
            .RuleFor(x => x.Active, f => f.Random.Bool());
    }
}
