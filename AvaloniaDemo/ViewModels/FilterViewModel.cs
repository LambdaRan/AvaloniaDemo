using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
	public partial class FilterViewModel : ObservableObject
	{
		private Regex? _FilterRegex;
		public event EventHandler? RefreshFilter;

		public bool Filter(string input)
		{
			return _FilterRegex?.IsMatch(input) ?? true;
		}

		[ObservableProperty]
		private string _FilterString = string.Empty;
		partial void OnFilterStringChanged(string value)
		{
			UpdateFilterRegex();
			RefreshFilter?.Invoke(this, EventArgs.Empty);
		}

		[ObservableProperty]
		private bool _UseCaseSensitiveFilter = false;
		partial void OnUseCaseSensitiveFilterChanged(bool value)
		{
			UpdateFilterRegex();
			RefreshFilter?.Invoke(this, EventArgs.Empty);
		}

		[ObservableProperty]
		private bool _UseWholeWordFilter = false;
		partial void OnUseWholeWordFilterChanged(bool value)
		{
			UpdateFilterRegex();
			RefreshFilter?.Invoke(this, EventArgs.Empty);
		}

		[ObservableProperty]
		private bool _UseRegexFilter = false;
		partial void OnUseRegexFilterChanged(bool value)
		{
			UpdateFilterRegex();
			RefreshFilter?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateFilterRegex()
		{
			var options = RegexOptions.Compiled;
			var pattern = UseRegexFilter
				? FilterString.Trim() : Regex.Escape(FilterString.Trim());
			if (!UseCaseSensitiveFilter) {
				options |= RegexOptions.IgnoreCase;
			}
			if (UseWholeWordFilter) {
				pattern = $"\\b(?:{pattern})\\b";
			}
			_FilterRegex = new Regex(pattern, options);
		}
	}
}
