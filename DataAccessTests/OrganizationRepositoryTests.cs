using Bogus;
using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class OrganizationRepositoryTests
{

    List<Organization> _organizations;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    OrganizationRepository _organizationRepository;

    public OrganizationRepositoryTests()
    {
        _organizations = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.Organizations).Returns((MockGenerator.GetQueryableMockDbSet<Organization>(_organizations)));
        _organizationRepository = new OrganizationRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _organizationRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _organizationRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _organizations.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Organizations.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _organizations.Last().ID;
        var recordToRemove = _organizations.Find(x => x.ID == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Organizations.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _organizationRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _organizations.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Organizations.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Organizations.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _organizations.Last().ID;
        var recordToUpdate = _organizations.Find(x => x.ID == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.ID = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Organizations.Find(newRecordData.ID)).Returns(recordToUpdate);

        bool actual = _organizationRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _organizations.Find(x => x.ID == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Organizations.Find(newRecordData.ID), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.MunicipalitiesId.Should().Be(newRecordData.MunicipalitiesId);
        recordUpdated.TypeOfOrganizationId.Should().Be(newRecordData.TypeOfOrganizationId);
    }

    public List<Organization> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public Organization GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<Organization> GetFaker()
    {
        short index = 1;
        return new Faker<Organization>()
            .RuleFor(x => x.ID, f => index++)
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.MunicipalitiesId, f => f.Random.Short(1,150))
            .RuleFor(x => x.TypeOfOrganizationId, f => f.Random.Byte(1,6));
    }
}
