using Bogus;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class MunicipalityRepositoryTests
{

    List<Municipality> _municipalities;
    Mock<SowScheduleContext> _mockSowScheduleDbContex;
    MunicipalityRepository _municipalityRepository;

    public MunicipalityRepositoryTests()
    {
        _municipalities = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleContext>();
        _mockSowScheduleDbContex.Setup(x => x.Municipalities).Returns((MockGenerator.GetQueryableMockDbSet<Municipality>(_municipalities)));
        _municipalityRepository = new MunicipalityRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _municipalityRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _municipalityRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _municipalities.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Municipalities.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _municipalities.Last().Id;
        var recordToRemove = _municipalities.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Municipalities.Find((short)idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _municipalityRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _municipalities.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Municipalities.Find((short)idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Municipalities.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _municipalities.Last().Id;
        var recordToUpdate = _municipalities.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Municipalities.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _municipalityRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _municipalities.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Municipalities.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.ProvinceId.Should().Be(newRecordData.ProvinceId);
    }

    public List<Municipality> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public Municipality GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<Municipality> GetFaker()
    {
        short index = 1;
        return new Faker<Municipality>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.Name, f => f.Address.State())
            .RuleFor(x => x.ProvinceId, f => f.Random.Byte(1, 17));

    }
}
