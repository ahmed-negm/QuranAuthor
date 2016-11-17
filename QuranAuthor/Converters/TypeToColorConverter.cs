using QuranAuthor.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace QuranAuthor.Converters
{
    public class TypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (ExplanationType)value;
            switch (type)
            {
                case ExplanationType.Explain:
                    return "#5b9bd5";
                case ExplanationType.Note:
                    return "Black";
                case ExplanationType.Guid:
                    return "#ffa500";
                default:
                    return "Red";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("TypeToColorConverter.ConvertBack");
        }
    }
}
