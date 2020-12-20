using Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainsDemo.Model.IntermediateData;

namespace TrainsDemo.ViewModel.Trains
{
	/// <summary>
	/// View Model для отображения расписания варианта поезда
	/// </summary>
	public class TrainScheduleViewModel : ViewModelBase
	{
		#region Fields

		private TrainSchedulePartViewModel selectedPart;
		// Модифицированная ObservableCollection с ViewModelями строк расписания.
		private ExtendedObservableCollection<TrainSchedulePartViewModel> scheduleParts;

		private string confirmScheduleCommandText;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Признак, стоит ли искать в базе существующий вариант поезда.
		/// После любого редактирования принимет значение False и как бы говорит нажать на кнопку "Сверить с базой данных".
		/// </summary>
		public bool FoundSimilarDB
		{
			get { return TrainViewModel.TrainVariant.FoundSimilarDB; }
			set
			{
				if (TrainViewModel.TrainVariant.FoundSimilarDB != value)
				{
					TrainViewModel.TrainVariant.FoundSimilarDB = value;
					OnPropertyChanged(nameof(FoundSimilarDB));
				}
			}
		}

		/// <summary>
		/// Ссылка на ViewModel варианта поезда
		/// </summary>
		public TrainViewModel TrainViewModel { get; private set; }

		/// <summary>
		/// Возвращает модифицированную ObservableCollection с ViewModelями строк расписания.
		/// </summary>
		public ExtendedObservableCollection<TrainSchedulePartViewModel> ScheduleParts
		{
			get
			{
				if (scheduleParts == null)
				{
					List<TrainSchedulePartViewModel> partsList = new List<TrainSchedulePartViewModel>(TrainViewModel.TrainVariant.Schedule.ScheduleParts.Count);

					foreach (SchedulePart part in TrainViewModel.TrainVariant.Schedule.ScheduleParts)
					{
						TrainSchedulePartViewModel newModel = new TrainSchedulePartViewModel(part, this);
						partsList.Add(newModel);
						newModel.PositionInList = partsList.Count;
					}
					scheduleParts = new ExtendedObservableCollection<TrainSchedulePartViewModel>(partsList);
				}
				return (scheduleParts);
			}
		}

		/// <summary>
		/// выделенная строка расписания поезда.
		/// </summary>
		public TrainSchedulePartViewModel SelectedPart
		{
			get { return (selectedPart); }
			set
			{
				if (selectedPart != value)
				{
					// ---- убираем признак с предыдущей ----
					if (selectedPart != null)
					{
						selectedPart.IsSelected = false;
					}

					selectedPart = value;

					// ---- устанавливаем признак на текущей ----
					if (selectedPart != null)
					{
						selectedPart.IsSelected = true;
					}

					OnPropertyChanged(nameof(SelectedPart));
				}
			}
		}

		/// <summary>
		/// Полное время хода. Если расписание не определено, то NULL
		/// </summary>
		public TimeSpan? FullRunTime
		{
			get { return (TrainViewModel.TrainVariant.Schedule.FullRunTime); }
		}

		/// <summary>
		/// Длина маршрута. Если маршрут не задан, то 0.
		/// </summary>
		public int Length
		{
			get { return (TrainViewModel.TrainVariant.Schedule.Length); }
		}

		/// <summary>
		/// Маршрутная скорость, в км/ч.
		/// </summary>
		public int Speed
		{
			get { return (int)(TrainViewModel.TrainVariant.Schedule.Speed + 0.5); }
		}

		/// <summary>
		/// Возвращает ViewModel первого куска расписания.
		/// Если расписание пустое, то возвращает NULL.
		/// </summary>
		public TrainSchedulePartViewModel FirstPart
		{
			get
			{
				if (scheduleParts != null)
				{
					if (scheduleParts.Count > 0)
					{
						return (scheduleParts[0]);
					}
				}
				return (null);
			}
		}

