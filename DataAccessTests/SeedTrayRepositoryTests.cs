using Bogus;
using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class SeedTrayRepositoryTests
{

    List<SeedTray> _seedTrays;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    SeedTrayRepository _seedTrayRepository;

    public SeedTrayRepositoryTests()
    {
        _seedTrays = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.SeedTrays).Returns((MockGenerator.GetQueryableMockDbSet<SeedTray>(_seedTrays)));
        _seedTrayRepository = new SeedTrayRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _seedTrayRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _seedTrayRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _seedTrays.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _seedTrays.Last().ID;
        var recordToRemove = _seedTrays.Find(x => x.ID == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.SeedTrays.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _seedTrayRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _seedTrays.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _seedTrays.Last().ID;
        var recordToUpdate = _seedTrays.Find(x => x.ID == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.ID = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.SeedTrays.Find(newRecordData.ID)).Returns(recordToUpdate);

        bool actual = _seedTrayRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _seedTrays.Find(x => x.ID == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Find(newRecordData.ID), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.TotalAlveolus.Should().Be(newRecordData.TotalAlveolus);
        recordUpdated.AlveolusLength.Should().Be(newRecordData.AlveolusLength);
        recordUpdated.AlveolusWidth.Should().Be(newRecordData.AlveolusWidth);
        recordUpdated.TrayLength.Should().Be(newRecordData.TrayLength);
        recordUpdated.TrayWidth.Should().Be(newRecordData.TrayWidth);
        recordUpdated.TrayArea.Should().Be(newRecordData.TrayArea);
        recordUpdated.TotalAmount.Should().Be(newRecordData.TotalAmount);
        recordUpdated.Material.Should().Be(newRecordData.Material);
        recordUpdated.Preference.Should().Be(newRecordData.Preference);
        recordUpdated.Active.Should().Be(newRecordData.Active);

    }

    public List<SeedTray> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public SeedTray GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<SeedTray> GetFaker()
    {
        byte index = 1;
        byte preference = 1;
        return new Faker<SeedTray>()
            .RuleFor(x => x.ID, f => index++)
            .RuleFor(x => x.Name, f => f.Person.FirstName)
            .RuleFor(x => x.AlveolusLength, f => f.Random.Byte(10, 25))
            .RuleFor(x => x.AlveolusWidth, f => f.Random.Byte(8, 14))
            .RuleFor(x => x.TotalAlveolus, (f, u) => Convert.ToInt16(u.AlveolusLength * u.AlveolusWidth))
            .RuleFor(x => x.TrayLength, f => Convert.ToDecimal(f.Random.Double(0.6, 1.0)))
            .RuleFor(x => x.TrayWidth, f => Convert.ToDecimal(f.Random.Double(0.3, 0.5)))
            .RuleFor(x => x.TrayArea, (f, u) => u.TrayLength * u.TrayWidth)
            .RuleFor(x => x.TotalAmount, f => f.Random.Short(300, 1500))
            .RuleFor(x => x.Material, f => f.Vehicle.Type())
            .RuleFor(x => x.Preference, f => preference++)
            .RuleFor(x => x.Active, f => f.Random.Bool());
    }
}


