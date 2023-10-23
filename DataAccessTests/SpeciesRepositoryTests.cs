using Bogus;
using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;
using SupportLayer.Models;
using DataAccess.Context;

namespace DataAccessTests;

public class SpeciesRepositoryTests
{

    List<Species> _species;
    Mock<SowScheduleContext> _mockSowScheduleDbContex;
    SpeciesRepository _speciesRepository;

    public SpeciesRepositoryTests()
    {
        _species = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleContext>();
        _mockSowScheduleDbContex.Setup(x => x.Species).Returns((MockGenerator.GetQueryableMockDbSet<Species>(_species)));
        _speciesRepository = new SpeciesRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _speciesRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _speciesRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _species.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Species.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _species.Last().Id;
        var recordToRemove = _species.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Species.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _speciesRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _species.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Species.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Species.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _species.Last().Id;
        var recordToUpdate = _species.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Species.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _speciesRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _species.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Species.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.ProductionDays.Should().Be(newRecordData.ProductionDays);
        recordUpdated.WeightOf1000Seeds.Should().Be(newRecordData.WeightOf1000Seeds);
        recordUpdated.AmountOfSeedsPerHectare.Should().Be(newRecordData.AmountOfSeedsPerHectare);
        recordUpdated.WeightOfSeedsPerHectare.Should().Be(newRecordData.WeightOfSeedsPerHectare);
    }

    public List<Species> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public Species GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<Species> GetFaker()
    {
        byte[] productionDays = new[] { (byte)30, (byte)45 };
        byte index = 1;
        return new Faker<Species>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.Name, f => f.Commerce.Product())
            .RuleFor(x => x.ProductionDays, f => f.PickRandom(productionDays))
            .RuleFor(x => x.WeightOf1000Seeds, f => Convert.ToDecimal(f.Random.Short(800, 1750)))
            .RuleFor(x => x.AmountOfSeedsPerHectare, f => f.Random.Int(30000,38000))
            .RuleFor(x => x.WeightOfSeedsPerHectare, (f, u) => Convert.ToDecimal((u.AmountOfSeedsPerHectare*u.WeightOf1000Seeds)/1000));
    }
}
