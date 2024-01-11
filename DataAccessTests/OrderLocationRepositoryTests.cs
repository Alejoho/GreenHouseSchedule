using Bogus;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;


namespace DataAccessTests;

public class OrderLocationRepositoryTests
{
    List<OrderLocation> _orderLocations;
    Mock<SowScheduleContext> _mockSowScheduleDbContex;
    OrderLocationRepository _orderLocationRepository;

    public OrderLocationRepositoryTests()
    {
        _orderLocations = GenerateRecords(20);
        _mockSowScheduleDbContex = new Mock<SowScheduleContext>();
        _mockSowScheduleDbContex.Setup(x => x.OrderLocations).Returns((MockGenerator.GetQueryableMockDbSet<OrderLocation>(_orderLocations)));
        _orderLocationRepository = new OrderLocationRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _orderLocationRepository.GetAll();

        actual.Count().Should().Be(20);
    }
    //TODO - These test method of filtered records sometimes pass and sometimes don't
    //I think is because the bogus library isn't generating with the seed correctly
    [Fact]
    public void GetByASowDateOn_ShouldReturnFilteredRecords()
    {
        _orderLocations = GenerateRecords(20);
        DateOnly date = new DateOnly(2023,8,1);

        var actual = _orderLocationRepository.GetByASowDateOn(date).ToList();

        int count = _orderLocations
            .Where(x => x.SowDate > date || x.SowDate == null)
            .Count();
        actual.Count().Should().Be(count);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _orderLocationRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _orderLocations.Count.Should().Be(21);
        _mockSowScheduleDbContex.Verify(x => x.OrderLocations.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _orderLocations.Last().Id;
        var recordToRemove = _orderLocations.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.OrderLocations.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _orderLocationRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _orderLocations.Count.Should().Be(19);
        _mockSowScheduleDbContex.Verify(x => x.OrderLocations.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.OrderLocations.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _orderLocations.Last().Id;
        var recordToUpdate = _orderLocations.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.OrderLocations.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _orderLocationRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _orderLocations.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.OrderLocations.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.GreenHouseId.Should().Be(newRecordData.GreenHouseId);
        recordUpdated.SeedTrayId.Should().Be(newRecordData.SeedTrayId);
        recordUpdated.OrderId.Should().Be(newRecordData.OrderId);
        recordUpdated.SeedTrayAmount.Should().Be(newRecordData.SeedTrayAmount);
        recordUpdated.SeedlingAmount.Should().Be(newRecordData.SeedlingAmount);
        recordUpdated.SowDate.Should().Be(newRecordData.SowDate);
        recordUpdated.EstimateDeliveryDate.Should().Be(newRecordData.EstimateDeliveryDate);
        recordUpdated.RealDeliveryDate.Should().Be(newRecordData.RealDeliveryDate);
    }

    public List<OrderLocation> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(297);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public OrderLocation GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<OrderLocation> GetFaker()
    {
        short[] alveolus = new short[] { 0, 160, 260, 264, 180, 150, 260, 264 };
        short[] productionDays = new short[] { 30, 45 };
        short index = 1;
        return new Faker<OrderLocation>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.GreenHouseId, f => f.Random.Byte(1, 8))
            .RuleFor(x => x.SeedTrayId, f => f.Random.Byte(1, 7))
            .RuleFor(x => x.OrderId, f => f.Random.Short(1, 500))
            .RuleFor(x => x.SeedTrayAmount, f => f.Random.Short(200, 750))
            .RuleFor(x => x.SeedlingAmount, (f, u) => u.SeedTrayAmount * alveolus[u.SeedTrayId])
            .RuleFor(x => x.SowDate, f =>
            f.Random.Bool() == true ?
                DateOnly.FromDateTime(
                    f.Date.Between(new DateTime(2023, 1, 1),
                        new DateTime(2023, 12, 31))) 
                : null
                )
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.SowDate)
            .RuleFor(x => x.RealDeliveryDate, (f, u) => u.EstimateDeliveryDate);
    }
}
