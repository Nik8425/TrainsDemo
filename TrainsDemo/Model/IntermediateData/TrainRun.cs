using System;

namespace TrainsDemo.Model.IntermediateData
{
	/// <summary>
	/// Промежуточный класс "рейс поезда".
	/// Содержит список TrainRow
	/// </summary>
	public class TrainRun
	{
		#region Fields

		// Для генерации ID
		private static int idGenerator = 0;

		#endregion Fields

		#region Properties

		/// <summary>
		/// ID рейса варианта поезда, для новых поездов ID = -1
		/// </summary>
		public int ID { get; set; }

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
		/// Дата отправления рейса. Временная часть в DateTime должна быть обнулена.
		/// </summary>
		public DateTime Date { get; private set; }

		/// <summary>
		/// True, если рейс из будущего, т.е не вошел во временной отрезок импорта и нужен только для поездов беспересадочных групп.
		/// </summary>
		public bool IsFuture { get; set; }

		/// <summary>
		/// Количество перевезенных пассажиров
		/// </summary>
		public int TransportedPassCount
		{
			get
			{
				int transportedPassCount = 0;
				return transportedPassCount;
			}
		}

		/// <summary>
		/// Количество пассажирских вагонов
		/// </summary>
		public int PassCarsCount
		{
			get
			{
				int passCarsCount = 0;
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
				return carsBriefCount;
			}
		}

		/// <summary>
		/// Является ли рейс неправильным и лишним, т.е не нужно ли его импортировать. Если True, то этот рейс формально
		/// есть, но нигде не участвует. По нему не создаются рейсы групп, не учитываются пересечения с другими поездами
		/// в этот день, не учитываются ошибки в корреспонденциях, т.к этот рейс не будет импортирован.
		/// Значение устанавливается пользователем, если он не соответствует периодичности и в нём нет пассажиров.
		/// </summary>
		public bool DoNotImport { get; set; }

		#endregion Properties

		#region Constructor

		public TrainRun(DateTime date)
		{
			// --- создаём новый ID (отрицательный, положительный будет присвоен при прокладывании маршрута) ---
			ID = --idGenerator;
			Date = date.Date;
		}

		#endregion Constructor
	}
}
