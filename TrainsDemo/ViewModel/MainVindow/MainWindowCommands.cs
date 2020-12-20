using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TrainsDemo.Model;
using TrainsDemo.ViewModel.Commands;
using TrainsDemo.ViewModel.MainWindow;
using TrainsDemo.ViewModel.Trains;

namespace TrainsDemo.ViewModel.MainVindow
{
	public class MainWindowCommands : ICommands
	{
		private MainWindowViewModel windowViewModel;
		private MainModel mainModel;

		public MainWindowCommands(MainWindowViewModel wnd, MainModel mainModel)
		{
			windowViewModel = wnd;
			this.mainModel = mainModel;
		}

		RelayCommand loadDataCommand;

		public ICommand LoadDataCommand
		{
			get
			{
				if (loadDataCommand == null)
					loadDataCommand = new RelayCommand(param => OnLoadData());

				return loadDataCommand;
			}
		}

		RelayCommand clearDataCommand;

		public ICommand ClearDataCommand
		{
			get
			{
				if (clearDataCommand == null)
					clearDataCommand = new RelayCommand(param => OnClearLoadData());

				return clearDataCommand;
			}
		}

		RelayCommand loadTrainScheduleCommand;

		public ICommand LoadTrainScheduleCommand
		{
			get
			{
				if (loadTrainScheduleCommand == null)
					loadTrainScheduleCommand = new RelayCommand(param => OnLoadTrainSchedule());

				return loadTrainScheduleCommand;
			}
		}

		private void OnLoadData()
		{
			if (windowViewModel.IsLoading)
				return;

			InfoGenerator infoGenerator = new InfoGenerator(mainModel);
			Dispatcher dispatcher = Application.Current.Dispatcher;

			// ---  загрузка ---
			Task task1 = new Task(() =>
			{
				windowViewModel.IsLoading = true;
				infoGenerator.LoadTrains();

				//обновление отображаемого списка групп (могли появиться новые варианты)
				dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
				{
					windowViewModel.TrainsListViewModel.RefreshTrains();
					windowViewModel.TrainsListViewModel.RefreshMarksAndComments(null, true);
					windowViewModel.RisePropertyChanged();
				});

				windowViewModel.IsLoading = false;
			});

			task1.Start();
		}

		private void OnClearLoadData()
		{
			if (windowViewModel.IsLoading || !windowViewModel.IsDataLoaded)
				return;

			//--- очистка всего  ---
			windowViewModel.TrainsListViewModel.ClearTrains();
			mainModel.ClearAllData();
			windowViewModel.RisePropertyChanged();
		}

		private void OnLoadTrainSchedule()
		{
			if (windowViewModel.IsLoading)
			{
				windowViewModel.TrainsListViewModel.LoadButtonText = "Скачать расписания";
				OnStopGetSchedule();
				return;
			}

			// ---  загрузка ---
			var res = Task.Factory.StartNew(() =>
			{
				TaskLoadSchedules();
			});
			res.ContinueWith(task => 
			{
				TaskLoadSchedulesFinishing();
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void TaskLoadSchedules()
		{
			InfoGenerator infoGenerator = new InfoGenerator(mainModel);
			windowViewModel.IsLoading = true;
			windowViewModel.ProgressBarVisibility = true;
			windowViewModel.OperationProgressDescription = "Загрузка расписаний.";
			windowViewModel.TrainsListViewModel.LoadButtonText = "Остановить загрузку";
			int trainsCount = windowViewModel.TrainsListViewModel.Trains.Count;

			//загрузка расписаний
			for (int i = 0; i < trainsCount; i++)
			{
				if (windowViewModel.StopLoading)
				{
					break;
				}

				TrainViewModel trainViewModel = windowViewModel.TrainsListViewModel.Trains[i];
				if (trainViewModel.TrainVariant.Schedule.ScheduleParts.Count > 0)
				{
					continue;
				}

				//обновление
				windowViewModel.ProgressBarStatus = (int)((double)i / trainsCount * 100);
				trainViewModel.IsLoading = true;
				Thread.Sleep(500);//имитация отклика сервера
				infoGenerator.LoadSchedule(trainViewModel.TrainVariant);
				trainViewModel.RiseAllPropertiesChanged();
				trainViewModel.IsLoading = false;
			}
		}

		private void TaskLoadSchedulesFinishing()
		{
			Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				windowViewModel.TrainsListViewModel.RefreshTrains();
				windowViewModel.TrainsListViewModel.RefreshMarksAndComments(null, true);
				windowViewModel.RisePropertyChanged();
			});

			windowViewModel.IsLoading = false;
			windowViewModel.ProgressBarVisibility = false;
			windowViewModel.OperationProgressDescription = string.Empty;
			if (windowViewModel.StopLoading)
			{
				windowViewModel.StopLoading = false;
			}
		}

		private void OnStopGetSchedule()
		{
			if (windowViewModel.IsLoading)
			{
				windowViewModel.StopLoading = true;
			}
		}
	}
}
