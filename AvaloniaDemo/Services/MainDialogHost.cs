using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.Services
{
	// todo  加上返回值
	public sealed class MainDialogHost
	{
		private MainDialog? _Dialog;

		public void Show<TViewModel>(string title = "")
		{
			_Dialog = _Dialog ?? new MainDialog();
			_Dialog.Title = title;
			_Dialog.Closed += OnWindowClosed;
			Dispatcher.UIThread.Invoke(() => {
				_Dialog.ShowActivated = true;
				_Dialog.Content = NavigationService.GetViewInstance(typeof(TViewModel));
				if (App.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
					_Dialog.ShowDialog(desktop.MainWindow!);
				}
			});
		}

		private void OnWindowClosed(object? sender, EventArgs e)
		{
			_Dialog = null;
		}

		public void Close()
		{
			Dispatcher.UIThread.Invoke(() => {
				_Dialog?.Close();
				_Dialog = null;
			});
		}
	}
}