		/// <summary>
		/// Признак одобренного пользователем расписания. Пропускаются ошибки расписания, 
		/// кроме отсутствующего расписания и отрицательных скоростей.
		/// </summary>
		public bool ConfirmAllSchedulePartsOK
		{
			get { return TrainViewModel.TrainVariant.Schedule.ConfirmAllSchedulePartsOK; }
			set
			{
				if (TrainViewModel.TrainVariant.Schedule.ConfirmAllSchedulePartsOK != value)
				{
					if (!value || (value && TrainViewModel.TrainVariant.Schedule.IsRouteExist && TrainViewModel.
						TrainVariant.Schedule.ScheduleParts.Skip(1).All(p => p.SpeedOnSection > 0)))
					{
						TrainViewModel.TrainVariant.Schedule.ConfirmAllSchedulePartsOK = value;
						SetConfirmScheduleCommandText(value);
						if (value)
						{
							TrainViewModel.IsAllSchedulePartsOK = true;
							TrainViewModel.IsScheduleOK = true;
						}
						else
						{
							TrainViewModel.IsAllSchedulePartsOK = TrainViewModel.TrainVariant.Schedule.CheckIsAllSchedulePartsOK();
							TrainViewModel.IsScheduleOK = TrainViewModel.TrainVariant.Schedule.CheckIsScheduleOK();
						}
						OnPropertyChanged(nameof(ConfirmAllSchedulePartsOK));
					}
					else
					{
						TrainViewModel.IsAllSchedulePartsOK = TrainViewModel.TrainVariant.Schedule.CheckIsAllSchedulePartsOK();
						TrainViewModel.IsScheduleOK = TrainViewModel.TrainVariant.Schedule.CheckIsScheduleOK();
					}
				}
			}
		}

		/// <summary>
		/// Текст команды, устанавливающей признак одобренного пользователем расписания.
		/// </summary>
		public string ConfirmScheduleCommandText
		{
			get { return confirmScheduleCommandText; }
			set
			{
				if (confirmScheduleCommandText != value)
				{
					confirmScheduleCommandText = value;
					OnPropertyChanged(nameof(ConfirmScheduleCommandText));
				}
			}
		}

		#endregion Properties

		#region Constructors

		public TrainScheduleViewModel(TrainViewModel c)
		{
			DisplayName = "Расписание";//Strings.TrainScheduleWindow_DisplayName;
			TrainViewModel = c;
			SetConfirmScheduleCommandText(ConfirmAllSchedulePartsOK);
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Очищает Viewmodel'и и посылает событие об изменении
		/// </summary>
		public void Clear()
		{
			SelectedPart = null;
			scheduleParts = null;
			OnPropertyChanged(string.Empty);
		}

		/// <summary>
		/// Обновить текст команды, устанавливающей признак одобренного пользователем расписания.
		/// </summary>
		public void SetConfirmScheduleCommandText(bool confirmAllSchedulePartsOK)
		{
			if (confirmAllSchedulePartsOK)
			{
				ConfirmScheduleCommandText = "";//Strings.TrainScheduleConfirmSetFalse_CommandText;
			}
			else
			{
				ConfirmScheduleCommandText = "";//Strings.TrainScheduleConfirmSetTrue_CommandText;
			}
		}

		/// <summary>
		/// Выделяет строку в расписании по номеру
		/// </summary>
		public void SelectPart(int pos)
		{
			selectedPart = ScheduleParts.FirstOrDefault(p => p.PositionInList == pos);
		}

		/// <summary>
		/// Возвращает элемент расписания по коду из "Экспресс"
		/// </summary>
		public TrainSchedulePartViewModel GetPartByCodeExpress(int codeExpress)
		{
			foreach (TrainSchedulePartViewModel part in ScheduleParts)
			{
				if (part.CodeExpress == codeExpress)
				{
					return part;
				}
			}
			return null;
		}

		/// <summary>
		/// Обновляет все куски расписания, т.к. после редактирования одного куска часто надо обновить несколько.
		/// </summary>
		public void NotifyPropsChangedForSchedule()
		{
			if (scheduleParts != null)
			{
				foreach (TrainSchedulePartViewModel partVM in scheduleParts)
				{
					partVM.NotifyAllPropsChanged();
				}
			}

			int selectedTabIndex = TrainViewModel.SelectedTabIndex;
			TrainViewModel.RiseAllPropertiesChanged();
			TrainViewModel.SelectedTabIndex = selectedTabIndex;
		}

		#endregion Methods
	}
}
