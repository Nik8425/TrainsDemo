using System;
using System.Windows;
using TrainsDemo.ViewModel.MainWindow;

namespace TrainsDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
