using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainsDemo.Model.IntermediateData
{
	/// <summary>
	/// Промежуточный класс "вариант поезда". Объединает рейсы, следующие по одному расписанию.
	/// </summary>
	public class TrainVariant
	{
		#region Fields

		// Для генерации ID
		private static int idGenerator = 0;

		// Список рейсов поездов. Ключ - Дата в DataTime с очищенной временнОй частью. Это дата отправления рейса.
		private SortedList<DateTime, TrainRun> trainRuns = new SortedList<DateTime, TrainRun>();

		private Schedule schedule;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Сгенерировать новый отрицательный ID
		/// </summary>
		public static int IdGenerator
		{
			get
			{
				idGenerator--;
				return idGenerator;
			}
		}

		/// <summary>
		/// ID варианта поезда, для вариантов поездов до прокладывания маршрута ID < 0
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Описание варианта поезда
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Описание периодичности хождения варианта поезда
		/// </summary>
		public string PeriodDescription { get; set; }

		/// <summary>
		/// Ссылка на поезд, в котором содержится этот вариант
		/// </summary>
		public Train Train { get; private set; }

		/// <summary>
		/// График, к которому относится данный вариант поезда.
		/// </summary>
		public TrainGraphic.Graphic Graphic { get; private set; }

		/// <summary>
		/// Время отправления варианта с начальной станции. TimeSpan - Время прошедшее с полночи.
		/// </summary>
		public TimeSpan InitTime { get; set; }

		/// <summary>
		/// Возвращает список рейсов, упорядоченных по дате отправления.
		/// </summary>
		public IList<TrainRun> TrainRuns
		{
			get { return (trainRuns.Values); }
		}

		/// <summary>
		/// Расписание варианта поезда
		/// </summary>
		public Schedule Schedule
		{
			get { return (schedule); }
		}

		/// <summary>
		/// Комментарий к проложенному/найденному маршруту
		/// </summary>
		public string ScheduleByRouteComment { get; set; }

		/// <summary>
		/// Признак того, что маршрут варианта поезда показан на карте
		/// </summary>
		public bool ShowRouteOnMap { get; set; }

		/// <summary>
		/// Количество перевезенных пассажиров
		/// </summary>
		public int TransportedPassCount
		{
			get
			{
				int transportedPassCount = 0;
				foreach (TrainRun trainRun in TrainRuns)
				{
					transportedPassCount += trainRun.TransportedPassCount;
				}

				return transportedPassCount;
			}
		}

		/// <summary>
		/// Количество неслужебных вагонов
		/// </summary>
		public int PassCarsCount
		{
			get
			{
				int passCarsCount = 0;
				foreach (TrainRun trainRun in TrainRuns)
				{
					passCarsCount += trainRun.PassCarsCount;
				}

				return passCarsCount;
			}
		}

		/// <summary>
		/// Количество служебных вагонов
		/// </summary>
		public int CarsBriefCount
		{
			get
			{
				int carsBriefCount = 0;
				foreach (TrainRun trainRun in TrainRuns)
				{
					carsBriefCount += trainRun.CarsBriefCount;
				}

				return carsBriefCount;
			}
		}

		/// <summary>
		/// Признак, стоит ли искать в базе существующий вариант поезда.
		/// После любого редактирования принимет значение False и как бы говорит нажать на кнопку "Сверить с базой данных".
		/// </summary>
		public bool FoundSimilarDB { get; set; }

		public string Comment { get; set; }

		#endregion Properties

		#region Constructors

		public TrainVariant(Train _train, TrainGraphic.Graphic _graphic, TimeSpan initTime)
		{
			// --- создаём новый ID (отрицательный, положительный будет присвоен при прокладывании маршрута) ---
			ID = --idGenerator;

			Train = _train;
			Graphic = _graphic;
			InitTime = initTime;

			schedule = new Schedule();

			//TrainType = Graphic.GetTrainTypeByNumber(_train.TrainKey.Number);

			Description = string.Empty;

			PeriodDescription = string.Empty;

			FoundSimilarDB = true;
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Добавление рейса
		/// </summary>
		public bool TryAddRun(TrainRun trRun)
		{
			if (trRun != null)
			{
				if (!trainRuns.ContainsKey(trRun.Date))
				{
					trainRuns.Add(trRun.Date, trRun);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Удаляет все рейсы поездов
		/// </summary>
		public void ClearRuns()
		{
			trainRuns.Clear();
		}

		/// <summary>
		/// Добавляет описание периодичности
		/// </summary>
		/// <param name="runDates">Список дат рейсов</param>
		public void SetPeriodDescription(List<DateTime> runDates)
		{
			string longPeriodDescription = string.Empty;
				//TextDescriptionPeriodicityGenerator.Instance.
				//TextDecsriptionPeriodicityEditor(runDates, Graphic.BeginDate, Graphic.EndDate);

			PeriodDescription = longPeriodDescription.Split('.').First() + ".";
		}

		#endregion Methods
	}
}
