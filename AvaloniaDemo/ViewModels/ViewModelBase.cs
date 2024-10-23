using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using AvaloniaDemo.Services;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
	public class ViewModelBase : ObservableObject
	{
		public ViewModelBase()
		{
		}

		public async ValueTask<string> OpenFilePickerAsync(Control control, string title = "")
		{
			var topLevel = TopLevel.GetTopLevel(control);
			if (topLevel == null) {
				return string.Empty;
			}
			var files = await topLevel.StorageProvider.OpenFilePickerAsync(
							new FilePickerOpenOptions()
							{
								AllowMultiple = false,
								Title = title,
							}).ConfigureAwait(false);
			if (files is null || files.Count == 0) {
				return string.Empty;
			}
			return files[0].Path.LocalPath ?? string.Empty;
		}
		public async ValueTask<string[]?> OpenFilesPickerAsync(Control control, string title = "")
		{
			var topLevel = TopLevel.GetTopLevel(control);
			if (topLevel == null) {
				return null;
			}
			var files = await topLevel.StorageProvider.OpenFilePickerAsync(
							new FilePickerOpenOptions()
							{
								AllowMultiple = true,
								Title = title,
							}).ConfigureAwait(false);
			return files?.Select(x => x.Path.LocalPath)?.ToArray();
		}

		public async ValueTask<string> OpenFolderPickerAsync(Control control, string title = "")
		{
			var topLevel = TopLevel.GetTopLevel(control);
			if (topLevel == null) {
				return string.Empty;
			}
			var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(
							new FolderPickerOpenOptions()
							{
								AllowMultiple = false,
								Title = title,
							}).ConfigureAwait(false);
			if (folders is null || folders.Count == 0) {
				return string.Empty;
			}
			return folders[0].Path.LocalPath! ?? string.Empty;
		}
		public async ValueTask<string[]?> OpenFoldersPickerAsync(Control control, string title = "")
		{
			var topLevel = TopLevel.GetTopLevel(control);
			if (topLevel == null) {
				return null;
			}
			var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(
							new FolderPickerOpenOptions()
							{
								AllowMultiple = true,
								Title = title,
							}).ConfigureAwait(false);
			return folders?.Select(x => x.Path.LocalPath)?.ToArray();
		}
	}
}
