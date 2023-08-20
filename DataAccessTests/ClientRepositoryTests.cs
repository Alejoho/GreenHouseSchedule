using Bogus;
using DataAccess;
using DataAccess.Contracts;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;

namespace DataAccessTests;

public class ClientRepositoryTests
{
    [Fact]
    public void GetAll_ShouldReturnClients()
    {
        List<Client> clientes = GenerateClients(5);

        var mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();

        mockSowScheduleDbContex.Setup(x => x.Clients).Returns((MockGenerator.GetQueryableMockDbSet<Client>(clientes)));

        IClientRepository clientRepository = new ClientRepository(mockSowScheduleDbContex.Object);

        var actual = clientRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertAClient()
    {
        var clientes = GenerateClients(5);
        var newClient = GenerateClients(6).Last();

        var mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();

        mockSowScheduleDbContex.Setup(x => x.Clients).Returns(MockGenerator.GetQueryableMockDbSet<Client>(clientes));

        IClientRepository clientRepository = new ClientRepository(mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Insert(newClient);

        actual.Should().BeTrue();
        clientes.Count.Should().Be(6);
        mockSowScheduleDbContex.Verify(x => x.Clients.Add(newClient), Times.Once());
        mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveAClient()
    {
        var clientes = GenerateClients(5);
        int idOfTheClientToRemove = clientes.Last().ID;
        var clientToRemove = clientes.Find(x => x.ID == idOfTheClientToRemove);

        var mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();

        mockSowScheduleDbContex.Setup(x => x.Clients).Returns(MockGenerator.GetQueryableMockDbSet<Client>(clientes));
        mockSowScheduleDbContex.Setup(x => x.Clients.Find(idOfTheClientToRemove)).Returns(clientToRemove);

        IClientRepository clientRepository = new ClientRepository(mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Remove(idOfTheClientToRemove);

        actual.Should().BeTrue();
        clientes.Count.Should().Be(4);
        mockSowScheduleDbContex.Verify(x => x.Clients.Find(idOfTheClientToRemove), Times.Once());
        mockSowScheduleDbContex.Verify(x => x.Clients.Remove(clientToRemove), Times.Once());
        mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateAClient()
    {
        var clientes = GenerateClients(5);
        int idOfTheClientToUpdate = clientes.Last().ID;
        var clientToUpdate = clientes.Find(x => x.ID == idOfTheClientToUpdate);

        var newClientData = new Client
        {
            ID = clientToUpdate.ID,
            Name = "Alejandro",
            NickName ="Alejo",
            PhoneNumber = "12345678",
            OtherNumber = "87654321",
            OrganizationId = 1
        };

        var mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();

        mockSowScheduleDbContex.Setup(x => x.Clients).Returns(MockGenerator.GetQueryableMockDbSet<Client>(clientes));
        mockSowScheduleDbContex.Setup(x => x.Clients.Find(newClientData.ID)).Returns(clientToUpdate).Verifiable();

        IClientRepository clientRepository = new ClientRepository(mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Update(newClientData);

        actual.Should().BeTrue();

        var clientUpdated = clientes.Find(x => x.ID == idOfTheClientToUpdate);
        mockSowScheduleDbContex.Verify(x => x.Clients.Find(newClientData.ID), Times.Once());
        mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

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
