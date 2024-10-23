using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaDemo.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace AvaloniaDemo.Views
{
	public partial class DataDemoView : UserControl
	{
		public DataDemoView()
		{
			ViewModel = Ioc.Default.GetRequiredService<DataDemoViewModel>();
			DataContext = ViewModel;
			InitializeComponent();
		}
		public DataDemoViewModel ViewModel { get; private set; }
		private void TreeDataGrid_OnDoubleTapped(object sender, TappedEventArgs e)
		{
			if (sender is TreeDataGrid grid && grid.DataContext is DataDemoViewModel) {
			}
		}
		private void OnTreeDataGridKeyDown(object? sender, KeyEventArgs e)
		{
			if (sender is TreeDataGrid grid && grid.DataContext is DataDemoViewModel) {
			}
		}
	}
}
