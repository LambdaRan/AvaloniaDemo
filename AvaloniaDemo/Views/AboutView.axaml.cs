using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views
{
	public partial class AboutView : UserControl
	{
		public AboutView()
		{
			ViewModel = Ioc.Default.GetRequiredService<AboutViewModel>();
			DataContext = ViewModel;
			InitializeComponent();
		}

		public AboutViewModel ViewModel { get; private set; }


	}
}
