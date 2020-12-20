namespace TrainsDemo
{
	public static class ConstValues
	{
		/// <summary>
		/// Стрококвая константа для обозначения незаданого значения. В виде одного минуса.
		/// </summary>
		public static readonly string NotSetValueOneMinus = " - ";

		/// <summary>
		/// Стандартное кол-во дней поверх конечной даты импорта для рейсов поездов в рейсах беспересадочных групп.
		/// </summary>
		public static readonly int DefaultAdditionalDaysCount = 8;

		/// <summary>
		/// Минимальное кол-во дней поверх конечной даты импорта для рейсов поездов в рейсах беспересадочных групп.
		/// </summary>
		public static readonly int MinAdditionalDaysCount = 0;

		/// <summary>
		/// Максимальное кол-во дней поверх конечной даты импорта для рейсов поездов в рейсах беспересадочных групп.
		/// </summary>
		public static readonly int MaxAdditionalDaysCount = 18;

		/// <summary>
		/// Минимальная маршрутная скорость, при которой расписание считается корректным.(км/ч)
		/// </summary>
		public static readonly double MinRouteSpeed = 10;

		/// <summary>
		/// Максимальная маршрутная скорость, при которой расписание считается корректным. (км/ч)
		/// </summary>
		public static readonly double MaxRouteSpeed = 300;

		#region Константы свойств для сортировки

		/// <summary>
		/// Приставка к названию свойства сортировки при включенной опции сортировки "Сохранять пары" или "Группы вагонов"
		/// </summary>
		public static readonly string MaxSortDescriptionPrefix = "Max";

		/// <summary>
		/// Название колонки позиции в списке для свойства сортировки
		/// </summary>
		public static readonly string PositionInList = "PositionInList";

		/// <summary>
		/// Путь к свойству номера прямого поезда пары для сортировки
		/// </summary>
		public static readonly string Pair_aTrainNumber = "TrainPair.aTrain.TrainKey.Number";

		/// <summary>
		/// Путь к свойству названия начальной станции прямого поезда пары для сортировки
		/// </summary>
		public static readonly string Pair_aTrainInitStationName = "TrainPair.aTrain.InitStationName";

		/// <summary>
		/// Путь к свойству названия конечной станции прямого поезда пары для сортировки
		/// </summary>
		public static readonly string Pair_aTrainFinStationName = "TrainPair.aTrain.FinStationName";

		/// <summary>
		/// Название свойства номера для сортировки
		/// </summary>
		public static readonly string Number = "Number";

		/// <summary>
		/// Название свойства начальной станции для сортировки
		/// </summary>
		public static readonly string InitStation = "Train.InitStationName";

		/// <summary>
		/// Название свойства конечной станции для сортировки
		/// </summary>
		public static readonly string FinStation = "Train.FinStationName";

		/// <summary>
		/// Название свойства для номера первого поезда группы
		/// </summary>
		public static readonly string CarGroupNitkaTrainNumber = "TrainNumber";

		#endregion Константы свойств для сортировки
	}
}
