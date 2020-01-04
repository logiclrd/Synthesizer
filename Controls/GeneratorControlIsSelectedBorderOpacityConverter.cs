using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Synthesizer.Controls
{
	class GeneratorControlIsSelectedBorderOpacityConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		const double UnselectedOpacity = 0.3;
		const double SelectedOpacity = 0.9;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool isSelected)
				return isSelected ? SelectedOpacity : UnselectedOpacity;
			else
				return UnselectedOpacity;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double opacity)
				return (opacity != UnselectedOpacity);
			else
				return false;
		}
	}
}
