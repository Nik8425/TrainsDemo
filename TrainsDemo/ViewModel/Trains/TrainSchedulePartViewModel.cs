using System;
using TrainsDemo.Model.IntermediateData;

namespace TrainsDemo.ViewModel.Trains
{
	/// <summary>
	/// View Model для строки расписания
	/// </summary>
	public class TrainSchedulePartViewModel : ViewModelBase
	{
		#region Fields

		private bool isSelected = false;

		#endregion Fields

		#region Properties

		/// <summary>
		/// ссылка на модель строки расписания
		/// </summary>
		public SchedulePart SchedulePart { get; private set; }

		/// <summary>
		/// Ссылка на ViewModel всего расписания.
		/// </summary>
		public TrainScheduleViewModel WholeScheduleViewModel { get; private set; }

		/// <summary>
		/// Позиция строки в списке. 
		/// </summary>
		public int PositionInList { get; set; }

		/// <summary>
		/// Признак того, что строка расписания выделена.
		/// </summary>
		public bool IsSelected
		{
			get { return (isSelected); }
			set
			{
				if (isSelected != value)
				{
					isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		/// <summary>
		/// Включать ли станцию в список, по которому будет проложен маршрут
		/// </summary>
		public bool IsInPath
		{
			get { return SchedulePart.IsInPath; }
			set
			{
				if (SchedulePart.IsInPath != value)
				{
					SchedulePart.IsInPath = value;
					WholeScheduleViewModel.FoundSimilarDB = false;
					WholeScheduleViewModel.NotifyPropsChangedForSchedule();
					OnPropertyChanged(nameof(IsInPath));
				}
			}
		}

		/// <summary>
		/// Код экспресс станции.
		/// </summary>
		public int CodeExpress
		{
			get { return (SchedulePart.CodeExpress); }
			set
			{
				if (SchedulePart.CodeExpress != value)
				{
					SchedulePart.CodeExpress = value;
					WholeScheduleViewModel.FoundSimilarDB = false;
					OnPropertyChanged(nameof(CodeExpress));
				}
			}
		}

		/// <summary>
		/// Название станции
		/// </summary>
		public string StationName
		{
			get { return (SchedulePart.StationName); }
			set
			{
				if (SchedulePart.StationName != value)
				{
					SchedulePart.StationName = value;
					OnPropertyChanged(nameof(StationName));
				}
			}
		}

		/// <summary>
		/// Время прибытия на станцию. Если не задано, то null.
		/// </summary>
		public DateTime? ArrivalTime
		{
			get { return (SchedulePart.ArrivalTime); }
			set
			{
				// -- берём из value только временную часть --
				if (SchedulePart.ArrivalTime == null
						||
					(SchedulePart.ArrivalTime != null && SchedulePart.ArrivalTime.Value.TimeOfDay != value.Value.TimeOfDay))
				{
					WholeScheduleViewModel.FoundSimilarDB = false;
				}
			}
		}

		/// <summary>
		/// Время прибытия на станцию с учетом временного сдвига.
		/// </summary>
		public DateTime? ArrivalTimeWithShift
		{
			get { return (SchedulePart.ArrivalTimeWithShift); }
			set
			{
				if (SchedulePart.ArrivalTimeWithShift == null
					||
					(SchedulePart.ArrivalTimeWithShift != null &&
					 SchedulePart.ArrivalTimeWithShift.Value.TimeOfDay != value.Value.TimeOfDay))
				{
					WholeScheduleViewModel.FoundSimilarDB = false;
				}
			}
		}

		/// <summary>
		/// Время отправления со станции. Если не задано, то null.
		/// </summary>
		public DateTime? DepartureTime
		{
			get { return (SchedulePart.DepartureTime); }
			set
			{
				// -- берём из value только временную часть --
				if (SchedulePart.DepartureTime == null
						||
					(SchedulePart.DepartureTime != null && SchedulePart.DepartureTime.Value.TimeOfDay != value.Value.TimeOfDay))
				{
					if (SchedulePart.PreviousPart == null)
					{
						WholeScheduleViewModel.TrainViewModel.InitTime = value.Value.TimeOfDay;
					}
					WholeScheduleViewModel.FoundSimilarDB = false;
				}
			}
		}

		/// <summary>
		/// Время отправления со станции с учетом временного сдвига.
		/// </summary>
		public DateTime? DepartureTimeWithShift
		{
			get { return (SchedulePart.DepartureTimeWithShift); }
			set
			{
				if (SchedulePart.DepartureTimeWithShift == null
						||
					(SchedulePart.DepartureTimeWithShift != null &&
					 SchedulePart.DepartureTimeWithShift.Value.TimeOfDay != value.Value.TimeOfDay))
				{
					if (SchedulePart.PreviousPart == null)
					{
						WholeScheduleViewModel.TrainViewModel.InitTime = DepartureTime.Value.TimeOfDay;
					}
					WholeScheduleViewModel.FoundSimilarDB = false;
				}
			}
		}

		/// <summary>
		/// Временной свдиг
		/// </summary>
		public short TimeShift
		{
			get { return (SchedulePart.TimeShift); }
			set
			{
				if (SchedulePart.TimeShift != value)
				{
					SchedulePart.TimeShift = value;
					WholeScheduleViewModel.NotifyPropsChangedForSchedule();
				}
			}
		}

		/// <summary>
		/// Сутки прибытия на станцию. Если не задано, то null.
		/// </summary>
		public string ArrivalDay
		{
			get
			{
				if (!SchedulePart.ArrivalTime.HasValue || !WholeScheduleViewModel.FirstPart.DepartureTime.HasValue)
				{
					return (ConstValues.NotSetValueOneMinus);
				}

				return (CalcDays(SchedulePart.ArrivalTime.Value.Date, WholeScheduleViewModel.FirstPart.DepartureTime.Value.Date).ToString());
			}
			set
			{
				int number;
				bool result = Int32.TryParse(value, out number);
				if (result)
				{
					if (SchedulePart.ArrivalTime.HasValue)
					{
						int diff = number - CalcDays(SchedulePart.ArrivalTime.Value.Date, WholeScheduleViewModel.FirstPart.DepartureTime.Value.Date);
						if (diff != 0)
						{
							SchedulePart.AddDaysToArrivalDate(diff);
							WholeScheduleViewModel.NotifyPropsChangedForSchedule();
						}
					}
				}
			}
		}

		/// <summary>
		/// Сутки прибытия на станцию c учётом свдига.
		/// </summary>
		public string ArrivalDayWithShift
		{
			get
			{
				if (!SchedulePart.ArrivalTimeWithShift.HasValue || !WholeScheduleViewModel.FirstPart.DepartureTimeWithShift.HasValue)
				{
					return (ConstValues.NotSetValueOneMinus);
				}

				return ((SchedulePart.ArrivalTimeWithShift.Value.Date - WholeScheduleViewModel.FirstPart.DepartureTimeWithShift.Value.Date).Days + 1).ToString();
			}
			set
			{
				int number;
				bool result = Int32.TryParse(value, out number);
				if (result)
				{
					if (SchedulePart.ArrivalTimeWithShift.HasValue)
					{
						int diff = number - CalcDays(SchedulePart.ArrivalTimeWithShift.Value.Date, WholeScheduleViewModel.FirstPart.DepartureTimeWithShift.Value.Date);
						if (diff != 0)
						{
							SchedulePart.AddDaysToArrivalDate(diff);
							WholeScheduleViewModel.NotifyPropsChangedForSchedule();
						}
					}
				}
			}
		}

		/// <summary>
		/// Сутки отправления на станцию. Если не задано, то null.
		/// </summary>
		public string DepartureDay
		{
			get
			{
				if (!SchedulePart.DepartureTime.HasValue || !WholeScheduleViewModel.FirstPart.DepartureTime.HasValue)
				{
					return (ConstValues.NotSetValueOneMinus);
				}

				return ((SchedulePart.DepartureTime.Value.Date - WholeScheduleViewModel.FirstPart.DepartureTime.Value.Date).Days + 1).ToString();
			}
			set
			{
				int number;
				bool result = Int32.TryParse(value, out number);
				if (result)
				{
					if (SchedulePart.DepartureTime.HasValue)
					{
						int diff = number - CalcDays(SchedulePart.DepartureTime.Value.Date, WholeScheduleViewModel.FirstPart.DepartureTime.Value.Date);
						if (diff != 0)
						{
							SchedulePart.AddDaysToDepartureDate(diff);
							WholeScheduleViewModel.NotifyPropsChangedForSchedule();
						}
					}
				}
			}
		}

		/// <summary>
		/// Сутки отправления со станции c учётом свдига.
		/// </summary>
		public string DepartureDayWithShift
		{
			get
			{
				if (!SchedulePart.DepartureTimeWithShift.HasValue ||
					!WholeScheduleViewModel.FirstPart.DepartureTimeWithShift.HasValue)
				{
					return (ConstValues.NotSetValueOneMinus);
				}

				return
					((SchedulePart.DepartureTimeWithShift.Value.Date -
					  WholeScheduleViewModel.FirstPart.DepartureTimeWithShift.Value.Date).Days + 1).ToString();
			}
			set
			{
				int number;
				bool result = Int32.TryParse(value, out number);
				if (result)
				{
					if (SchedulePart.DepartureTimeWithShift.HasValue)
					{
						int diff = number - CalcDays(SchedulePart.DepartureTimeWithShift.Value.Date, WholeScheduleViewModel.FirstPart.DepartureTimeWithShift.Value.Date);
						if (diff != 0)
						{
							SchedulePart.AddDaysToDepartureDate(diff);
							WholeScheduleViewModel.NotifyPropsChangedForSchedule();
						}
					}
				}
			}
		}

		/// <summary>
		/// Время стоянки в виде String.
		/// </summary>
		public string StayTime
		{
			get
			{
				if (DepartureTime.HasValue && ArrivalTime.HasValue)
				{
					int stayMinutes = (int)(DepartureTime.Value - ArrivalTime.Value).TotalMinutes;
					return stayMinutes.ToString();
				}
				return (ConstValues.NotSetValueOneMinus);
			}
		}

		/// <summary>
		/// Растояние в километрах от начала маршрута.
		/// </summary>
		public int KM
		{
			get { return (SchedulePart.DistanceKM); }
			set
			{
				if (SchedulePart.DistanceKM != value)
				{
					SchedulePart.DistanceKM = value;

					OnPropertyChanged(nameof(KM));
					WholeScheduleViewModel.NotifyPropsChangedForSchedule();
				}
			}
		}

		/// <summary>
		/// Признак технической стоянки
		/// </summary>
		public bool TehStop
		{
			get { return (SchedulePart.IsTechnicalStop); }
			set
			{
				if (SchedulePart.IsTechnicalStop != value)
				{
					SchedulePart.IsTechnicalStop = value;
					OnPropertyChanged(nameof(TehStop));
				}
			}
		}

		/// <summary>
		/// Скорость на участке от предыдущего куска расписания с заданным временем, до текущего.
		/// В виде строки и с округлением до целого значения в км/ч.
		/// </summary>
		public string Speed
		{
			get
			{
				double? speed = SchedulePart.SpeedOnSection;

				if (speed.HasValue && SchedulePart.IsInPath)
				{
					int speed_int = (int)(speed + 0.5);
					return (speed_int.ToString());
				}
				return (ConstValues.NotSetValueOneMinus);
			}
		}

		/// <summary>
		/// Признак, что расписание на участке корректно. True - значит OK.
		/// Определяется по значению скорости от предыдущего куска с заданным расписанием до текущего куска.
		/// Если на текущем куске время прибытия не задано, то возвращает True.
		/// </summary>
		public bool IsSchedulePartOK
		{
			get { return (SchedulePart.IsSchedulePartOK); }
		}

		/// <summary>
		/// Время прибытия не NULL. Используется для режима редактирования суток прибытия.
		/// </summary>
		public bool ArrivalTimeHasValue
		{
			get { return ArrivalTime.HasValue; }
		}

		/// <summary>
		/// Время отправления не NULL. Используется для режима редактирования суток отправления.
		/// </summary>
		public bool DepartureTimeHasValue
		{
			get { return DepartureTime.HasValue; }
		}

		/// <summary>
		/// Признак, показывающий отсутствие времён на обязательной станции.
		/// </summary>
		public bool IsTrainChangeHasTimes
		{
			get
			{
				return SchedulePart.IsTrainChangeHasTimes;
			}
		}

		#endregion Properties

		#region Constructors

		public TrainSchedulePartViewModel(SchedulePart schedPart, TrainScheduleViewModel schedViewModel)
		{
			SchedulePart = schedPart;
			WholeScheduleViewModel = schedViewModel;

			DisplayName = StationName;
		}

		#endregion Constructors

		#region Private Helpers

		private int CalcDays(DateTime curDate, DateTime trainStartDate)
		{
			return ((curDate - trainStartDate).Days + 1);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Часто после редактирования надо изменить несколько свойств.
		/// Поэтому обновляем все, чтоб не заморачиваться. 
		/// </summary>
		public void NotifyAllPropsChanged()
		{
			OnPropertyChanged(String.Empty);
		}

		#endregion Methods

	}
}
