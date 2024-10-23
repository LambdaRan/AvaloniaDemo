using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo;

public partial class NoticeView : UserControl
{
    public NoticeView()
    {
		ViewModel = Ioc.Default.GetRequiredService<NoticeViewModel>();
		DataContext = ViewModel;
		InitializeComponent();
    }
	public NoticeViewModel ViewModel { get; private set; }
}