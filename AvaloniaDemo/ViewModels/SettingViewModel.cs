using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaloniaDemo.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
	public partial class SettingViewModel : ViewModelBase<SettingViewModel>
	{
		public SettingViewModel(ICommonServices<SettingViewModel> commonServices)
			: base(commonServices)
		{
		}

		[ObservableProperty]
		public string? _RootPath;
		partial void OnRootPathChanged(string? value)
		{
		}
		[RelayCommand]
		private async Task SelectRootFolderAsync(Control control)
		{
			var folder = await OpenFolderPickerAsync(control, "设置目录").ConfigureAwait(false);
			if (!Directory.Exists(folder)) {
				MessageService.NotificationError("错误", "选择的目录不存在！");
				return;
			}
			RootPath = folder;
		}
	}
}
