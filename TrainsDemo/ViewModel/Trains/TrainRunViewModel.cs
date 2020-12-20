using System;
using TrainsDemo.Model.IntermediateData;
using TrainsDemo.ViewModel.Commands;

namespace TrainsDemo.ViewModel.Trains
{
	/// <summary>
	/// View Model для отображения рейса поезда
	/// </summary>
	public class TrainRunViewModel : ViewModelBase
	{
		#region Fields

		private int passCarsCount = 0;
		private int carsBriefCount = 0;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Класс со всеми командами.
		/// </summary>
		public ICommands Commands { get; private set; }

		/// <summary>
		/// View Model поезда
		/// </summary>
		public TrainViewModel TrainViewModel { get; private set; }

		/// <summary>
		/// Ссылка на модель рейса поезда
		/// </summary>
		public TrainRun TrainRun { get; private set; }

		/// <summary>
		/// Дата отправления рейса
		/// </summary>
		public DateTime Date
		{
			get { return (TrainRun.Date); }
		}

		/// <summary>
		/// Позиция строки в списке
		/// </summary>
		public int PositionInList { get; set; }

		/// <summary>
		/// Количество перевезенных пассажиров для данного рейса поезда
		/// </summary>
		public int TransportedPassCount
		{
			get
			{
				return TrainRun.TransportedPassCount;
			}
		}

		/// <summary>
		/// Количество неслужебных вагонов для данного рейса поезда
		/// </summary>
		public int PassCarsCount
		{
			get
			{
				if (passCarsCount == 0)
				{
				}
				return passCarsCount;
			}
		}

		/// <summary>
		/// Количество служебных вагонов для данного рейса поезда
		/// </summary>
		public int CarsBriefCount
		{
			get
			{
				if (carsBriefCount == 0)
				{
				}
				return carsBriefCount;
			}
		}

		#endregion Properties

		#region Constructors

		public TrainRunViewModel(TrainRun r, TrainViewModel trainVM)
		{
			TrainRun = r;
			DisplayName = TrainRun.Date.ToShortDateString();
			TrainViewModel = trainVM;
			Commands = trainVM.Commands;
		}

		#endregion Constructors
	}
}
