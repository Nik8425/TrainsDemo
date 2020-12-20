using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TrainsDemo.Properties;

namespace TrainsDemo.Model
{
	public partial class MainModel
	{
		#region Fields

		// начальная дата импорта
		private DateTime beginDate = new DateTime(2000, 1, 1);

		// конечная дата импорта
		private DateTime endDate = new DateTime(2000, 1, 31);

		// кол-во дополнительных дней импорта для беспересадочных групп вагонов
		private int additionalDays = ConstValues.DefaultAdditionalDaysCount;

		//FIX ME: перенести сюда список станций
		//// словарь станций, Key - codeExpress, Value - станция
		//private Dictionary<int, IStation> stations = new Dictionary<int, IStation>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// Начальная дата импорта
		/// </summary>
		public DateTime BeginDate
		{
			get { return (beginDate); }
			set
			{
				beginDate = value;
				if (endDate < beginDate)
				{
					endDate = beginDate;
				}
				SaveProps();
			}
		}

		/// <summary>
		/// Конечная дата импорта
		/// </summary>
		public DateTime EndDate
		{
			get { return (endDate); }
			set
			{
				endDate = value;
				if (beginDate > endDate)
				{
					beginDate = endDate;
				}
				SaveProps();
			}
		}

		/// <summary>
		/// Кол-во дополнительных дней импорта для беспересадочных групп вагонов
		/// </summary>
		public int AdditionalDays
		{
			get { return (additionalDays); }
			set
			{
				additionalDays = value;
				if (additionalDays < ConstValues.MinAdditionalDaysCount || additionalDays > ConstValues.MaxAdditionalDaysCount)
				{
					additionalDays = ConstValues.DefaultAdditionalDaysCount;
				}
				SaveProps();
			}
		}

		#endregion Properties

		#region Constructors

		public MainModel()
		{
			LoadProps();
			InfoGenerator info = new InfoGenerator(this);
		}

		#endregion Constructors

		//methods

		#region Global Methods

		/// <summary>
		/// Очистка всех данных
		/// </summary>
		public void ClearAllData()
		{
			trains.Clear();
			trainPairs.Clear();
		}

		#endregion Global Methods

		#region SaveLoadPropsInFile

		/// <summary>
		/// Загружаем из файла сохранённые настройки
		/// </summary>
		void LoadProps()
		{
			string CurDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\";
			CurDir += "Settings" + "\\";
			string curFileName = CurDir + Strings.SettingSavePropsFile;

			if (File.Exists(curFileName))
			{
				try
				{
					using (FileStream file = new FileStream(curFileName, FileMode.Open, FileAccess.Read))
					{
						BinaryFormatter binFormatter = new BinaryFormatter();
						beginDate = (DateTime)binFormatter.Deserialize(file);
						endDate = (DateTime)binFormatter.Deserialize(file);
						additionalDays = (int)binFormatter.Deserialize(file);
					}
				}
				catch (Exception)
				{
					//FIX ME: лог приложения
				}
			}
		}

		/// <summary>
		/// Сохраняем в файл настройки
		/// </summary>
		void SaveProps()
		{
			try
			{
				string CurDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\";
				CurDir += "Settings" + "\\";
				if (!Directory.Exists(CurDir))
				{
					Directory.CreateDirectory(CurDir);
				}

				string curFileName = CurDir + Strings.SettingSavePropsFile;

				using (FileStream file = new FileStream(curFileName, FileMode.OpenOrCreate))
				{
					BinaryFormatter binFormatter = new BinaryFormatter();
					binFormatter.Serialize(file, BeginDate);
					binFormatter.Serialize(file, EndDate);
					binFormatter.Serialize(file, additionalDays);
				}
			}
			catch (Exception)
			{
				//FIX ME: лог приложения
			}
		}

		#endregion SaveLoadPropsInFile
	}
}

