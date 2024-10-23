using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
	public class NavigationItem
	{
		public string Label { get; private set; } = string.Empty;
		public Type ViewModel { get; private set; }

		public NavigationItem(Type viewModel) 
		{
			ViewModel = viewModel;
		}
		public NavigationItem(string label, Type viewModel) : this(viewModel)
		{
			Label = label;
		}
	}
}
