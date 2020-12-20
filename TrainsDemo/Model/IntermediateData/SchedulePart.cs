using System;

namespace TrainsDemo.Model.IntermediateData
{
	/// <summary>
	/// Элемент расписания
	/// </summary>
	public class SchedulePart
	{
		#region Fields

		private DateTime? arrivalTimeWithShift;
		private DateTime? departureTimeWithShift;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Код экспресс станции.
		/// </summary>
		public int CodeExpress { get; set; }

		/// <summary>
		/// Название станции
		/// </summary>
		public string StationName { get; set; }

		/// <summary>
		/// Время прибытия на станцию. Если не задано, то null.
		/// </summary>
		public DateTime? ArrivalTime { get; set; }

		/// <summary>
		/// Время отправления со станции. Если не задано, то null.
		/// </summary>
		public DateTime? DepartureTime { get; set; }

		/// <summary>
		/// Временной сдвиг (в минутах)
		/// </summary>
		public short TimeShift { get; set; }

		/// <summary>
		/// Время прибытия на станцию с учетом временного сдвига. Если не задано, то null.
		/// </summary>
		public DateTime? ArrivalTimeWithShift
		{
			get
			{
				arrivalTimeWithShift = ArrivalTime;

				if (arrivalTimeWithShift.HasValue)
				{
					arrivalTimeWithShift = arrivalTimeWithShift.Value.AddMinutes(TimeShift);
				}

				return (arrivalTimeWithShift);
			}
		}

		/// <summary>
		/// Время отправления со станции с учетом временного сдвига. Если не задано, то null.
		/// </summary>
		public DateTime? DepartureTimeWithShift
		{
			get
			{
				departureTimeWithShift = DepartureTime;

				if (departureTimeWithShift.HasValue)
				{
					departureTimeWithShift = departureTimeWithShift.Value.AddMinutes(TimeShift);
				}

				return (departureTimeWithShift);
			}
		}

		/// <summary>
		/// Расстояние в километрах с от начала маршрута.
		/// </summary>
		public int DistanceKM { get; set; }

		/// <summary>
		/// Признак технической стоянки
		/// </summary>
		public bool IsTechnicalStop { get; set; }

		/// <summary>
		/// Ссылка на предыдущий кусок расписания.
		/// Для расчётных характеристик и проверки коректности расписания на участке.
		/// </summary>
		public SchedulePart PreviousPart { get; private set; }

		/// <summary>
		/// Ссылка на следующий кусок расписания.
		/// Для расчётных характеристик и проверки коректности расписания на участке.
		/// </summary>
		public SchedulePart NextPart { get; private set; }

		/// <summary>
		/// Участие станции в прокладывании маршрута
		/// </summary>
		public bool IsInPath { get; set; }

		/// <summary>
		/// Скорость на участке от предыдущего куска расписания с заданным временем отправления. в км/ч.
		/// Скорость вычисляется до текущего куска, если у него есть время прибытия. Если у текущего время не задано, то ищется следующий с заданным временем.
		/// Если предыдущего или следующего куска нет или не задано время, то возвращает null.
		/// </summary>
		public double? SpeedOnSection
		{
			get
			{
				// --- следующий кусок, у которого есть время прибытия
				SchedulePart nextSched = ArrivalTimeWithShift.HasValue ? this : GetNextPartWithArrival();

				if (nextSched != null)
				{
					SchedulePart prevSched = GetPreviousPartWithDeparture();

					if (prevSched != null)
					{
						double hours = (nextSched.ArrivalTimeWithShift.Value - prevSched.DepartureTimeWithShift.Value).TotalHours;
						double rast = nextSched.DistanceKM - prevSched.DistanceKM;

						return (rast / hours);
					}
				}
				return (null);
			}
		}

