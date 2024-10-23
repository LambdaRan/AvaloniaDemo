using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo;

public partial class SettingView : UserControl
{
    public SettingView()
    {
		ViewModel = Ioc.Default.GetRequiredService<SettingViewModel>();
		DataContext = ViewModel;
		InitializeComponent();
    }
	public SettingViewModel ViewModel { get; private set; }
}