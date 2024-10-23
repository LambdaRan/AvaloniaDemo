using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace AvaloniaDemo.Services
{
	public sealed class BusyDialog
	{
		private BusyDialogView? _Dialog;

		public void Show()
		{
			_Dialog = _Dialog ?? new BusyDialogView();
			Dispatcher.UIThread.Invoke(() => {
				_Dialog.ShowActivated = true;
				_Dialog.ShowInTaskbar = false;
				if (App.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
					_Dialog.ShowDialog(desktop.MainWindow!);
				}
			});
		}
		public void Close()
		{
			Dispatcher.UIThread.Invoke(() => {
				_Dialog?.Close();
				_Dialog = null;
			});
		}
		public void Hide() 
		{
			Dispatcher.UIThread.Invoke(() => {
				_Dialog?.Hide();
			});
		}
	}
}
