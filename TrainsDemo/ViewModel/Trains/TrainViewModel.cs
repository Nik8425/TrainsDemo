using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TrainsDemo.Model;
using TrainsDemo.Model.IntermediateData;
using TrainsDemo.ViewModel.Commands;
using TrainsDemo.ViewModel.MainWindow;

namespace TrainsDemo.ViewModel.Trains
{
	/// <summary>
	/// ViewModel для поезда и Варианта поезда одновременно.
	/// Создаётся для каждого варианта, поэтому для одного поезда может быть создано несколько, по количеству его вариантов.
	/// </summary>
	public partial class TrainViewModel : ViewModelBase
	{
		#region Private Fields

		private MainModel mainModel;

		// Ссылка на ViewModel окна импорта
		private MainWindowViewModel importWindowViewModel;

		// Список ViewModelей для отображения свойств варианта поезда.
		private ReadOnlyCollection<ViewModelBase> variantViewModels = null;

		// Ссылка на ViewModel всего списка для удобства, что бы не обращаться через importWindowViewModel
		private TrainsListViewModel trainsListViewModel;

		// Дублирование для удобства ViewModelи рейсов. Чтобы не искать в списке variantViewModels.
		private TrainRunsListViewModel trainRunsListViewModel = null;

		// Дублирование для удобства ViewModelи вариантов. Чтобы не искать в списке variantViewModels.
		private TrainScheduleViewModel trainScheduleViewModel = null;

		private bool lastTrainVariantInTrain;

		private bool lastTrainVariantInPair;

		private int selectedTabIndex;

		private int positionInList = 0;

		private bool isLoading = false;

		#endregion Fields

		#region Properties

		/// <summary>
		/// ID варианта поезда
		/// </summary>
		public string ID
		{
			get
			{
				return TrainVariant.ID.ToString();
			}
		}

		/// <summary>
		/// ID поезда
		/// </summary>
		public string TrainID
		{
			get
			{
				return TrainVariant.Train.ID.ToString();
			}
		}

		/// <summary>
		/// Класс со всеми командами.
		/// </summary>
		public ICommands Commands { get; private set; }

		/// <summary>
		/// Возвращает статус по признаку IsLoading
		/// Если у варианта установлен признак IsLoading, статус Loading.
		/// </summary>
		public LoadStatus StateStatus
		{
			get
			{
				if (IsLoading)
				{
					return (LoadStatus.Loading);
				}
				else
				{
					return (LoadStatus.Viewing);
				}
			}
		}

		/// <summary>
		/// Признак, что сейчас происходот загрузка данных о варианте.
		/// Так же вызывает событие об изменении статуса варианта.
		/// </summary>
		public bool IsLoading
		{
			get { return (isLoading); }
			set
			{
				if (isLoading != value)
				{
					isLoading = value;
					OnPropertyChanged(nameof(IsLoading));
					OnPropertyChanged(nameof(StateStatus));
				}
			}
		}

		/// <summary>
		/// Ссылка на модель поезда.
		/// </summary>
		public Train Train { get; private set; }

		/// <summary>
		/// Ссылка на модель варианта поезда.
		/// </summary>
		public TrainVariant TrainVariant { get; private set; }

		/// <summary>
		/// Чётный/нечетный поезд. Для раскрашивания поездов в списке разными цветами.
		/// </summary>
		public Parity TrainParity { get; set; }

		/// <summary>
		/// Комментарий к проложенному/найденному маршруту
		/// </summary>
		public string ScheduleByRouteComment
		{
			get { return TrainVariant.ScheduleByRouteComment; }
		}

		/// <summary>
		/// График варианта поезда
		/// </summary>
		public TrainGraphic.Graphic Graphic
		{
			get { return (TrainVariant.Graphic); }
		}

