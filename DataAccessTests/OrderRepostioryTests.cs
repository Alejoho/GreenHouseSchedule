﻿using Bogus;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class OrderRepostioryTests
{

    List<Order> _orders;
    Mock<SowScheduleContext> _mockSowScheduleDbContex;
    OrderRepository _orderRepository;

    public OrderRepostioryTests()
    {
        _orders = GenerateRecords(20);
        _mockSowScheduleDbContex = new Mock<SowScheduleContext>();
        _mockSowScheduleDbContex.Setup(x => x.Orders).Returns((MockGenerator.GetQueryableMockDbSet<Order>(_orders)));
        _orderRepository = new OrderRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _orderRepository.GetAll();

        actual.Count().Should().Be(20);
    }

    [Fact]
    public void GetByARealSowDateOn_ShouldReturnFilteredRecords()
    {
        DateOnly date = new DateOnly(2023, 7, 1);

        var actual = _orderRepository.GetByARealSowDateOn(date).ToList();

        int count = _orders
            .Where(x => x.RealSowDate >= date || x.RealSowDate == null)
            .Count();
        actual.Count().Should().Be(count);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _orderRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _orders.Count.Should().Be(21);
        _mockSowScheduleDbContex.Verify(x => x.Orders.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _orders.Last().Id;
        var recordToRemove = _orders.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Orders.Find((short)idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _orderRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _orders.Count.Should().Be(19);
        _mockSowScheduleDbContex.Verify(x => x.Orders.Find((short)idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Orders.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _orders.Last().Id;
        var recordToUpdate = _orders.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Orders.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _orderRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _orders.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Orders.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.ClientId.Should().Be(newRecordData.ClientId);
        recordUpdated.ProductId.Should().Be(newRecordData.ProductId);
        recordUpdated.AmountOfWishedSeedlings.Should().Be(newRecordData.AmountOfWishedSeedlings);
        recordUpdated.AmountOfAlgorithmSeedlings.Should().Be(newRecordData.AmountOfAlgorithmSeedlings);
        recordUpdated.WishDate.Should().Be(newRecordData.WishDate);
        recordUpdated.DateOfRequest.Should().Be(newRecordData.DateOfRequest);
        recordUpdated.EstimateSowDate.Should().Be(newRecordData.EstimateSowDate);
        recordUpdated.EstimateDeliveryDate.Should().Be(newRecordData.EstimateDeliveryDate);
        recordUpdated.RealSowDate.Should().Be(newRecordData.RealSowDate);
        recordUpdated.RealDeliveryDate.Should().Be(newRecordData.RealDeliveryDate);
        recordUpdated.Sown.Should().Be(newRecordData.Sown);
    }

    public List<Order> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(785);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public Order GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<Order> GetFaker()
    {
        int[] productionDays = new[] { 30, 45 };
        short index = 1;
        return new Faker<Order>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.ClientId, f => f.Random.Short(1, 300))
            .RuleFor(x => x.ProductId, f => f.Random.Byte(1, 60))
            .RuleFor(x => x.AmountOfWishedSeedlings, f => f.Random.Int(20000, 80000))
            .RuleFor(x => x.AmountOfAlgorithmSeedlings, (f, u) => Convert.ToInt32(u.AmountOfWishedSeedlings * 1.2))
            .RuleFor(x => x.WishDate, f =>
                DateOnly.FromDateTime(
                    f.Date.Between(new DateTime(2023, 1, 1),
                        new DateTime(2023, 12, 31))
                    )
                )
            .RuleFor(x => x.DateOfRequest, (f, u) => u.WishDate.AddDays(-f.Random.Int(90, 180)))
            .RuleFor(x => x.EstimateSowDate, (f, u) => u.WishDate.AddDays(-f.PickRandom(productionDays)))
            .RuleFor(x => x.EstimateDeliveryDate, (f, u) => u.WishDate)
            .RuleFor(x => x.RealSowDate, (f, u) =>
                f.Random.Bool() ? u.EstimateSowDate : null
                )
            .RuleFor(x => x.RealDeliveryDate, (f, u) => u.EstimateDeliveryDate)
            .RuleFor(x => x.Sown, f => f.Random.Bool());
    }
}
