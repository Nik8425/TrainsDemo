using Extension;
using System.Collections.Generic;
using System.Linq;
using TrainsDemo.Model.IntermediateData;

namespace TrainsDemo.ViewModel.Trains
{
	/// <summary>
	/// View Model для отображения списка рейсов варианта поезда
	/// </summary>
	public class TrainRunsListViewModel : ViewModelBase
	{
		#region Fields

		// Модифицированная ObservableCollection с ViewModelями Рейсов.
		private ExtendedObservableCollection<TrainRunViewModel> trainRuns = null;
		private TrainRunViewModel selectedTrainRun;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Ссылка на модель варианта поезда
		/// </summary>
		public TrainVariant TrainVariant { get; private set; }

		/// <summary>
		/// Ссылка на ViewModel варианта поезда
		/// </summary>
		public TrainViewModel TrainViewModel { get; private set; }

		/// <summary>
		/// Возвращает модифицированную ObservableCollection с ViewModelями Рейсов.
		/// </summary>
		public ExtendedObservableCollection<TrainRunViewModel> TrainRuns
		{
			get
			{
				if (trainRuns == null)
				{
					List<TrainRunViewModel> runsList = new List<TrainRunViewModel>(TrainVariant.TrainRuns.Count);
					foreach (TrainRun run in TrainVariant.TrainRuns)
					{
						TrainRunViewModel newModel = new TrainRunViewModel(run, TrainViewModel);
						runsList.Add(newModel);
						newModel.PositionInList = runsList.Count;
					}
					trainRuns = new ExtendedObservableCollection<TrainRunViewModel>(runsList);
				}
				return (trainRuns);
			}
		}

		/// <summary>
		/// выделенный в списке рейс поезда.
		/// </summary>
		public TrainRunViewModel SelectedTrainRun
		{
			get { return (selectedTrainRun); }
			set
			{
				if (selectedTrainRun != value)
				{
					selectedTrainRun = value;
					OnPropertyChanged(nameof(SelectedTrainRun));
				}
			}
		}

		#endregion Properties

		#region Constructors

		public TrainRunsListViewModel(TrainVariant trVarParam, TrainViewModel trainVM)
		{
			DisplayName = "Рейсы";
			TrainVariant = trVarParam;
			TrainViewModel = trainVM;
			selectedTrainRun = TrainRuns.First();
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Очищает Viewmodel'и и посылает событие об изменении
		/// </summary>
		public void Clear()
		{
			SelectedTrainRun = null;
			trainRuns = null;
			OnPropertyChanged(string.Empty);
		}

		#endregion Methods
	}
}
