namespace TrainsDemo
{
	/// <summary>
	/// Признак для обозначения прибытия или отправления
	/// </summary>
	public enum DepArr { Departure, Arrive }

	/// <summary>
	/// Признак статуса расписания
	/// </summary>
	public enum ScheduleStatus { NotExits, WithErrors, Good }

	/// <summary>
	/// Признак загрузки
	/// </summary>
	public enum LoadStatus { Loading, Viewing }

	/// <summary>
	/// Для обозначения чётный, нечётный
	/// </summary>
	public enum Parity { Even, Odd }
}
