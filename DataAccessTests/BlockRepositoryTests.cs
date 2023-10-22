using Bogus;
using DataAccess.Contracts;
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

public class BlockRepositoryTests
{
    List<Block> _blocks;
    Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
    BlockRepository _blockRepository;

    public BlockRepositoryTests()
    {
        _blocks = GenerateRecords(5);
        _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
        _mockSowScheduleDbContex.Setup(x => x.Blocks).Returns((MockGenerator.GetQueryableMockDbSet<Block>(_blocks)));
        _blockRepository = new BlockRepository(_mockSowScheduleDbContex.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnRecords()
    {
        var actual = _blockRepository.GetAll();

        actual.Count().Should().Be(5);
    }

    [Fact]
    public void Insert_ShouldInsertARecord()
    {
        var newRecord = GenerateRecords(6).Last();

        bool actual = _blockRepository.Insert(newRecord);

        actual.Should().BeTrue();
        _blocks.Count.Should().Be(6);
        _mockSowScheduleDbContex.Verify(x => x.Blocks.Add(newRecord), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Remove_ShouldRemoveARecord()
    {
        int idOfTheRecordToRemove = _blocks.Last().Id;
        var recordToRemove = _blocks.Find(x => x.Id == idOfTheRecordToRemove);

        _mockSowScheduleDbContex.Setup(x => x.Blocks.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

        bool actual = _blockRepository.Remove(idOfTheRecordToRemove);

        actual.Should().BeTrue();
        _blocks.Count.Should().Be(4);
        _mockSowScheduleDbContex.Verify(x => x.Blocks.Find(idOfTheRecordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.Blocks.Remove(recordToRemove), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
    }

    [Fact]
    public void Update_ShouldUpdateARecord()
    {
        int idOfTheRecordToUpdate = _blocks.Last().Id;
        var recordToUpdate = _blocks.Find(x => x.Id == idOfTheRecordToUpdate);

        var newRecordData = GenerateOneRandomRecord();
        newRecordData.Id = idOfTheRecordToUpdate;

        _mockSowScheduleDbContex.Setup(x => x.Blocks.Find(newRecordData.Id)).Returns(recordToUpdate);

        bool actual = _blockRepository.Update(newRecordData);

        actual.Should().BeTrue();

        var recordUpdated = _blocks.Find(x => x.Id == idOfTheRecordToUpdate);
        _mockSowScheduleDbContex.Verify(x => x.Blocks.Find(newRecordData.Id), Times.Once());
        _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

        recordUpdated.OrderLocationId.Should().Be(newRecordData.OrderLocationId);
        recordUpdated.BlockNumber.Should().Be(newRecordData.BlockNumber);
        recordUpdated.SeedTrayAmount.Should().Be(newRecordData.SeedTrayAmount);
        recordUpdated.NumberWithinThBlock.Should().Be(newRecordData.NumberWithinThBlock);            
    }

    public List<Block> GenerateRecords(int count)
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();

        return fakeRecord.Generate(count).ToList();
    }

    public Block GenerateOneRandomRecord()
    {
        Randomizer.Seed = new Random(123);
        var fakeRecord = GetFaker();
        return fakeRecord.Generate(1).Single();
    }

    public Faker<Block> GetFaker()
    {
        int index = 1;
        return new Faker<Block>()
            .RuleFor(x => x.Id, f => index++)
            .RuleFor(x => x.OrderLocationId, f => f.Random.Byte())
            .RuleFor(x => x.BlockNumber, f => f.Random.Byte(1, 4))
            .RuleFor(x => x.SeedTrayAmount, f => f.Random.Short(1, 1000))
            .RuleFor(x => x.NumberWithinThBlock, f => f.Random.Byte(1, 25));
    }

}
