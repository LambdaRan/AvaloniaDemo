using AvaloniaDemo.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaDemo.Services
{
	public sealed class UserConfig
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? RootPath { get; set; }

		public int NoticeId { get; set; } = 0;
	}
	[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Default)]
	[JsonSerializable(typeof(UserConfig))]
	//[JsonSerializable(typeof(VersionType))]
	[JsonSerializable(typeof(string))]
	public partial class AppConfigSourceGenerationContext : JsonSerializerContext
	{

	}

	public sealed class AppConfig : IDisposable
	{
		public AppConfig() {}

		public string SvnPath {
			get => Path.Combine(GetAppBasePath(), @"svn.exe");
		}
		public string TortoiseSVN => "TortoiseProc.exe";

		public string RootPath {
			get => _UserConfig.RootPath ?? string.Empty;
			set {
				_UserConfig.RootPath = value;
				TriggerLazySave();
			}
		}

		public string GetAppBasePath()
			=> AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? Environment.CurrentDirectory;

		public int HasReadNoticeId {
			get => _UserConfig.NoticeId;
			set {
				_UserConfig.NoticeId = value;
				TriggerLazySave();
			}
		}
		public string NoticePath => Path.Combine(GetAppBasePath(), "Notice.txt");
		public int LatestNoticeId { get; private set; } = -1;
		public string LatestNoticeMsg { get; private set; } = "暂无更新内容";
		public bool HasReadNotice() 
		{ 
			if (HasReadNoticeId == -1 || LatestNoticeId == -1) {
				return true;
			}
			return HasReadNoticeId == LatestNoticeId;
		}
		public void SetHasReadNotice()
		{
			HasReadNoticeId = LatestNoticeId;
		}
		private void ReadNotice()
		{
			if (!File.Exists(NoticePath)) {
				return;
			}
			var lines = new List<string>();
			using (var reader = new StreamReader(NoticePath, FilePathDirUtil.GBK)) {
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
					if (line.StartsWith('#')) {
						continue;
					}
					lines.Add(line);
					if (lines.Count > 10) {
						lines.Add($"更多更新内容请看{NoticePath}");
						break;
					}
				}
			}
			if (int.TryParse(lines[0], out var id)) {
				LatestNoticeId = id;
				LatestNoticeMsg = string.Join("\r\n", lines.Skip(1));
			}
		}

		#region 配置文件读写
		// 自身数据
		private readonly string UserConfigFileName = "UserConfig.json";
		private UserConfig _UserConfig = new();
		private bool _SaveAfter = false;
		private Timer? _SaveTimer;
		public async Task InitAsync()
		{
			_SaveAfter = false;
			_SaveTimer = new Timer(LazySaveTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
			await ReadUserConfigAsync();
			ReadNotice();
		}
		public async Task<bool> ReadUserConfigAsync()
		{
			string path = Path.Combine(GetAppBasePath(), UserConfigFileName);
			if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
				return false;
			}
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				_UserConfig = await JsonSerializer.DeserializeAsync<UserConfig>(fs).ConfigureAwait(false) ?? new();
				//_UserConfig = await JsonSerializer.DeserializeAsync<UserConfig>(fs, AppConfigSourceGenerationContext.Default.UserConfig).ConfigureAwait(false) ?? new();
				//string jsonString = JsonSerializer.Serialize(_UserConfig);
				//Debug.WriteLine($"ReadUserConfigAsync:{jsonString}");
			}
			return true;
		}
		public async Task SaveUserConfigAsync()
		{
			string path = Path.Combine(GetAppBasePath(), UserConfigFileName);
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite)) {
				var options = new JsonSerializerOptions()
				{
					WriteIndented = true
				};
				await JsonSerializer.SerializeAsync(fs, _UserConfig, options).ConfigureAwait(false);
			}
		}
		private void LazySaveTimerCallback(object? state)
		{
			_SaveAfter = false;
			SaveUserConfigAsync().Wait();
		}
		private void TriggerLazySave()
		{
			if (_SaveTimer is null) {
				SaveUserConfigAsync().Wait();
				return;
			}
			if (_SaveAfter) {
				return;
			}
			_SaveAfter = true;
			_SaveTimer.Change(30 * 1000, Timeout.Infinite);
		}

		#endregion

		public void Dispose() => _SaveTimer?.Dispose();
	}
}
