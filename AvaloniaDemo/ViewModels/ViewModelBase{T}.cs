using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using AvaloniaDemo.Services;
using AvaloniaDemo.Src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
	public class ViewModelBase<TMode> : ObservableObject
	{
		public ViewModelBase(ICommonServices<TMode> commonServices)
		{
			MessageService = commonServices.MessageService;
			Logger = commonServices.Logger;
			DialogHost = commonServices.DialogHost;
		}

		public MessageService MessageService { get; }
		public ILogger<TMode> Logger { get; }
		public MainDialogHost DialogHost { get; }

		#region logger
		public void LogDebug(string? message, params object?[] args)
		{
			Logger.LogDebug(message, args);
		}
		public void LogInformation(Exception? exception, string? message, params object?[] args)
		{
			Logger.LogInformation(exception, message, args);
		}
		public void LogInformation(string? message, params object?[] args)
		{
			Logger.LogInformation(message, args);
		}
		public void LogWarning(Exception? exception, string? message, params object?[] args)
		{
			Logger.LogWarning(exception, message, args);
		}
		public void LogWarning(string? message, params object?[] args)
		{
			Logger.LogWarning(message, args);
		}
		public void LogError(Exception? exception, string? message, params object?[] args)
		{
			Logger.LogError(exception, message, args);
		}
		public void LogError(string? message, params object?[] args)
		{
			Logger.LogError(message, args);
		}
		#endregion

		#region status message
		public virtual void Subscribe()
		{
			// Nothing
		}
		public virtual void Unsubscribe()
		{
			MessageService.UnregisterAll(this);
		}
		public void WorkStartStatusMessage(string message)
		{
			MessageService.WorkStartStatusMessage(message);
		}
		public void WorkEndStatusMessage(string message)
		{
			MessageService.WorkEndStatusMessage(message);
		}
		public void ShowStatusReady()
		{
			MessageService.ShowStatusNormalMsg("Ready");
		}
		public void ShowStatusNormalMsg(string message)
		{
			MessageService.ShowStatusNormalMsg(message);
		}
		public void ShowStatusErrorMsg(string message)
		{
			MessageService.ShowStatusErrorMsg(message);
		}
		#endregion

		#region Log message

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ShowLog(string message) => MessageService.ShowLog(message);
		#endregion

		#region Notification message
		public void NotificationInfomation(string title, string message)
			 => MessageService.NotificationInfomation(title, message);
		public void NotificationSuccess(string title, string message)
			=> MessageService.NotificationSuccess(title, message);
		public void NotificationWarning(string title, string message)
			=> MessageService.NotificationWarning(title, message);
		public void NotificationError(string title, string message)
			=> MessageService.NotificationError(title, message);
		#endregion

		#region file/folder picker
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
		#endregion
	}
}
