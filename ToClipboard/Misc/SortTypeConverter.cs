using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using ToClipboard.Model;

namespace ToClipboard.Misc
{
    // see: https://stackoverflow.com/a/38378577/353147
    public class SortTypeConverter : MarkupExtension, IValueConverter
    {
        public SortType DesiredSort { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SortType currentSort = (SortType)value;
            return currentSort == DesiredSort;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
