using Domain;
using System.Reflection;
using FluentAssertions;

namespace DomainTests;

public class UnitTest1
{
    
    public void Test1()
    {
        SeedBedStatus status = new SeedBedStatus("ale");
        

        MethodInfo methodInfo = typeof(SeedBedStatus)
            .GetMethod("Test", BindingFlags.NonPublic | BindingFlags.Instance);

        int result = (int)methodInfo.Invoke(status, null);

        result.Should().Be(1);
    }
}