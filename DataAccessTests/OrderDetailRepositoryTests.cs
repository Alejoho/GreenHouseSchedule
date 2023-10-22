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

namespace DataAccessTests
{
    public class OrderDetailRepositoryTests
    {

        List<OrderDetail> _orderDetails;
        Mock<SowScheduleDBEntities> _mockSowScheduleDbContex;
        OrderDetailRepository _orderDetailRepository;

        public OrderDetailRepositoryTests()
        {
            _orderDetails = GenerateRecords(5);
            _mockSowScheduleDbContex = new Mock<SowScheduleDBEntities>();
            _mockSowScheduleDbContex.Setup(x => x.OrderDetails).Returns((MockGenerator.GetQueryableMockDbSet<OrderDetail>(_orderDetails)));
            _orderDetailRepository = new OrderDetailRepository(_mockSowScheduleDbContex.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnRecords()
        {
            var actual = _orderDetailRepository.GetAll();

            actual.Count().Should().Be(5);
        }

        [Fact]
        public void Insert_ShouldInsertARecord()
        {
            var newRecord = GenerateRecords(6).Last();

            bool actual = _orderDetailRepository.Insert(newRecord);

            actual.Should().BeTrue();
            _orderDetails.Count.Should().Be(6);
            _mockSowScheduleDbContex.Verify(x => x.OrderDetails.Add(newRecord), Times.Once());
            _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Remove_ShouldRemoveARecord()
        {
            int idOfTheRecordToRemove = _orderDetails.Last().Id;
            var recordToRemove = _orderDetails.Find(x => x.Id == idOfTheRecordToRemove);

            _mockSowScheduleDbContex.Setup(x => x.OrderDetails.Find(idOfTheRecordToRemove)).Returns(recordToRemove);

            bool actual = _orderDetailRepository.Remove(idOfTheRecordToRemove);

            actual.Should().BeTrue();
            _orderDetails.Count.Should().Be(4);
            _mockSowScheduleDbContex.Verify(x => x.OrderDetails.Find(idOfTheRecordToRemove), Times.Once());
            _mockSowScheduleDbContex.Verify(x => x.OrderDetails.Remove(recordToRemove), Times.Once());
            _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Update_ShouldUpdateARecord()
        {
            var idOfTheRecordToUpdate = _orderDetails.Last().Id;
            var recordToUpdate = _orderDetails.Find(x => x.Id == idOfTheRecordToUpdate);

            var newRecordData = GenerateOneRandomRecord();
            newRecordData.Id = idOfTheRecordToUpdate;

            _mockSowScheduleDbContex.Setup(x => x.OrderDetails.Find(newRecordData.Id)).Returns(recordToUpdate);

            bool actual = _orderDetailRepository.Update(newRecordData);

            actual.Should().BeTrue();

            var recordUpdated = _orderDetails.Find(x => x.Id == idOfTheRecordToUpdate);
            _mockSowScheduleDbContex.Verify(x => x.OrderDetails.Find(newRecordData.Id), Times.Once());
            _mockSowScheduleDbContex.Verify(x => x.SaveChanges(), Times.Once());

            recordUpdated.OrderId.Should().Be(newRecordData.OrderId);
            recordUpdated.SeedsSource.Should().Be(newRecordData.SeedsSource);
            recordUpdated.Germination.Should().Be(newRecordData.Germination);
            recordUpdated.Description.Should().Be(newRecordData.Description);            
        }

        public List<OrderDetail> GenerateRecords(int count)
        {
            Randomizer.Seed = new Random(123);
            var fakeRecord = GetFaker();

            return fakeRecord.Generate(count).ToList();
        }

        public OrderDetail GenerateOneRandomRecord()
        {
            var fakeRecord = GetFaker();

            return fakeRecord.Generate(1).Single();
        }

        public Faker<OrderDetail> GetFaker()
        {
            short index = 1;
            return new Faker<OrderDetail>()
                .RuleFor(x => x.Id, f => index++)
                .RuleFor(x => x.OrderId, f => f.Random.Short(1,300))
                .RuleFor(x => x.SeedsSource, f => f.Address.StreetName())
                .RuleFor(x => x.Germination, f => f.Random.Byte(45,95))
                .RuleFor(x => x.Description, f => f.Commerce.ProductDescription());
        }
    }
}
