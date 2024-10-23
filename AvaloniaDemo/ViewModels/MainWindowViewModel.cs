using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using AvaloniaDemo.Services;
using AvaloniaDemo.Src;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase<MainWindowViewModel>
	{
		private readonly AppConfig _AppCfg;
		public MainWindowViewModel(ICommonServices<MainWindowViewModel> commonServices, AppConfig cfg)
			: base(commonServices) 
		{
			SelectedItem = _Items[0];
			_AppCfg = cfg;
		}

		public override void Subscribe()
		{
			base.Subscribe();
			MessageService.Register<MainWindowViewModel, StatusMessage>(this, (r, m) => {
				Dispatcher.UIThread.Post(() => {
					r.IsError = m.IsError;
					r.StatusMessage = m.Status;
					if (m.IsToLog) {
						r.AddLog(m.Status);
					}
				});
			});
			MessageService.Register<MainWindowViewModel, Notification>(this, (r, m) => {
				Dispatcher.UIThread.Post(() => { r.NotificationManager?.Show(m); });
			});
			MessageService.Register<MainWindowViewModel, LogMessage>(this, (r, m) => {
				Dispatcher.UIThread.Post(() => { r.AddLog(m.Log); });
			});
		}
		public override void Unsubscribe()
		{
			base.Unsubscribe();
		}

		#region Navigation
		private List<NavigationItem> _Items = new List<NavigationItem>()
		{
			new NavigationItem("Demo", typeof(DataDemoViewModel)),
			new NavigationItem("设置", typeof(SettingViewModel)),
			new NavigationItem("关于", typeof(AboutViewModel)),
		};
		public IEnumerable<NavigationItem> Items 
		{
			get => _Items.AsEnumerable();
		}

		[ObservableProperty]
		private object _SelectedItem;

		[ObservableProperty]
		private object? _CurrentPage;

		public void Navigate(NavigationItem item)
		{
			CurrentPage = NavigationService.GetViewInstance(item.ViewModel);
			//NotificationManager?.Show(new Notification("SwitchPage", $"当前页面是:{item.Label}", NotificationType.Information));
		}
		#endregion

		#region Status
		[ObservableProperty]
		private bool _IsError = false;

		[ObservableProperty]
		private string _StatusMessage = "Ready";
		#endregion

		#region Log
		public int MaxLogLine => 100;
		public TextEditor? TextEditor { get; set; }

		public void AddLog(string message)
		{
			if (TextEditor is null) {
				return;
			}
			TextEditor.AppendText($"{string.Format("{0:HH:mm:ss}", DateTime.Now)} {message}\r\n");
			if (!RemoveLog()) {
				TextEditor.ScrollToLine(TextEditor.LineCount - 3);
			}
		}
		private bool RemoveLog()
		{
			var textArea = TextEditor?.TextArea;
			if (textArea?.Document is null) {
				return false;
			}
			if (textArea.Document.LineCount < MaxLogLine) {
				return false;
			}
			int firstLineIndex = 1;
			int lastLineIndex = 1;
			var startLine = textArea.Document.GetLineByNumber(firstLineIndex);
			var endLine = textArea.Document.GetLineByNumber(lastLineIndex);
			textArea.Selection = Selection.Create(textArea, startLine.Offset, endLine.Offset + endLine.TotalLength);
			textArea.Selection.ReplaceSelectionWithText(string.Empty);
			TextEditor?.ScrollToLine(TextEditor.LineCount - 3);
			return true;
		}
		#endregion

		#region Notification
		public WindowNotificationManager? NotificationManager { get; set; }
		#endregion

		// 显示公告
		public async void AutoShowNoticeWndAsync()
		{
			if (_AppCfg.HasReadNotice()) {
				return;
			}
			_AppCfg.SetHasReadNotice();
			DialogHost.Show<NoticeViewModel>();
			await Task.Delay(TimeSpan.FromSeconds(5));
			DialogHost.Close();
		}
	}
}
