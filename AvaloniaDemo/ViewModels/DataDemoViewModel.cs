using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaDemo.Extensions;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Input;
using AvaloniaDemo.Services;
using System.Threading;
using System.Reflection;
using AvaloniaDemo.Utils;
using System.IO;
using Avalonia.Threading;
using AvaloniaDemo.Models;

namespace AvaloniaDemo.ViewModels
{
	public partial class DataDemoViewModel : ViewModelBase<DataDemoViewModel>
	{
		private readonly AppConfig _AppCfg;
		private readonly BusyDialog _BusyDialog;
		public DataDemoViewModel(ICommonServices<DataDemoViewModel> commonServices, AppConfig appcfg, BusyDialog dialog)
			: base(commonServices)
		{
			_AppCfg = appcfg;
			_BusyDialog = dialog;

			Source = new FlatTreeDataGridSource<DataEntry>(_Items)
			{
				Columns =
				{
					new TextColumn<DataEntry, string>("Id", static x => x.Id, new GridLength(1, GridUnitType.Star)),
					new TextColumn<DataEntry, string>("Name", static x => x.Name, new GridLength(2, GridUnitType.Star)),
					new TextColumn<DataEntry, int>("Age", static x => x.Age, new GridLength(1, GridUnitType.Star)),
				},
			};
			// 单行选择
			Source.RowSelection!.SingleSelect = false;
			// 搜索
			FilterViewModel = new FilterViewModel();
			FilterViewModel.RefreshFilter += (s, e) => ItemFilter();
			ItemFilter();
		}
		private List<DataEntry> _AllItems = new() { 
			new DataEntry() { Id = "1", Name = "n1", Age = 1},
			new DataEntry() { Id = "2", Name = "n2", Age = 2}
		};
		private ObservableCollectionExt<DataEntry> _Items = new ObservableCollectionExt<DataEntry>();
		//[ObservableProperty]
		//private FlatTreeDataGridSource<string> _Source;
		public FlatTreeDataGridSource<DataEntry> Source { get; }
		public FilterViewModel FilterViewModel { get; private set; }
		private void ItemFilter()
		{
			var items = _AllItems.Where(item => FilterViewModel.Filter(item.Id));
			_Items.ClearThenCopyFrom(items);
		}
	}
}