		/// <summary>
		/// Комментарий
		/// </summary>
		public string Comment
		{
			get
			{
				if (TrainVariant.Comment != null)
				{
					return TrainVariant.Comment;
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Комментарий
		/// </summary>
		public bool CommentEnabled
		{
			get
			{
				if (TrainVariant.Comment == null)
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Признак, что это последний вариант в поезде. Нужно для визуального отделения вариантов разных поездов (не очень жирная горизонтальная линия)
		/// </summary>
		public bool LastTrainVariantInTrain
		{
			get { return lastTrainVariantInTrain; }
			set
			{
				if (lastTrainVariantInTrain != value)
				{
					lastTrainVariantInTrain = value;
					OnPropertyChanged(nameof(LastTrainVariantInTrain));
				}
			}
		}

		/// <summary>
		/// Признак, что это последний вариант в паре поездов. Нужно для визуального отделения вариантов разных разных (очень жирная горизонтальная линия)
		/// </summary>
		public bool LastTrainVariantInPair
		{
			get { return lastTrainVariantInPair; }
			set
			{
				if (lastTrainVariantInPair != value)
				{
					lastTrainVariantInPair = value;
					OnPropertyChanged(nameof(LastTrainVariantInPair));
				}
			}
		}

		/// <summary>
		/// Возвращает ReadOnly список ViewModeleй для отображения свойств варианта поезда.
		/// </summary>
		public ReadOnlyCollection<ViewModelBase> VariantViewModels
		{
			get
			{
				if (variantViewModels == null)
				{
					List<ViewModelBase> modelsList = CreateVariantsViewModels();
					variantViewModels = new ReadOnlyCollection<ViewModelBase>(modelsList);
				}
				return (variantViewModels);
			}
		}

		public int SelectedTabIndex
		{
			get { return selectedTabIndex; }
			set
			{
				if (selectedTabIndex != value)
				{
					selectedTabIndex = value;
					OnPropertyChanged(nameof(SelectedTabIndex));
				}
			}
		}

		/// <summary>
		/// Позиция строки в списке. 
		/// </summary>
		public int PositionInList
		{
			get { return positionInList; }
			set
			{
				if (positionInList != value)
				{
					positionInList = value;
					OnPropertyChanged(nameof(PositionInList));
				}
			}
		}

		/// <summary>
		/// --- Номер поезда ---
		/// </summary>
		public int Number
		{
			get { return (Train.TrainKey.Number); }
		}

		/// <summary>
		/// Код экспресс начальной станции поезда 
		/// </summary>
		public int InitStationExpress
		{
			get { return (Train.TrainKey.InitStationExpress); }
		}

		/// <summary>
		/// Код экспресс конечной станции поезда 
		/// </summary>
		public int FinStationExpress
		{
			get { return (Train.TrainKey.FinStationExpress); }
		}

		/// <summary>
		/// Название начальной станции поезда 
		/// </summary>
		public string InitStationName
		{
			get { return (Train.InitStationName); }
		}

		/// <summary>
		/// Название конечной станции поезда 
		/// </summary>
		public string FinStationName
		{
			get { return (Train.FinStationName); }
		}

		public TimeSpan InitTime
		{
			get { return (TrainVariant.InitTime); }
			set
			{
				if (TrainVariant.InitTime != value)
				{
					TrainVariant.InitTime = value;
					OnPropertyChanged(nameof(InitTime));
				}
			}
		}

		/// <summary>
		/// Признак наличия корректного расписания, без учета ошибок внутри расписания
		/// </summary>
		public bool IsScheduleOK
		{
			get { return (TrainVariant.Schedule.IsScheduleOK); }
			set
			{
				if (TrainVariant.Schedule.IsScheduleOK != value)
				{
					TrainVariant.Schedule.IsScheduleOK = value;
					OnPropertyChanged(nameof(IsScheduleOK));
					OnPropertyChanged(nameof(ScheduleStatus));
				}
			}
		}

		/// <summary>
		/// Признак наличия ошибок внутри расписания, в частях расписания
		/// </summary>
		public bool IsAllSchedulePartsOK
		{
			get { return (TrainVariant.Schedule.IsAllSchedulePartsOK); }
			set
			{
				if (TrainVariant.Schedule.IsAllSchedulePartsOK != value)
				{
					TrainVariant.Schedule.IsAllSchedulePartsOK = value;
					OnPropertyChanged(nameof(IsAllSchedulePartsOK));
					OnPropertyChanged(nameof(ScheduleStatus));
				}
			}
		}

		/// <summary>
		/// Количество рейсов
		/// </summary>
		public string RunsCount
		{
			get
			{
				return (TrainVariant.TrainRuns.Where(r => !r.IsFuture).Count().ToString() + " + " + TrainVariant.TrainRuns.Where(r => r.IsFuture).Count().ToString());
			}
		}

		/// <summary>
		/// Дата первого рейса
		/// </summary>
		public DateTime FirstRunDate
		{
			get
			{
				if (TrainVariant.TrainRuns.Count > 0)
					return (TrainVariant.TrainRuns[0].Date);
				else
					return (new DateTime());
			}
		}

		/// <summary>
		/// Дата последнего рейса
		/// </summary>
		public DateTime LastRunDate
		{
			get
			{
				if (TrainVariant.TrainRuns.Count > 0)
					return (TrainVariant.TrainRuns[TrainVariant.TrainRuns.Count - 1].Date);
				else
					return (DateTime.MinValue);
			}
		}

		/// <summary>
		/// Возвращает статус по признаку расписания IsSheduleOK и IsAllSchedulePartsOk
		/// </summary>
		public ScheduleStatus ScheduleStatus
		{
			get
			{
				if (IsScheduleOK && IsAllSchedulePartsOK)
					return (ScheduleStatus.Good);
				else if (!IsScheduleOK)
					return (ScheduleStatus.NotExits);
				else
					return (ScheduleStatus.WithErrors);
			}
		}

		/// <summary>
		/// Текст для строки контекстного меню "Объединить/Разбить пару поездов"
		/// </summary>
		public string MenuItemSetPairText
		{
			get
			{
				if (Train.TrainPair.bTrain != null)
				{
					return "Разбить пару"; //Strings.UnsetPair_MenuItem;
				}
				else
				{
					return "Объединить пару";//Strings.SetPair_MenuItem;
				}
			}
		}

		/// <summary>
		/// Количество перевезенных пассажиров для данного варианта поезда
		/// </summary>
		public int TransportedPassCount
		{
			get
			{
				return TrainVariant.TransportedPassCount;
			}
		}

		/// <summary>
		/// Количество неслужебных вагонов для данного варианта поезда
		/// </summary>
		public int PassCarsCount
		{
			get
			{
				return TrainVariant.PassCarsCount;
			}
		}

		/// <summary>
		/// Количество служебных вагонов для данного варианта поезда
		/// </summary>
		public int CarsBriefCount
		{
			get
			{
				return TrainVariant.CarsBriefCount;
			}
		}

		/// <summary>
		/// ID пары
		/// </summary>
		public string TrainPairID
		{
			get
			{
				return TrainVariant.Train.TrainPair.ID.ToString();
			}
		}

		/// <summary>
		/// Ссылка на пару
		/// </summary>
		public TrainPair TrainPair
		{
			get { return TrainVariant.Train.TrainPair; }
		}

		#endregion Properties

		#region Properties for sorting

		/// <summary>
		/// Максимальное значение номера поезда в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxNumber
		{
			get
			{
				return (TrainPair.aTrain.TrainKey.Number);
			}
		}

		/// <summary>
		/// Максимальное значение статуса расписания в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public ScheduleStatus MaxScheduleStatus
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.Schedule.ScheduleRating > maxVariant.Schedule.ScheduleRating)
					{
						maxVariant = trainVariant;
					}
				}

				if (TrainPair.bTrain != null)
				{
					foreach (TrainVariant trainVariant in TrainPair.bTrain.Variants.Skip(1))
					{
						if (trainVariant.Schedule.ScheduleRating > maxVariant.Schedule.ScheduleRating)
						{
							maxVariant = trainVariant;
						}
					}
				}

				return trainsListViewModel.GetTrainViewMobelByTrainVariantID(maxVariant.ID).ScheduleStatus;
			}
		}

		/// <summary>
		/// Максимальное значение названия начальной станции в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public string MaxInitStationName
		{
			get
			{
				return TrainPair.aTrain.InitStationName;
			}
		}

		/// <summary>
		/// Максимальное значение названия конечной станции в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public string MaxFinStationName
		{
			get
			{
				return TrainPair.aTrain.FinStationName;
			}
		}

		/// <summary>
		/// Максимальное значение времени отправления в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public TimeSpan MaxInitTime
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.InitTime.CompareTo(maxVariant.InitTime) < 0)
					{
						maxVariant = trainVariant;
					}
				}

