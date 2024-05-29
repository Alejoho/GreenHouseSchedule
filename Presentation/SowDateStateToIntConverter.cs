﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation
{
    public class SowDateStateToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
            {
                if(date == DateOnly.FromDateTime(DateTime.Today))
                {
                    return 0;
                }
                else if (date < DateOnly.FromDateTime(DateTime.Today))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }  
            }

            throw new FormatException("Error al intentar convertir un dato tipo Fecha a Bool.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
