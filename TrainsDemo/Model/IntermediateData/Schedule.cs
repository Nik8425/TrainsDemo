using System;
using System.Collections.Generic;

namespace TrainsDemo.Model.IntermediateData
{
	/// <summary>
	/// Класс для хранения расписания промежуточных вариантов поездов и групп вагонов.
	/// Представляет собой перечень из SchedulePart.
	/// </summary>
	public class Schedule
	{
		#region Fields

		private List<SchedulePart> scheduleParts = new List<SchedulePart>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// Список всех кусков расписания
		/// </summary>
		public IList<SchedulePart> ScheduleParts
		{
			get { return (scheduleParts); }
		}

		/// <summary>
		/// Длина маршрута.
		/// Если число кусков расписания меньше 2, то возвращает 0.
		/// </summary>
		public int Length
		{
			get
			{
				if (scheduleParts.Count >= 2)
				{
					return (scheduleParts[scheduleParts.Count - 1].DistanceKM - scheduleParts[0].DistanceKM);
				}
				return (0);
			}
		}

		/// <summary>
		/// Возвращает полное время хода. в виде TimeSpan
		/// Если расписание не определено, то возвращает NULL.
		/// </summary>
		public TimeSpan? FullRunTime
		{
			get
			{
				if (IsRouteExist)
				{
					DateTime? departureTime = scheduleParts[0].DepartureTimeWithShift;
					DateTime? arrivalTime = scheduleParts[scheduleParts.Count - 1].ArrivalTimeWithShift;

					// --- проверка, что заданы времена отправления/прибытия --- 
					if (departureTime.HasValue && arrivalTime.HasValue)
					{
						return (arrivalTime.Value - departureTime.Value);
					}
				}
				return (null);
			}
		}

		/// <summary>
		/// Возвращает маршрутную скорость. в км/ч.
		/// Если расписание не определено, то возвращает 0.
		/// </summary>
		public double Speed
		{
			get
			{
				TimeSpan? runTime = FullRunTime;
				if (runTime.HasValue)
				{
					return ((double)Length / runTime.Value.TotalHours);
				}
				return (0);
			}
		}

		/// <summary>
		/// Проверка существования маршрута. 
		/// Возвращет True, если расстояние (свойство Length) между конечными станциями больше нуля.
		/// </summary>
		public bool IsRouteExist
		{
			get { return (Length > 0); }
		}

		/// <summary>
		/// Проверка корректности расписания.
		/// Чтобы вернулось True, должны быть времена отправления с первой станции и прибытия на последнюю.
		/// Скорость движения не должна быть больше или меньше заданных граничных величин.
		/// А так же IsRouteExist должен возвращать true.
		/// </summary>
		public bool IsScheduleOK { get; set; }

		/// <summary>
		/// Проверка корректности частей расписания.
		/// Возвращает True, если во всех частях OK
		/// </summary>
		public bool IsAllSchedulePartsOK { get; set; }

		/// <summary>
		/// Признак одобренного пользователем расписания. Пропускаются ошибки расписания, 
		/// кроме отсутствующего расписания и отрицательных скоростей.
		/// </summary>
		public bool ConfirmAllSchedulePartsOK { get; set; }

		/// <summary>
		/// Возвращает первый кусок расписания. Если расписвание пустое, то NULL
		/// </summary>
		public SchedulePart FirstPart
		{
			get
			{
				if (scheduleParts.Count > 0)
				{
					return (scheduleParts[0]);
				}
				return (null);
			}
		}

		/// <summary>
		/// Возвращает последний кусок расписания. Если расписвание пустое, то NULL
		/// </summary>
		public SchedulePart LastPart
		{
			get
			{
				if (scheduleParts.Count > 0)
				{
					return (scheduleParts[scheduleParts.Count - 1]);
				}
				return (null);
			}
		}

		/// <summary>
		/// Рейтинг расписания (используется для сортировки по столбцам)
		/// </summary>
		public byte ScheduleRating
		{
			get
			{
				byte rating = 0;
				if (IsScheduleOK)
				{
					rating++;
				}
				if (IsAllSchedulePartsOK)
				{
					rating++;
				}
				return rating;
			}
		}

		#endregion Properties

		#region Constructor

		public Schedule() { }

		#endregion Constructor

		#region Public Methods

		/// <summary>
		/// Ошибки внутри расписания
		/// </summary>
		public bool CheckIsAllSchedulePartsOK()
		{
			foreach (SchedulePart schedulePart in ScheduleParts)
			{
				if (!schedulePart.IsSchedulePartOK)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Ошибки расписания (маршрут существует, адекватная маршутная скорость)
		/// </summary>
		public bool CheckIsScheduleOK()
		{
			foreach (SchedulePart schedulePart in ScheduleParts)
			{
				//if (!schedulePart.IsTrainChangeHasTimes)
				//{
				//	return false;
				//}
			}

			if (IsRouteExist)
			{
				// --- проверка на скорость ---
				double speed = Speed;
				if (ConstValues.MinRouteSpeed <= speed && speed <= ConstValues.MaxRouteSpeed)
				{
					return (true);
				}
			}
			return (false);
		}

		/// <summary>
		/// Очистка всего расписания
		/// </summary>
		public void ClearSchedule()
		{
			scheduleParts.Clear();
		}

		/// <summary>
		/// Добавление в конец нового куска расписания с заданными свойствами.
		/// </summary>
		/// <param name="codeExpress">Код -экспресс станции</param>
		/// <param name="stationName">Название станции</param>
		/// <param name="arrivalTime">Время прибытия на станцию</param>
		/// <param name="departureTime">Время отправления со станции</param>
		/// <param name="km">Нарастающая длина маршрута до станции</param>
		/// <param name="tehStop">Признак технической стоянки.</param>
		/// <returns></returns>
		public SchedulePart AddNewPart(int codeExpress, string stationName, DateTime? arrivalTime, DateTime? departureTime,
									int km, bool tehStop)
		{
			SchedulePart newSchedulePart = new SchedulePart(LastPart, codeExpress, stationName, arrivalTime, departureTime, km,
												tehStop);
			scheduleParts.Add(newSchedulePart);

			return (newSchedulePart);
		}

		#endregion
	}
}
