using Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TrainsDemo.Model;
using TrainsDemo.Model.IntermediateData;
using TrainsDemo.Properties;
using TrainsDemo.ViewModel.Commands;
using TrainsDemo.ViewModel.MainWindow;

namespace TrainsDemo.ViewModel.Trains
{
	/// <summary>
	/// ViewModel для первой вкладки с промежуточными поездами.
	/// </summary>
	public class TrainsListViewModel : ViewModelBase
	{
		#region Fields

		private MainModel mainModel;

		private MainWindowViewModel importWindowViewModel;

		// Индекс для быстрого поиска ViewModelи поезда по ID варианта поезда.
		private Dictionary<int, TrainViewModel> trainViewModelIndex = new Dictionary<int, TrainViewModel>();

		private TrainViewModel selectedTrainVariant;

		private string loadButtonText;

		private string loadButtonTooltipText;

		private bool showPairs;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Признак активной загрузки
		/// </summary>
		public bool IsLoading
		{
			get { return importWindowViewModel.IsLoading; }
		}

		/// <summary>
		/// Объединять поезда в пары
		/// </summary>
		public bool ShowPairs
		{
			get { return showPairs; }
			set
			{
				if (showPairs != value && !importWindowViewModel.IsLoading)
				{
					showPairs = value;
					DefaultSort();
					RefreshMarksAndComments(null, true);
					//снимаем и ставим выделение, что бы сработал ScrollToSelected
					TrainViewModel selected = SelectedTrainVariant;
					SelectedTrainVariant = null;
					SelectedTrainVariant = selected;
				}
			}
		}

		/// <summary>
		/// Возвращает модифицированную ObservableCollection с ViewModelями поездов.
		/// </summary>
		public ExtendedObservableCollection<TrainViewModel> Trains { get; private set; }

		/// <summary>
		/// Возвращает копию ObservableCollection типа ICollectionView (используется для сортировки по полям)
		/// </summary>
		public ICollectionView TrainCollectionView { get; private set; }

		/// <summary>
		/// Класс со всеми командами
		/// </summary>
		public ICommands Commands { get; private set; }

		/// <summary>
		/// Выделенный в списке вариант
		/// </summary>
		public TrainViewModel SelectedTrainVariant
		{
			get { return (selectedTrainVariant); }
			set
			{
				if (selectedTrainVariant != value)
				{
					selectedTrainVariant = value;
					OnPropertyChanged(nameof(SelectedTrainVariant));
				}
			}
		}

		/// <summary>
		/// Текст кнопки "Скачать расписание/Остановить загрузку"
		/// </summary>
		public string LoadButtonText
		{
			get { return loadButtonText; }
			set
			{
				if (loadButtonText != value)
				{
					loadButtonText = value;
					OnPropertyChanged(nameof(LoadButtonText));
				}
			}
		}

		/// <summary>
		/// Текст всплывающей подсказки кнопки "Скачать расписание/Остановить загрузку"
		/// </summary>
		public string LoadButtonTooltipText
		{
			get { return loadButtonTooltipText; }
			set
			{
				if (loadButtonTooltipText != value)
				{
					loadButtonTooltipText = value;
					OnPropertyChanged(nameof(LoadButtonTooltipText));
				}
			}
		}

		/// <summary>
		/// Команда скачивания расписания. Создаётся в ImportWindowViewModel. Сюда передаётся ссылка на неё, для удобства binding'а.
		/// </summary>
		public ICommand GetScheduleFromCorrespCommand { get; set; }

		#endregion Properties

		#region Constructors

