using QuranAuthor.Models;
using QuranAuthor.Repositories;
using System;
using System.Globalization;
using System.Windows.Data;

namespace QuranAuthor.Converters
{
    public class ChapterConverter : IValueConverter
    {
        private ChapterRepository chapterRepository = new ChapterRepository();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var chapterId = (int)value;
            return this.chapterRepository.GetChapters()[chapterId - 1].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ChapterConverter.ConvertBack");
        }
    }
}
