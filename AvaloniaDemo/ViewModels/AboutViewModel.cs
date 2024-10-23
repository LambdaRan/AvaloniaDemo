using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaloniaDemo.Services;
using AvaloniaDemo.Utils;

namespace AvaloniaDemo.ViewModels
{
	public partial class AboutViewModel : ViewModelBase<AboutViewModel>
	{
		private readonly AppConfig _AppCfg;
		public AboutViewModel(ICommonServices<AboutViewModel> commonServices, AppConfig appCfg)
			: base(commonServices)
		{
			_AppCfg = appCfg;
		}

		[RelayCommand]
		private async Task OpenAvaloniaDemoFolderAsync()
		{
			string dir = _AppCfg.GetAppBasePath();
			await CmdUtil.OpenFolderAsync(dir);
		}
		[RelayCommand]
		private async Task OpenAvaloniaDemoLogFolderAsync()
		{
			string dir = Path.Combine(_AppCfg.GetAppBasePath(), "logs");
			await CmdUtil.OpenFolderAsync(dir);
		}
	}
}
