using Bogus;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class DeliveryDetailsRepositoryTests
{
    List<DeliveryDetail> _deliveryDetails;
    Mock<SowScheduleContext> _mockSowScheduleDbContex;
    DeliveryDetailRepository _deliveryDetailRepository;

    public DeliveryDetailsRepositoryTests()
    {
        _deliveryDetails = GenerateRecords(20);
        _mockSowScheduleDbContex = new Mock<SowScheduleContext>();
        _mockSowScheduleDbContex.Setup(x => x.DeliveryDetails).Returns((MockGenerator.GetQueryableMockDbSet<DeliveryDetail>(_deliveryDetails)));
        _deliveryDetailRepository = new DeliveryDetailRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _deliveryDetailRepository.GetAll();

        actual.Count().Should().Be(20);
    }

    [Fact]
    public void GetByADeliveryDateOn_ShouldReturnFilteredRecords()
    {
        _deliveryDetails = GenerateRecords(20);
        DateOnly date = new DateOnly(2023, 8, 1);

        var actual = _deliveryDetailRepository.GetByADeliveryDateOn(date).ToList();

        int count = _deliveryDetails.Where(x => x.DeliveryDate > date).Count();
        actual.Count().Should().Be(count);
    }
    

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _deliveryDetailRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _deliveryDetails.Count.Should().Be(21);
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        var idOfTheRecordToRemove = _deliveryDetails.Last().Id;
        var recordToRemove = _deliveryDetails.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.DeliveryDetails.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _deliveryDetailRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _deliveryDetails.Count.Should().Be(19);
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _deliveryDetails.Last().Id;
        var recordToUpdate = _deliveryDetails.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.DeliveryDetails.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _deliveryDetailRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _deliveryDetails.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.DeliveryDetails.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.BlockId.Should().Be(newRecordData.BlockId);
        recordUpdated.DeliveryDate.Should().Be(newRecordData.DeliveryDate);
        recordUpdated.SeedTrayAmountDelivered.Should().Be(newRecordData.SeedTrayAmountDelivered);

    }

    public List<DeliveryDetail> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(759);
        var fakeDeliveryDetail = GetFaker();

        return fakeDeliveryDetail.Generate(count).ToList();
    }

    public DeliveryDetail GenerateOneRandomRecord()
    {

        var fakeDeliveryDetail = GetFaker();

        return fakeDeliveryDetail.Generate(1).Single();
    }

    public Faker<DeliveryDetail> GetFaker()
    {
        DateTime startDate = new DateTime(2023, 1, 1);
        DateTime endDate = new DateTime(2023, 12, 31);
        int index = 1;
        return new Faker<DeliveryDetail>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.BlockId, f => f.Random.Int(1, 200))
            .RuleFor(x => x.DeliveryDate, f =>
                DateOnly.FromDateTime(f.Date.Between(startDate, endDate)))
            .RuleFor(x => x.SeedTrayAmountDelivered, f => f.Random.Short(1, 1000));
    }
}

