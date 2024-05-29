using System;

namespace Presentation
{
    public static class DateOnlyExtensions
    {
        public static DateOnly Today(this DateOnly date)
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
