using Domain;
using System.Reflection;
using FluentAssertions;

namespace DomainTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        SeedBedStatus status = new SeedBedStatus(1);
        

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("Test", BindingFlags.NonPublic | BindingFlags.Instance);

        int result = (int)methodInfo.Invoke(status, new object[] { });

        result.Should().Be(1);
    }
}