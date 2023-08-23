using Bogus;
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

    public ClientRepositoryTests()
    {
        _clients = GenerateClients(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.Clients).Returns((MockGenerator.GetQueryableMockDbSet<Client>(_clients)));
    }

    [Fact]
    public void GetAll_ShouldReturnClients()
    {        
        IClientRepository clientRepository = new ClientRepository(_mockSowScheduleDbContex.Object);

        var actual = clientRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertAClient()
    {
        var newClient = GenerateClients(6).Last();

        IClientRepository clientRepository = new ClientRepository(_mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Insert(newClient);

        actual.Should().BeTrue();
        _clients.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Clients.Add(newClient), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveAClient()
    {
        int idOfTheClientToRemove = _clients.Last().ID;
        var clientToRemove = _clients.Find(x => x.ID == idOfTheClientToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Clients.Find(idOfTheClientToRemove)).Returns(clientToRemove);

        IClientRepository clientRepository = new ClientRepository(_mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Remove(idOfTheClientToRemove);

        actual.Should().BeTrue();
        _clients.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Clients.Find(idOfTheClientToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Clients.Remove(clientToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateAClient()
    {
        int idOfTheClientToUpdate = _clients.Last().ID;
        var clientToUpdate = _clients.Find(x => x.ID == idOfTheClientToUpdate);

        var newClientData = new Client
        {
            ID = clientToUpdate.ID,
            Name = "Alejandro",
            NickName ="Alejo",
            PhoneNumber = "12345678",
            OtherNumber = "87654321",
            OrganizationId = 1
        };

        _mockSowScheduleDbContex.Setup(x => x.Clients).Returns(MockGenerator.GetQueryableMockDbSet<Client>(_clients));
        _mockSowScheduleDbContex.Setup(x => x.Clients.Find(newClientData.ID)).Returns(clientToUpdate).Verifiable();

        IClientRepository clientRepository = new ClientRepository(_mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Update(newClientData);

        actual.Should().BeTrue();

        var clientUpdated = _clients.Find(x => x.ID == idOfTheClientToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Clients.Find(newClientData.ID), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        clientUpdated.Name.Should().Be(newClientData.Name);
        clientUpdated.NickName.Should().Be(newClientData.NickName);
        clientUpdated.PhoneNumber.Should().Be(newClientData.PhoneNumber);
        clientUpdated.OtherNumber.Should().Be(newClientData.OtherNumber);
        clientUpdated.OrganizationId.Should().Be(newClientData.OrganizationId);        
    }

    public List<Client> GenerateClients(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeClient = new Faker<Client>()
            .RuleFor(x => x.ID, f => Convert.ToInt16(f.IndexFaker))
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.NickName, f => f.Address.StreetName())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.OtherNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.OrganizationId, f => Convert.ToInt16(f.Address.BuildingNumber()[0]));

        return fakeClient.Generate(count).ToList();
    }
}
