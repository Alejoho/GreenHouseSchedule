using Bogus;
using DataAccess;
using DataAccess.Contracts;
using DataAccess.Repositories;
using FluentAssertions;
using Moq;
using System.Data.Entity;

namespace DataAccessTests;

public class ClientRepositoryTests
{
    [Fact]
    public void GetAll_ShouldReturnClients()
    {
        List<Client> clientes = GenerateClients(5);
        //var data = clientes.AsQueryable();

        var mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        var mockClientDbSet = MockGenerator.GenerateDbSetMock<DbSet<Client>,Client> (clientes);
        //new Mock<DbSet<Client>>();
        //MockGenerator.GenerateDbSetMock<DbSet<Client>>(clientes);
        //mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.Provider).Returns(data.Provider);
        //mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.ElementType).Returns(data.ElementType);
        //mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.Expression).Returns(data.Expression);
        //mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

        //mockSowScheduleDbContex.Setup(x => x.Clients).Returns((MockGenerator.GenerateDbSetMock<DbSet<Client>>(clientes)).Object);

        mockSowScheduleDbContex.Setup(x => x.Clients).Returns(mockClientDbSet.Object);

        IClientRepository clientRepository = new ClientRepository(mockSowScheduleDbContex.Object);

        var actual = clientRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertAClient()
    {
        var clientes = GenerateClients(5);
        var data = clientes.AsQueryable();
        var newCliente = GenerateClients(1).First();

        var mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        var mockClientDbSet = new Mock<DbSet<Client>>();

        mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.Provider).Returns(data.Provider);
        mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.ElementType).Returns(data.ElementType);
        mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.Expression).Returns(data.Expression);
        mockClientDbSet.As<IQueryable<Client>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
        mockClientDbSet.Setup(x => x.Add(newCliente)).Callback((Client x) => clientes.Add(x));

        mockSowScheduleDbContex.Setup(x => x.Clients).Returns(mockClientDbSet.Object);

        IClientRepository clientRepository = new ClientRepository(mockSowScheduleDbContex.Object);

        bool actual = clientRepository.Insert(newCliente);

        actual.Should().BeTrue();
        clientes.Count.Should().Be(6);
        mockClientDbSet.Verify(x => x.Add(newCliente), Times.Once());
        mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveAClient()
    {

    }

    [Fact]
    public void Remove_ShouldUpdateAClient()
    {

    }

    public List<Client> GenerateClients(int count)
    {
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
