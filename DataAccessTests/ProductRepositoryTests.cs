using Bogus;
using DataAccess.Repositories;
using DataAccess;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessTests;

public class ProductRepositoryTests
{
    List<Product> _products;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    ProductRepository _productRepository;

    public ProductRepositoryTests()
    {
        _products = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.Products).Returns((MockGenerator.GetQueryableMockDbSet<Product>(_products)));
        _productRepository = new ProductRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _productRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _productRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _products.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Products.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _products.Last().ID;
        var recordToRemove = _products.Find(x => x.ID == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Products.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _productRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _products.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Products.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Products.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        var idOfTheRecordToUpdate = _products.Last().ID;
        var recordToUpdate = _products.Find(x => x.ID == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.ID = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Products.Find(newRecordData.ID)).Returns(recordToUpdate);

        bool actual = _productRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _products.Find(x => x.ID == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Products.Find(newRecordData.ID), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.SpecieId.Should().Be(newRecordData.SpecieId);
        recordUpdated.Variety.Should().Be(newRecordData.Variety);
    }

    public List<Product> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public Product GenerateOneRandomRecord()
    {
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(1).Single();
    }

    public Faker<Product> GetFaker()
    {
        byte index = 1;
        return new Faker<Product>()
            .RuleFor(x => x.ID, f => index++)
            .RuleFor(x => x.SpecieId, f => f.Random.Byte(1,20))
            .RuleFor(x => x.Variety, f => f.Commerce.Product());
    }
}
