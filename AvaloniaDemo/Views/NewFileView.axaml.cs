using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.Services;
using AvaloniaDemo.ViewModels;
using System;
using System.Reflection.Emit;

namespace AvaloniaDemo;

public partial class NewFileView : UserControl
{
    public NewFileView()
    {
		ViewModel = Ioc.Default.GetRequiredService<NewFileViewModel>();
		DataContext = ViewModel;
		InitializeComponent();
    }

	public NewFileViewModel ViewModel { get; private set; }
}