using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrainsDemo.Model;
using TrainsDemo.Properties;
using TrainsDemo.ViewModel.Commands;
using TrainsDemo.ViewModel.MainVindow;
using TrainsDemo.ViewModel.Trains;

namespace TrainsDemo.ViewModel.MainWindow
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		#region Fields

		private int selectedMainTabIndex;

		private bool isLoading;

		private bool isDataLoading;

		private bool stopLoading = false;

		private string operationStatus = Strings.Status_Ready;

		private string operationProgressDescription = String.Empty;

		// главный объект, содержащий операции и данные
		private MainModel mainModel = null;

		/// <summary>
		/// Содержит все команды.
		/// </summary>
		private ICommands commands = null;

		/// <summary>
		/// Список ViewModelей для вкладок TabControla.
		/// </summary>
		private ReadOnlyCollection<ViewModelBase> tabsViewModels = null;

		/// <summary>
		/// Дублирование для удобства ViewModelи поездов. Чтобы не искать в списке tabsViewModels.
		/// </summary>
		private TrainsListViewModel trainsListViewModel = null;

		private int progressBarStatus;

		private bool progressBarVisibility;

		private string remainingTime;

		#endregion Fields

		#region Properties

		public TrainsListViewModel TrainsListViewModel
		{
			get { return trainsListViewModel; }
		}

		public DateTime BeginDate
		{
			get { return (mainModel.BeginDate); }
			set
			{
				if (mainModel.BeginDate != value)
				{
					mainModel.BeginDate = value;
					// --- обновляем все даты, т.к. внутри mainModel'а могли поменяться все ----
					OnPropertyChanged(nameof(BeginDate));
					OnPropertyChanged(nameof(EndDate));
				}
			}
		}

		public DateTime EndDate
		{
			get { return (mainModel.EndDate); }
			set
			{
				if (mainModel.EndDate != value)
				{
					mainModel.EndDate = value;
					// --- обновляем все даты, т.к. внутри mainModel'а могли поменяться все ---
					OnPropertyChanged(nameof(EndDate));
					OnPropertyChanged(nameof(BeginDate));
				}
			}
		}

		public int AdditionalDays
		{
			get { return (mainModel.AdditionalDays); }
			set
			{
				if (mainModel.AdditionalDays != value)
				{
					mainModel.AdditionalDays = value;
					OnPropertyChanged(nameof(AdditionalDays));
				}
			}
		}

		public int SelectedMainTabIndex 
		{
			get { return selectedMainTabIndex; } 
			set
			{
				if(selectedMainTabIndex != value)
				{
					selectedMainTabIndex = value;
					OnPropertyChanged(nameof(SelectedMainTabIndex));
				}
			}
		}

		/// <summary>
		/// --- Признак того, что происходит ЛЮБАЯ загрузка из базы. ---
		/// --- Этот признак устанавливать всегда. 
		/// --- Если нужно для какой-то загрузки завести свой особый признак, то устанавливать оба признака одновременно.
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
				}
			}
		}

		/// <summary>
		/// ---- Признак загрузки поездов (самая первая загрузка) ----
		/// ---- Для установки свойства IsEnabled у кнопки "Скачать расписание/Остановить загрузку" ----
		/// </summary>
		public bool IsDataLoading
		{
			get { return isDataLoading; }
			set
			{
				if (isDataLoading != value)
				{
					isDataLoading = value;
					OnPropertyChanged(nameof(IsDataLoading));
				}
			}
		}

		/// <summary>
		/// ---- Признак прерывания загрузки всех расписаний и групп вагонов ----
		/// ---- Если True, то остановить загрузку ----
		/// </summary>
		public bool StopLoading
		{
			get { return stopLoading; }
			set
			{
				if (stopLoading != value)
				{
					stopLoading = value;
					OnPropertyChanged(nameof(StopLoading));
				}
			}
		}

		/// <summary>
		/// Свойство для проверки того, что есть загруженные данные ---
		/// </summary>
		public bool IsDataLoaded
		{
			get { return (mainModel.Trains.Count > 0 ? true : false); }
		}

		/// <summary>
		/// Свойство для отображения статуса в нижней строке. ---
		/// </summary>
		public string OperationStatus
		{
			get { return (operationStatus); }
			set
			{
				if (operationStatus != value)
				{
					operationStatus = value;
					OnPropertyChanged(nameof(OperationStatus));
				}
			}
		}

		/// <summary>
		/// Свойство для отображения прогресса операции в виде предложения
		/// </summary>
		public string OperationProgressDescription
		{
			get { return (operationProgressDescription); }
			set
			{
				if (operationProgressDescription != value)
				{
					operationProgressDescription = value;
					OnPropertyChanged(nameof(OperationProgressDescription));
				}
			}
		}

		/// <summary>
		/// Значение прогресса выполнения
		/// </summary>
		public int ProgressBarStatus
		{
			get { return progressBarStatus; }
			set
			{
				if (progressBarStatus != value)
				{
					progressBarStatus = value;
					OnPropertyChanged(nameof(ProgressBarStatus));
				}
			}
		}

		/// <summary>
		/// Свойство видимости
		/// </summary>
		public bool ProgressBarVisibility
		{
			get { return progressBarVisibility; }
			set
			{
				if (progressBarVisibility != value)
				{
					progressBarVisibility = value;
					OnPropertyChanged(nameof(ProgressBarVisibility));
				}
			}
		}

		/// <summary>
		/// Оставшееся время выполнения операции
		/// </summary>
		public string RemainingTime
		{
			get { return remainingTime; }
			set
			{
				if (remainingTime != value)
				{
					remainingTime = value;
					OnPropertyChanged(nameof(RemainingTime));
				}
			}
		}

		/// <summary>
		/// Возвращает ReadOnly список ViewModeleй для отображения во вкладках TabControla
		/// </summary>
		public ReadOnlyCollection<ViewModelBase> TabsViewModels
		{
			get
			{
				if (tabsViewModels == null)
				{
					List<ViewModelBase> modelsList = CreateTabsViewModels();
					tabsViewModels = new ReadOnlyCollection<ViewModelBase>(modelsList);
				}
				return (tabsViewModels);
			}
		}

		public ICommands Commands
		{
			get
			{
				if (commands == null)
				{
					commands = new MainWindowCommands(this, mainModel);
				}
				return (commands);
			}
		}

		#endregion Properties

		#region Constructors

		public MainWindowViewModel()
		{
			mainModel = new MainModel();
			base.DisplayName = "Поезда";
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Создаёт список ViewModeleй для отображения во вкладках TabControla
		/// </summary>
		/// <returns></returns>
		List<ViewModelBase> CreateTabsViewModels()
		{
			List<ViewModelBase> rez = new List<ViewModelBase>();

			// --- View Model for Intermediate Trains ---
			trainsListViewModel = new TrainsListViewModel(mainModel, Commands, this);
			rez.Add(trainsListViewModel);

			// --- View Model for Something else ---

			return (rez);
		}

		public void RisePropertyChanged()
		{
			OnPropertyChanged(string.Empty);
		}

		#endregion Methods
	}
}