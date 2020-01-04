using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Synthesizer.Controls
{
	class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool booleanValue))
				booleanValue = false;

			return booleanValue ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value is Visibility visibility) && (visibility == Visibility.Visible);
		}
	}
}
