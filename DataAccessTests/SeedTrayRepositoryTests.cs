using Bogus;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class SeedTrayRepositoryTests
{

    List<SeedTray> _seedTrays;
    Mock<SowScheduleContext> _mockSowScheduleDbContex;
    SeedTrayRepository _seedTrayRepository;

    public SeedTrayRepositoryTests()
    {
        _seedTrays = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleContext>();
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
        int idOfTheRecordToRemove = _seedTrays.Last().Id;
        var recordToRemove = _seedTrays.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.SeedTrays.Find((byte)idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _seedTrayRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _seedTrays.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Find((byte)idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _seedTrays.Last().Id;
        var recordToUpdate = _seedTrays.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.SeedTrays.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _seedTrayRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _seedTrays.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.SeedTrays.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.TotalAlveolus.Should().Be(newRecordData.TotalAlveolus);
        recordUpdated.AlveolusLength.Should().Be(newRecordData.AlveolusLength);
        recordUpdated.AlveolusWidth.Should().Be(newRecordData.AlveolusWidth);
        recordUpdated.TrayLength.Should().Be(newRecordData.TrayLength);
        recordUpdated.TrayWidth.Should().Be(newRecordData.TrayWidth);
        recordUpdated.TrayArea.Should().Be(newRecordData.TrayArea);
        recordUpdated.LogicalTrayArea.Should().Be(newRecordData.LogicalTrayArea);
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
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.AlveolusLength, f => f.Random.Byte(10, 25))
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
}