		public TrainsListViewModel(MainModel im, ICommands com, MainWindowViewModel importWndVM)
		{
			base.DisplayName = Strings.TrainsWindow_DisplayName;
			mainModel = im;
			importWindowViewModel = importWndVM;
			Commands = com;
			showPairs = true;
			LoadButtonText = "Скачать расписание";//Strings.ButtonText_LoadSchedule;
			LoadButtonTooltipText = "Скачать расписание";//Strings.ButtonTooltipText_LoadSchedule;

			Trains = new ExtendedObservableCollection<TrainViewModel>();
			TrainCollectionView = CollectionViewSource.GetDefaultView(Trains);

			RefreshTrains();
			DefaultSort();
			RefreshMarks(new SortedDictionary<int, Tuple<int, TrainViewModel>>());
			foreach (TrainViewModel trainViewModel in Trains)
			{
				trainViewModel.IsScheduleOK = trainViewModel.TrainVariant.Schedule.CheckIsScheduleOK();
				trainViewModel.IsAllSchedulePartsOK = trainViewModel.TrainVariant.Schedule.CheckIsAllSchedulePartsOK();
			}
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Обновляет список ViewModeleй поездов
		/// Проходит по всем поездам в importer'e, проверяет, добавлена ли его ViewModel, если нет, добавляет.
		/// </summary>
		public HashSet<TrainViewModel> RefreshTrains()
		{
			HashSet<TrainViewModel> recentlyAdded = new HashSet<TrainViewModel>();

			List<TrainViewModel> newViewModels = new List<TrainViewModel>(Trains);

			foreach (Train train in mainModel.Trains)
			{
				TrainViewModel curViewModel = null;
				foreach (TrainVariant variant in train.Variants)
				{
					if (!trainViewModelIndex.TryGetValue(variant.ID, out curViewModel))
					{
						curViewModel = new TrainViewModel(train, variant, Commands, importWindowViewModel, mainModel);
						newViewModels.Add(curViewModel);
						trainViewModelIndex.Add(variant.ID, curViewModel);

						recentlyAdded.Add(curViewModel);
					}
				}
			}

			if (Trains.Count != newViewModels.Count)
			{
				if (Trains.Count != 0)
				{
					for (int i = 0; i < newViewModels.Count; i++)
					{
						TrainViewModel trainViewModel = newViewModels[i];

						if (recentlyAdded.Contains(trainViewModel))
						{
							Trains.Insert(i, trainViewModel);
						}
					}
				}
				else
				{
					Trains.Reset(newViewModels);
				}
			}

			return recentlyAdded;
		}

		/// <summary>
		/// Проставляем признаки для отображения
		/// </summary>
		/// <param name="trainViewModel">параметр (null если групповая операция)</param>
		public void RefreshMarksAndComments(TrainViewModel trainViewModel, bool updateComments)
		{

			SortedDictionary<int, Tuple<int, TrainViewModel>> newViewModelsIndex = new SortedDictionary<int, Tuple<int, TrainViewModel>>();

			// --- проставляем признаки ----
			if (!TrainCollectionView.IsEmpty)
			{
				//чётность, позиции, признак последнего варианта
				RefreshMarks(newViewModelsIndex);

				//комментарии
				if (updateComments)
				{
					//RefreshComments(newViewModelsIndex, trainViewModel);
				}
			}
		}

		/// <summary>
		/// Проставляет чётность, позиции, признак последнего варианта
		/// </summary>
		private void RefreshMarks(SortedDictionary<int, Tuple<int, TrainViewModel>> newViewModelsIndex)
		{
			int i = 1;
			bool isFirst = true;
			TrainViewModel prevTrVM = null;
			Parity curParity = Parity.Odd;// --- чётность первого поезда ---
			foreach (object obj in TrainCollectionView)
			{
				TrainViewModel curTrVM = obj as TrainViewModel;

				if (isFirst)
				{
					curTrVM.TrainParity = curParity;
					curTrVM.PositionInList = i;
					curTrVM.LastTrainVariantInPair = false;
					curTrVM.LastTrainVariantInTrain = false;
					newViewModelsIndex.Add(curTrVM.TrainVariant.ID, Tuple.Create(1, curTrVM));
					isFirst = false;
				}
				else
				{
					//обновляём признаки
					prevTrVM.LastTrainVariantInPair = false;
					prevTrVM.LastTrainVariantInTrain = false;

					curTrVM.PositionInList = i;

					newViewModelsIndex.Add(curTrVM.TrainVariant.ID, Tuple.Create(i, curTrVM));

					//  --- сменился поезд ---
					if (curTrVM.Train != prevTrVM.Train)
					{
						if (ShowPairs)
						{
							// --- признак последнего варианта в паре ---
							if (curTrVM.Train.TrainPair.ID != prevTrVM.Train.TrainPair.ID)
							{
								// --- меняем четность ---
								curParity = curParity.Invert();
								prevTrVM.LastTrainVariantInPair = true;
							}
							else
							{
								prevTrVM.LastTrainVariantInPair = false;
							}

							// --- признак последнего варианта поезде ---
							if (!prevTrVM.LastTrainVariantInPair)
							{
								prevTrVM.LastTrainVariantInTrain = true;
							}
						}
						else
						{
							// --- меняем четность ---
							curParity = curParity.Invert();
							prevTrVM.LastTrainVariantInTrain = true;
						}
					}
					else
					{
						prevTrVM.LastTrainVariantInTrain = false;
						prevTrVM.LastTrainVariantInPair = false;
					}

					// --- четность очередного поезда---
					curTrVM.TrainParity = curParity;
				}
				prevTrVM = curTrVM;
				i++;
			}
		}

		/// <summary>
		/// Начальная сортировка (после команды "Загрузить")
		/// </summary>
		public void DefaultSort()
		{
			TrainCollectionView.SortDescriptions.Clear();

			if (ShowPairs)
			{
				TrainCollectionView.SortDescriptions.Add(new SortDescription(ConstValues.Pair_aTrainNumber, ListSortDirection.Ascending));
				TrainCollectionView.SortDescriptions.Add(new SortDescription(ConstValues.Pair_aTrainInitStationName, ListSortDirection.Ascending));
				TrainCollectionView.SortDescriptions.Add(new SortDescription(ConstValues.Pair_aTrainFinStationName, ListSortDirection.Ascending));
			}

			TrainCollectionView.SortDescriptions.Add(new SortDescription(ConstValues.Number, ListSortDirection.Ascending));
			TrainCollectionView.SortDescriptions.Add(new SortDescription(ConstValues.InitStation, ListSortDirection.Ascending));
			TrainCollectionView.SortDescriptions.Add(new SortDescription(ConstValues.FinStation, ListSortDirection.Ascending));

			TrainCollectionView.Refresh();
		}

		/// <summary>
		/// Очистка ViewModel'ей всех поездов и вариантов 
		/// </summary>
		public void ClearTrains()
		{
			trainViewModelIndex.Clear();
			Trains.Clear();
		}

		/// <summary>
		/// Удалить View Model
		/// </summary>
		/// <param name="idList">ID</param>
		public void DeleteTrainViewModels(List<int> idList)
		{
			foreach (int id in idList)
			{
				Trains.Remove(trainViewModelIndex[id]);
				trainViewModelIndex.Remove(id);
			}
		}

		/// <summary>
		/// Возвращает список всех моделей вариантов переданного поезда
		/// </summary>
		public List<TrainViewModel> GetTrainViewModelsByTrain(Train trainParam)
		{
			//Список всех вариантов переданного поезда
			List<TrainViewModel> otherTrainVariants = null;
			otherTrainVariants = new List<TrainViewModel>();

			foreach (TrainVariant trainVariant in trainParam.Variants)
			{
				otherTrainVariants.Add(trainViewModelIndex[trainVariant.ID]);
			}

			return otherTrainVariants;
		}

		/// <summary>
		/// Возвращает ViewModel варианта поезда по ID варианта поезда
		/// </summary>
		public TrainViewModel GetTrainViewMobelByTrainVariantID(int id)
		{
			TrainViewModel trainViewModel = null;
			if (trainViewModelIndex.TryGetValue(id, out trainViewModel))
			{
				return trainViewModel;
			}
			return trainViewModel;
		}

		#endregion Methods
	}
}
