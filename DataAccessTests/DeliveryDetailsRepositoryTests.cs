using Bogus;
using DataAccess;
using DataAccess.Contracts;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class DeliveryDetailsRepositoryTests
{
    List<DeliveryDetail> _deliveryDetails;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    IDeliveryDetailRepository _deliveryDetailRepository;

    public DeliveryDetailsRepositoryTests()
    {
        _deliveryDetails = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.DeliveryDetails).Returns((MockGenerator.GetQueryableMockDbSet<DeliveryDetail>(_deliveryDetails)));
        _deliveryDetailRepository = new DeliveryDetailRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _deliveryDetailRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _deliveryDetailRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _deliveryDetails.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        var idOfTheRecordToRemove = _deliveryDetails.Last().ID;
        var recordToRemove = _deliveryDetails.Find(x => x.ID == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.DeliveryDetails.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _deliveryDetailRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _deliveryDetails.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _deliveryDetails.Last().ID;
        var recordToUpdate = _deliveryDetails.Find(x => x.ID == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.ID = idOfTheRecordToUpdate;
       
        _mockSowScheduleDbContex.Setup(x => x.DeliveryDetails.Find(newRecordData.ID)).Returns(recordToUpdate);

        bool actual = _deliveryDetailRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _deliveryDetails.Find(x => x.ID == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Find(newRecordData.ID), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.BlockId.Should().Be(newRecordData.BlockId);
        recordUpdated.DeliveryDate.Should().Be(newRecordData.DeliveryDate);
        recordUpdated.SeedTrayAmountDelivered.Should().Be(newRecordData.SeedTrayAmountDelivered);

    }

    public List<DeliveryDetail> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        DateTime starDate = new DateTime(2023, 1, 1);
        DateTime endDate = new DateTime(2023, 12, 31);
        var fakeDeliveryDetail = new Faker<DeliveryDetail>()
            .RuleFor(x => x.ID, f => f.IndexFaker)
            .RuleFor(x => x.BlockId, f => Convert.ToInt32(f.Address.BuildingNumber()[0]))
            .RuleFor(x => x.DeliveryDate, f => f.Date.Between(starDate, endDate))
            .RuleFor(x => x.SeedTrayAmountDelivered, f => f.Random.Short(1,1000));

        return fakeDeliveryDetail.Generate(count).ToList();
    }

    public DeliveryDetail GenerateOneRandomRecord()
    {
        DateTime starDate = new DateTime(2023, 1, 1);
        DateTime endDate = new DateTime(2023, 12, 31);
        var fakeDeliveryDetail = new Faker<DeliveryDetail>()
            .RuleFor(x => x.ID, f => f.IndexFaker)
            .RuleFor(x => x.BlockId, f => Convert.ToInt32(f.Address.BuildingNumber()[0]))
            .RuleFor(x => x.DeliveryDate, f => f.Date.Between(starDate, endDate))
            .RuleFor(x => x.SeedTrayAmountDelivered, f => f.Random.Short(1, 1000));

        return fakeDeliveryDetail.Generate(1).Single();
    }
}

