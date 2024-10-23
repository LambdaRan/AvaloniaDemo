using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaDemo.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaDemo.Utils;

namespace AvaloniaDemo.ViewModels
{
	public partial class NoticeViewModel : ViewModelBase<NoticeViewModel>
	{
		private readonly AppConfig _AppCfg;
		public NoticeViewModel(ICommonServices<NoticeViewModel> commonServices, AppConfig appCfg)
			: base(commonServices)
		{
			_AppCfg = appCfg;
			_NoticeContent = _AppCfg.LatestNoticeMsg;
		}

		[ObservableProperty]
		private string? _NoticeContent;
	}
}
