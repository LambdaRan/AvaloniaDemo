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
	public partial class NewFileViewModel : ViewModelBase<NewFileViewModel>
	{
		public NewFileViewModel(ICommonServices<NewFileViewModel> commonServices)
			: base(commonServices)
		{
		}

		[ObservableProperty]
		private string _FileName = string.Empty;

		[ObservableProperty]
		private string _FileFolder = string.Empty;

		[RelayCommand]
		private async Task SelectDestFolderAsync(Control control)
		{
			FileFolder = string.Empty;
			var folder = await OpenFolderPickerAsync(control, "在哪创建文件");
			if (!Directory.Exists(folder)) {
				MessageService.NotificationError("错误", "选择的目录不存在！");
				return;
			}
			FileFolder = folder;
		}

		[RelayCommand]
		public async Task CreateNewFileAsync()
		{
			if (string.IsNullOrEmpty(FileName)) {
				MessageService.NotificationError("错误", "请填写文件名！");
				return;
			}
			if (string.IsNullOrEmpty(FileFolder) || !Directory.Exists(FileFolder)) {
				MessageService.NotificationError("错误", "选择的目录不存在!");
				return;
			}
			await Task.Delay(TimeSpan.FromSeconds(2));
			MessageService.NotificationSuccess("成功", "创建文件成功,Happy Work！");
			DialogHost.Close();
		}
	}
}