		/// <summary>
		/// Признак, что расписание на участке корректно. True - значит OK.
		/// Определяется по значению скорости от предыдущего куска с заданным расписанием до текущего куска.
		/// Если на текущем куске время прибытия не задано, то возвращает True.
		/// </summary>
		public bool IsSchedulePartOK
		{
			get
			{
				// ---- проверка времени стоянки ----
				if (DepartureTimeWithShift.HasValue && ArrivalTimeWithShift.HasValue)
				{
					if ((DepartureTimeWithShift.Value - ArrivalTimeWithShift.Value).TotalMinutes < 0)
						return (false);
				}

				if(PreviousPart == null && !DepartureTimeWithShift.HasValue)
				{
					return false;
				}

				// ---- проверка скорости ----
				double? speed = SpeedOnSection;
				if (speed.HasValue)
				{
					if (speed < ConstValues.MinRouteSpeed || speed > ConstValues.MaxRouteSpeed)
						return (false);
				}

				return (true);
			}
		}

		/// <summary>
		/// Признак станции прицепки. Станция прицепки означает, что она является начальной или конечной станцией группы вагонов,
		/// идущей с данным поездом.
		/// </summary>
		public bool IsTrainChange { get; set; }

		/// <summary>
		/// Признак, показывающий отсутствие времён на станции прицепки.
		/// </summary>
		public bool IsTrainChangeHasTimes
		{
			get
			{
				if (IsTrainChange)
				{
					if (PreviousPart != null && NextPart != null)
					{
						if (!ArrivalTime.HasValue || !DepartureTime.HasValue)
						{
							return false;
						}
					}
					else
					{
						if (PreviousPart == null)
						{
							if (!DepartureTime.HasValue)
							{
								return false;
							}
						}
						if (NextPart == null)
						{
							if (!ArrivalTime.HasValue)
							{
								return false;
							}
						}
					}
				}

				return true;
			}
		}

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="prevPart"></param>
		/// <param name="codeExpress"></param>
		/// <param name="stationName"></param>
		/// <param name="arrivalTime"></param>
		/// <param name="departureTime"></param>
		/// <param name="km"></param>
		/// <param name="tehStop"></param>
		public SchedulePart(SchedulePart prevPart, int codeExpress, string stationName,
							DateTime? arrivalTime, DateTime? departureTime, int km, bool tehStop)
		{
			// ---- проставляем ссылки на предыдущий и следующий кусок ---
			if (prevPart != null)
			{
				PreviousPart = prevPart;
				PreviousPart.NextPart = this;
			}

			CodeExpress = codeExpress;
			StationName = stationName;
			ArrivalTime = arrivalTime;
			DepartureTime = departureTime;
			DistanceKM = km;
			IsTechnicalStop = tehStop;
			IsInPath = true;
		}

		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Обновляет время прибытия на станцию.
		/// </summary>
		/// <param name="days">число дней</param>
		public void AddDaysToArrivalDate(int days)
		{
			if (ArrivalTime.HasValue)
			{
				ArrivalTime = ArrivalTime.Value.AddDays(days);
			}
		}

		/// <summary>
		/// Обновляет время отправления со станции.
		/// </summary>
		/// <param name="days">число дней</param>
		public void AddDaysToDepartureDate(int days)
		{
			if (DepartureTime.HasValue)
			{
				DepartureTime = DepartureTime.Value.AddDays(days);
			}
		}

		/// <summary>
		/// Ищет предыдущий кусок расписания, для которого задано время отправления и станция включена в маршрут
		/// </summary>
		/// <returns></returns>
		public SchedulePart GetPreviousPartWithDeparture()
		{
			SchedulePart rez = PreviousPart;
			while (rez != null)
			{
				if (rez.DepartureTimeWithShift.HasValue && rez.IsInPath)
					break;

				rez = rez.PreviousPart;
			}
			return (rez);
		}

		/// <summary>
		/// Ищет следующий кусок расписания, для которого задано время отправления и станция включена в маршрут
		/// </summary>
		/// <returns></returns>
		public SchedulePart GetNextPartWithArrival()
		{
			SchedulePart rez = NextPart;
			while (rez != null)
			{
				if (rez.ArrivalTimeWithShift.HasValue && rez.IsInPath)
					break;

				rez = rez.NextPart;
			}
			return (rez);
		}

		#endregion
	}
}
