using System.Windows.Input;

namespace TrainsDemo.ViewModel.Commands
{
	public interface ICommands
	{
		/// <summary>
		/// Загрузить начальные данные из базы ExpressImport.
		/// </summary>
		ICommand LoadDataCommand { get; }

		/// <summary>
		/// Очистка всех данных в памяти
		/// </summary>
		ICommand ClearDataCommand { get; }
	}
}
