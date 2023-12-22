using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess.Context;

public class DateOnlyComparer : ValueComparer<DateOnly>
{
    public DateOnlyComparer() : base(
        (x, y) => x.DayNumber == y.DayNumber,
        dateOnly => dateOnly.GetHashCode())
    { }
}

//protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
//{
//    base.ConfigureConventions(configurationBuilder);

//    configurationBuilder.Properties<DateOnly>()
//        .HaveConversion<DateOnlyConverter, DateOnlyComparer>()
//        .HaveColumnType("date");

//    configurationBuilder.Properties<TimeOnly>()
//        .HaveConversion<TimeOnlyConverter, TimeOnlyComparer>();
//}