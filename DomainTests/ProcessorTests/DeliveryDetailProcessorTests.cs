using DataAccess.Contracts;
using Domain.Processors;
using FluentAssertions;
using log4net;
using Moq;

namespace DomainTests.ProcessorTests;

public class DeliveryDetailProcessorTests
{
    private Mock<IDeliveryDetailRepository> _repoMock;
    private Mock<ILog> _logMock;
    private DeliveryDetailProcessor _processor;
    private DateOnly _date;
    private DeliveryDetail _newDeliveryDetail;

    public DeliveryDetailProcessorTests()
    {
        _repoMock = new Mock<IDeliveryDetailRepository>();

        _repoMock.Setup(x => x.Insert(It.IsAny<DeliveryDetail>()))
            .Callback<DeliveryDetail>(d => _newDeliveryDetail = d);

        _logMock = new Mock<ILog>();

        _logMock.Setup(x => x.Info(It.IsAny<string>()));

        _processor = new DeliveryDetailProcessor(_logMock.Object, _repoMock.Object);

        _date = DateOnly.FromDateTime(DateTime.Now);
    }

    [Fact]
    public void SaveNewDeliveryDetails_ShouldSaveADeliveryDetail()
    {
        Block block = new Block()
        {
            Id = 5,
            SeedTrayAmount = 100
        };

        short deliveredSeedTrays = 100;

        _processor.SaveNewDeliveryDetail(block, _date, deliveredSeedTrays);

        _newDeliveryDetail.BlockId.Should().Be(block.Id);
        _newDeliveryDetail.SeedTrayAmountDelivered.Should().Be(deliveredSeedTrays);
        block.DeliveryDetails.Should().HaveCount(1);

        _repoMock.Verify(x => x.Insert(It.IsAny<DeliveryDetail>()), Times.Once);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveNewDeliveryDetails_ShouldThrowAnArgumentExceptionOnTheDate()
    {
        Block block = new Block()
        {
            Id = 5,
            SeedTrayAmount = 100
        };

        short deliveredSeedTrays = 100;

        _processor.SaveNewDeliveryDetail(block, _date.AddDays(3), deliveredSeedTrays);

        _newDeliveryDetail.Should().BeNull();
        block.DeliveryDetails.Should().HaveCount(0);
        _repoMock.Verify(x => x.Insert(It.IsAny<DeliveryDetail>()), Times.Never);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void SaveNewDeliveryDetails_ShouldThrowAnArgumentExceptionOnTheSeedTrays()
    {
        Block block = new Block()
        {
            Id = 5,
            SeedTrayAmount = 100
        };

        short deliveredSeedTrays = 125;

        _processor.SaveNewDeliveryDetail(block, _date, deliveredSeedTrays);

        _newDeliveryDetail.Should().BeNull();
        block.DeliveryDetails.Should().HaveCount(0);
        _repoMock.Verify(x => x.Insert(It.IsAny<DeliveryDetail>()), Times.Never);
        _logMock.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
    }


}