				return maxVariant.InitTime;
			}
		}

		/// <summary>
		/// Количество рейсов (используется при сортировке)
		/// </summary>
		public int CountRuns
		{
			get { return TrainVariant.TrainRuns.Where(r => !r.IsFuture).Count(); }
		}

		/// <summary>
		/// Максимальное значение количества рейсов в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxCountRuns
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.TrainRuns.Count.CompareTo(maxVariant.TrainRuns.Count) < 0)
					{
						maxVariant = trainVariant;
					}
				}

				return maxVariant.TrainRuns.Where(r => !r.IsFuture).Count();
			}
		}

		/// <summary>
		/// Максимальное значение даты первого рейса в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public DateTime MaxFirstRunDate
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.TrainRuns.First().Date.CompareTo(maxVariant.TrainRuns.First().Date) < 0)
					{
						maxVariant = trainVariant;
					}
				}

				return maxVariant.TrainRuns.First().Date;
			}
		}

		/// <summary>
		/// Максимальное значение даты последнего рейса в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public DateTime MaxLastRunDate
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.TrainRuns.Last().Date.CompareTo(maxVariant.TrainRuns.Last().Date) < 0)
					{
						maxVariant = trainVariant;
					}
				}

				return maxVariant.TrainRuns.Last().Date;
			}
		}

		/// <summary>
		/// Максимальное значение количества перевезённых пассажиров в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxTransportedPassCount
		{
			get
			{
				int sum = 0;
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					sum += trainVariant.TransportedPassCount;
				}
				return sum;
			}
		}

		/// <summary>
		/// Максимальное значение количества пассажирских вагонов в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxPassCarsCount
		{
			get
			{
				int sum = 0;
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					sum += trainVariant.PassCarsCount;
				}

				return sum;
			}
		}

		/// <summary>
		/// Максимальное значение количества пассажирских вагонов в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxCarsBriefCount
		{
			get
			{
				int sum = 0;
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					sum += trainVariant.CarsBriefCount;
				}

				return sum;
			}
		}

		/// <summary>
		/// Максимальное значение ID поезда в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxTrainID
		{
			get
			{
				return TrainPair.aTrain.ID;
			}
		}

		/// <summary>
		/// Максимальное значение ID варианта в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public int MaxID
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.ID.CompareTo(maxVariant.ID) < 0)
					{
						maxVariant = trainVariant;
					}
				}

				return maxVariant.ID;
			}
		}

		/// <summary>
		/// Максимальное значение комментария к маршруту в паре (используется при сортировке с сохранением пар)
		/// </summary>
		public string MaxScheduleByRouteComment
		{
			get
			{
				TrainVariant maxVariant = TrainPair.aTrain.Variants[0];
				foreach (TrainVariant trainVariant in TrainPair.aTrain.Variants.Skip(1))
				{
					if (trainVariant.ScheduleByRouteComment.CompareTo(maxVariant.ScheduleByRouteComment) < 0)
					{
						maxVariant = trainVariant;
					}
				}

				if (TrainPair.bTrain != null)
				{
					foreach (TrainVariant trainVariant in TrainPair.bTrain.Variants.Skip(1))
					{
						if (trainVariant.ScheduleByRouteComment.CompareTo(maxVariant.ScheduleByRouteComment) < 0)
						{
							maxVariant = trainVariant;
						}
					}
				}

				return maxVariant.ScheduleByRouteComment;
			}
		}

		#endregion Properties for sorting

		#region Constructors

		public TrainViewModel(Train tr, TrainVariant trVar, ICommands com, MainWindowViewModel importWndVM, MainModel im)
		{
			Commands = com;
			importWindowViewModel = importWndVM;
			trainsListViewModel = importWndVM.TrainsListViewModel;
			mainModel = im;
			Train = tr;
			TrainVariant = trVar;

			DisplayName = Number.ToString() + " " + InitStationName + " - " + FinStationName;

			TrainParity = Parity.Odd;
			LastTrainVariantInTrain = false;
			LastTrainVariantInPair = false;
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Создаёт список ViewModeleй для отображения во вкладках свойств варианта поезда.
		/// </summary>
		private List<ViewModelBase> CreateVariantsViewModels()
		{
			List<ViewModelBase> rez = new List<ViewModelBase>();

			// --- View Model for Train Runs ---
			trainRunsListViewModel = new TrainRunsListViewModel(TrainVariant, this);
			rez.Add(trainRunsListViewModel);

			// --- View Model for Train Schedule ---
			trainScheduleViewModel = new TrainScheduleViewModel(this);
			rez.Add(trainScheduleViewModel);

			// --- View Model for Something else ---

			return (rez);
		}

		/// <summary>
		/// Посылаем событие об изменении всех свойств. Так же обновляем дочерние ViewModel'и.
		/// </summary>
		public void RiseAllPropertiesChanged()
		{
			int selectedTab = SelectedTabIndex;

			if (TrainVariant.Schedule != null)
			{
				if (!TrainVariant.Schedule.ConfirmAllSchedulePartsOK)
				{
					TrainVariant.Schedule.IsAllSchedulePartsOK = TrainVariant.Schedule.CheckIsAllSchedulePartsOK();
					TrainVariant.Schedule.IsScheduleOK = TrainVariant.Schedule.CheckIsScheduleOK();
				}
			}

			variantViewModels = null;
			OnPropertyChanged(String.Empty);

			SelectedTabIndex = selectedTab;
		}

		/// <summary>
		/// Оповещает об изменении свойства (обновить Binding)
		/// </summary>
		public void RisePropertyChanged(string propertyName)
		{
			VerifyPropertyName(propertyName);
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// Выделить этот вариант поезда в списке поездов
		/// </summary>
		public void SetSelectedThis()
		{
			importWindowViewModel.TrainsListViewModel.SelectedTrainVariant = this;
		}

		#endregion Methods
	}
}
