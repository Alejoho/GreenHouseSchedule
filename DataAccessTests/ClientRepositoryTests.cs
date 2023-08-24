﻿using Bogus;
using DataAccess;
using DataAccess.Contracts;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class ClientRepositoryTests
{
    List<Client> _clients;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    IClientRepository _clientRepository;

    public ClientRepositoryTests()
    {
        _clients = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.Clients).Returns((MockGenerator.GetQueryableMockDbSet<Client>(_clients)));
        _clientRepository = new ClientRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _clientRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _clientRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _clients.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Clients.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _clients.Last().ID;
        var recordToRemove = _clients.Find(x => x.ID == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Clients.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _clientRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _clients.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Clients.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Clients.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _clients.Last().ID;
        var recordToUpdate = _clients.Find(x => x.ID == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.ID = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Clients.Find(newRecordData.ID)).Returns(recordToUpdate);

        bool actual = _clientRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _clients.Find(x => x.ID == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Clients.Find(newRecordData.ID), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.Name.Should().Be(newRecordData.Name);
        recordUpdated.NickName.Should().Be(newRecordData.NickName);
        recordUpdated.PhoneNumber.Should().Be(newRecordData.PhoneNumber);
        recordUpdated.OtherNumber.Should().Be(newRecordData.OtherNumber);
        recordUpdated.OrganizationId.Should().Be(newRecordData.OrganizationId);
    }

    public List<Client> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = new Faker<Client>()
            .RuleFor(x => x.ID, f => Convert.ToInt16(f.IndexFaker))
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.NickName, f => f.Address.StreetName())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.OtherNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.OrganizationId, f => Convert.ToInt16(f.Address.BuildingNumber()[0]));

        return fakeRecord.Generate(count).ToList();
    }

    public Client GenerateOneRandomRecord()
    {      
        var fakeRecord = new Faker<Client>()
            .RuleFor(x => x.ID, f => Convert.ToInt16(f.IndexFaker))
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.NickName, f => f.Address.StreetName())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.OtherNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.OrganizationId, f => Convert.ToInt16(f.Address.BuildingNumber()[0]));

        return fakeRecord.Generate(1).Single();
    }
}
