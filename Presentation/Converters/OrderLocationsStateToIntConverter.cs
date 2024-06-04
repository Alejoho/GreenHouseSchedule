using Presentation.Resources;
using SupportLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class OrderLocationsStateToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<OrderLocation> orderLocations)
            {
                if (orderLocations.Any(x => x.EstimateSowDate < new DateOnly().Today()) &&
                    orderLocations.Any(x => x.EstimateSowDate == new DateOnly().Today()))
                {
                    return 1;
                }
                else if (orderLocations.Any(x => x.EstimateSowDate == new DateOnly().Today()))
                {
                    return 0;
                }
                else if (orderLocations.Any(x => x.EstimateSowDate < new DateOnly().Today()))
                {
                    return -1;
                }
                else
                {
                    return 2;
                }
            }

            throw new FormatException("Error al intentar determinar el color de fondo de una orden.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
