using System;
using System.Windows;
using System.Windows.Data;


namespace TrainsDemo.View
{
	public class BoolInverter : IValueConverter
	{
		public static readonly BoolInverter Instance = new BoolInverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			bool val = (bool)value;
			return (!val);
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			bool val = (bool)value;
			return (!val);
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class BoolToVisibleHiddenConverter : IValueConverter
	{
		public static readonly BoolToVisibleHiddenConverter Instance = new BoolToVisibleHiddenConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			bool visible = (bool)value;
			if (visible)
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Hidden;
			}

		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class BoolToVisibleCollapsedConverter : IValueConverter
	{
		public static readonly BoolToVisibleCollapsedConverter Instance = new BoolToVisibleCollapsedConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			bool visible = (bool)value;
			if (visible)
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Collapsed;
			}

		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class DateToStringConverter : IValueConverter
	{
		public static readonly DateToStringConverter Instance = new DateToStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			DateTime val = (DateTime)value;
			if (val == DateTime.MinValue)
				return (ConstValues.NotSetValueOneMinus);
			else
				return (val.ToShortDateString());
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class DateTimeToStringConverter : IValueConverter
	{
		public static readonly DateTimeToStringConverter Instance = new DateTimeToStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			DateTime val = (DateTime)value;
			if (val == DateTime.MinValue)
				return (ConstValues.NotSetValueOneMinus);
			else
				return (val.ToShortDateString() + " " + val.ToShortTimeString());
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class TimeToStringConverter : IValueConverter
	{
		public static readonly TimeToStringConverter Instance = new TimeToStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return (ConstValues.NotSetValueOneMinus);

			DateTime val = (DateTime)value;
			if (val == DateTime.MinValue)
				return (ConstValues.NotSetValueOneMinus);
			else
				return (val.ToShortTimeString());
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class TimeSpanToHourMinuteStringConverter : IValueConverter
	{
		public static readonly TimeSpanToHourMinuteStringConverter Instance = new TimeSpanToHourMinuteStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			TimeSpan val = (TimeSpan)value;
			if (val.Minutes < 10)
				return (val.Hours.ToString() + ":0" + val.Minutes.ToString());
			else
				return (val.Hours.ToString() + ":" + val.Minutes.ToString());
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class NullableTimeSpanToHourMinuteStringConverter : IValueConverter
	{
		public static readonly NullableTimeSpanToHourMinuteStringConverter Instance = new NullableTimeSpanToHourMinuteStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			TimeSpan? val = (TimeSpan?)value;
			if (!val.HasValue)
				return "-";
			if (val.Value.Minutes < 10)
				return (val.Value.Hours.ToString() + ":0" + val.Value.Minutes.ToString());
			else
				return (val.Value.Hours.ToString() + ":" + val.Value.Minutes.ToString());
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class NulableTimeSpanToHourMinuteStringConverter : IValueConverter
	{
		public static readonly NulableTimeSpanToHourMinuteStringConverter Instance = new NulableTimeSpanToHourMinuteStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			TimeSpan? val = (TimeSpan?)value;
			if (val.HasValue)
			{
				int hours = (int)val.Value.TotalHours;

				if (val.Value.Minutes < 10)
					return (hours.ToString() + ":0" + val.Value.Minutes.ToString());
				else
					return (hours.ToString() + ":" + val.Value.Minutes.ToString());
			}
			else
			{
				return (ConstValues.NotSetValueOneMinus);
			}
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class BoolToCheckMarkConverter : IValueConverter
	{
		public static readonly BoolToCheckMarkConverter Instance = new BoolToCheckMarkConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			bool val = (bool)value;
			if (val)
				return ("\u2713");
			else
				return ("");
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class ScheduleBadGoodStatusConverter : IValueConverter
	{
		public static readonly ScheduleBadGoodStatusConverter Instance = new ScheduleBadGoodStatusConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			ScheduleStatus val = (ScheduleStatus)value;
			if (val == ScheduleStatus.Good || val == ScheduleStatus.WithErrors)
				return ("\u2713");  //галка
			else
				return ("Х");
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class LoadStatusConverter : IValueConverter
	{
		public static readonly LoadStatusConverter Instance = new LoadStatusConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			LoadStatus val = (LoadStatus)value;
			if (val == LoadStatus.Loading)
				return ("\u27a1");  //стрелка вправо
			else
				return ("");
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class IntToStringConverter : IValueConverter
	{
		public static readonly IntToStringConverter Instance = new IntToStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			return value.ToString();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class StationIsMissedToMenuItemIsEnabledConverter : IMultiValueConverter
	{
		public static readonly StationIsMissedToMenuItemIsEnabledConverter
											Instance = new StationIsMissedToMenuItemIsEnabledConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object[] values, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue
			 || values[1] == DependencyProperty.UnsetValue
			 || values[2] == DependencyProperty.UnsetValue
			 || values[3] == DependencyProperty.UnsetValue)
			{
				return (false);
			}

			if ((bool)values[0])
			{
				if ((bool)values[1] || (bool)values[2] || (bool)values[3])
				{
					return (true);
				}
			}

			return (false);
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class TimeShiftMinutesToTimeStringConverter : IValueConverter
	{
		public static readonly TimeShiftMinutesToTimeStringConverter Instance = new TimeShiftMinutesToTimeStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			if ((short)value > 0)
			{
				return "-" + new TimeSpan(0, (short)value, 0).ToString(@"h\:mm");
			}
			else if ((short)value < 0)
			{
				return "+" + new TimeSpan(0, -(short)value, 0).ToString(@"h\:mm");
			}
			else
			{
				return "0";
			}
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}

	public class NullableTimeToStringConverter : IValueConverter
	{
		public static readonly NullableTimeToStringConverter Instance = new NullableTimeToStringConverter();
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			DateTime? val = (DateTime?)value;
			if (val == null)
				return (string.Empty);
			else
				return (val.Value.TimeOfDay.ToString(@"h\:mm"));
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
		public object ConvertBack(object value, Type targetTypes, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
	}
}