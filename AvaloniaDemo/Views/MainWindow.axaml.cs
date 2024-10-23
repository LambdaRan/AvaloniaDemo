using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaEdit.Editing;
using AvaloniaEdit;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.ViewModels;
using System;
using AvaloniaDemo.Services;
using AvaloniaDemo.Src;
using CommunityToolkit.Diagnostics;
using Avalonia.Controls.Notifications;

namespace AvaloniaDemo.Views
{
	public partial class MainWindow : Window
	{
		private readonly TextEditor? _textEditor;
		public MainWindow()
		{
			ViewModel = Ioc.Default.GetRequiredService<MainWindowViewModel>();
			DataContext = ViewModel;
			Loaded += OnWindowLoaded;
			Unloaded += OnWindowUnloaded;
			Closing += OnWindowClosing;
			InitializeComponent();

			_textEditor = this.FindControl<TextEditor>("Editor");
			Guard.IsNotNull(_textEditor);
			_textEditor.Options.EnableEmailHyperlinks = false;
			_textEditor.Options.EnableTextDragDrop = false;
			_textEditor.Options.CutCopyWholeLine = false;
			_textEditor.Options.EnableTextDragDrop = false;
			_textEditor.Options.EnableImeSupport = false;

			_textEditor.IsReadOnly = true;
		}
		public MainWindowViewModel ViewModel { get; private set; }

		private void OnWindowLoaded(object? sender, RoutedEventArgs e)
		{
			ViewModel.NotificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(this));
			ViewModel.TextEditor = _textEditor;
			ViewModel.Subscribe();
			ViewModel.AutoShowNoticeWndAsync();
		}
		private void OnWindowUnloaded(object? sender, RoutedEventArgs e)
		{
			ViewModel.Unsubscribe();
			ViewModel.MessageService.UnregisterAll(this);
			ViewModel.NotificationManager = null;
			ViewModel.TextEditor = null;
		}
		private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
		{
		}
		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as ListBox)!.SelectedItem is NavigationItem item) {
				ViewModel.Navigate(item);
			}
		}
	}
}