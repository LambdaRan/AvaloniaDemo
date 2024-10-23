using CliWrap;
using CliWrap.Buffered;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Primitives;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MicrStringTokenizer = Microsoft.Extensions.Primitives.StringTokenizer;

namespace AvaloniaDemo.Utils
{
	// https://svnbook.red-bean.com/en/1.6/index.html
	// https://svnbook.red-bean.com/en/1.7/svn.ref.svn.html
	// https://svnbook.red-bean.com/en/1.7/svn.ref.html
	// https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-automation.html
	public static partial class SvnUtil
	{
		private static readonly char[] CRLF = { '\r', '\n' };
		public static readonly char StatusDeletion = 'D';
		public static readonly char StatusModified = 'M';
		public static readonly char StatusAddition = 'A';
		public static readonly char StatusConflict = 'C';
		public static readonly char StatusNotUnderVersionControl = '?';
		public static readonly char StatusLock = 'L';

		#region Commandline
		// *   721574   xxx\xxx\xxx.xxx
		[GeneratedRegex(@"\*.*\d{4,7}\s*(\S+)")]
		private static partial Regex OutOfDateFilesRegex();
		public static async Task<List<string>> GetFolderOutOfDateFilesAsync(string svn, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return new List<string>();
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"status -u {workdir}");
			var result = new List<string>();
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var mcoll = OutOfDateFilesRegex().Matches(cmdRet.StandardOutput);
				foreach (Match m in mcoll) {
					result.Add(m.Groups[1].Value);
				}
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return result;
		}
		public static async Task<List<string>> GetOutOfDateFilesAsync(string svn, IEnumerable<string> files, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || files.Count() == 0 || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return new List<string>();
			}
			var sb = StringBuilderCache.Acquire(512);
			sb.Append("status -u");
			sb.Append(' ');
			sb.AppendJoin(' ', files);
			var param = StringBuilderCache.GetStringAndRelease(sb);
			var cmd = Cli.Wrap(svn)
				.WithArguments(param)
				.WithWorkingDirectory(workdir);
			var result = new List<string>();
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var mcoll = OutOfDateFilesRegex().Matches(cmdRet.StandardOutput);
				foreach (Match m in mcoll) {
					result.Add(m.Groups[1].Value);
				}
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return result;
		}
		public static async Task<bool> CheckFileIsOutOfDateAsync(string svn, string path)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(path) || !File.Exists(path)) {
				return false;
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"status -u {path}");
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var mcoll = OutOfDateFilesRegex().Matches(cmdRet.StandardOutput);
				return mcoll?.Count > 0;
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return false;
		}

		private static async Task<int> _GetRevisionAsync(string svn, string revision, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return -1;
			}
			if (string.IsNullOrWhiteSpace(revision) || (revision != "BASE" && revision != "HEAD")) {
				ThrowHelper.ThrowArgumentException("Revision must be BASE or HEAD");
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"info -r {revision} {workdir}")
				.WithWorkingDirectory(workdir);
			var result = new List<string>();
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var tokenier = new MicrStringTokenizer(cmdRet.StandardOutput, CRLF);
				var srevision = tokenier.FirstOrDefault(
					s => s.StartsWith("Revision:", StringComparison.OrdinalIgnoreCase) || s.StartsWith("版本:", StringComparison.OrdinalIgnoreCase),
					StringSegment.Empty);
				if (srevision == StringSegment.Empty) {
					return -1;
				}
				return Convert.ToInt32(srevision.Subsegment(srevision.IndexOf(':') + 1).Trim().ToString());
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return -1;
		}
		public static async Task<int> GetLocalRevisionAsync(string svn, string workdir)
		{
			return await _GetRevisionAsync(svn, "BASE", workdir);
		}
		public static async Task<int> GetHeadRevisionAsync(string svn, string workdir)
		{
			return await _GetRevisionAsync(svn, "HEAD", workdir);
		}

		private static async Task RunCmdAsync(string svn, string scmd, string path)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(scmd) || string.IsNullOrEmpty(path)) {
				return;
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"{scmd} {path}");
			try {
				await cmd.ExecuteAsync().ConfigureAwait(false);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return;
		}
		private static async Task RunCmdAsync(string svn, string scmd, IEnumerable<string> files, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return;
			}
			if (string.IsNullOrEmpty(scmd) || files.Count() == 0) {
				return;
			}
			var sb = StringBuilderCache.Acquire(512);
			sb.Append(scmd);
			sb.Append(' ');
			sb.AppendJoin(' ', files);
			var param = StringBuilderCache.GetStringAndRelease(sb);
			var cmd = Cli.Wrap(svn)
						.WithArguments(param)
						.WithWorkingDirectory(workdir);
			try {
				await cmd.ExecuteAsync().ConfigureAwait(false);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return;
		}
		public static async Task RevertAsync(string svn, string path)
		{
			await RunCmdAsync(svn, "revert", path);
		}
		public static async Task ResolvedAsync(string svn, string path)
		{
			await RunCmdAsync(svn, "resolve --accept working", path);
		}
		public static async Task AddAsync(string svn, string path)
		{
			await RunCmdAsync(svn, "add", path);
		}
		public static async Task AddDirectoryAsync(string svn, string path)
		{
			await RunCmdAsync(svn, "add --non-recursive", path);
		}
		public static async Task DeleteAsync(string svn, string path)
		{
			await RunCmdAsync(svn, "delete", path);
		}
		public static async Task UpdateAsync(string svn, string path)
		{
			await RunCmdAsync(svn, "update --accept postpone", path);
		}
		public static async Task UpdateAsync(string svn, IEnumerable<string> files, string workdir)
		{
			await RunCmdAsync(svn, "update --accept postpone", files, workdir);
		}

		public static async Task<string[]> GetDirectoryFilesAsync(string svn, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return Array.Empty<string>();
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"list {workdir}")
				.WithWorkingDirectory(workdir);
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				return cmdRet.StandardOutput.Split(CRLF, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return Array.Empty<string>();
		}
		public static async Task<string> StatusForFileAsync(string svn, string path)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(path) || !File.Exists(path)) {
				return "N";
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"status {path}");
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				if (cmdRet.StandardOutput.Length >= 8) {
					return cmdRet.StandardOutput.Substring(0, 8);
				}
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return "N";
		}
		public static async Task<bool> IsConflictAsync(string svn, string path)
		{
			var status = await StatusForFileAsync(svn, path);
			return status.Contains(StatusConflict);
		}
		public static async Task<bool> IsCleanAsync(string svn, string path)
		{
			var status = await StatusForFileAsync(svn, path);
			return string.IsNullOrWhiteSpace(status);
		}
		public static async Task<bool> IsNotUnderVersionControlAsync(string svn, string path)
		{
			var status = await StatusForFileAsync(svn, path);
			return status.Contains(StatusNotUnderVersionControl);
		}
		public static async Task<Dictionary<string, string>> StatusForFilesAsync(string svn, IEnumerable<string> files, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return new Dictionary<string, string>();
			}
			var sb = StringBuilderCache.Acquire(512);
			sb.Append("status");
			sb.Append(' ');
			sb.AppendJoin(' ', files);
			var param = StringBuilderCache.GetStringAndRelease(sb);
			var cmd = Cli.Wrap(svn)
				.WithArguments(param)
				.WithWorkingDirectory(workdir);
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var tokenizer = new MicrStringTokenizer(cmdRet.StandardOutput, CRLF);
				return tokenizer.Where(v => v.Length >= 8 && !v.EndsWith(":", StringComparison.Ordinal))
					.ToDictionary(
						static line => line.Subsegment(8).Trim().ToString(),
						static line => line.Subsegment(0, 8).Trim().ToString());
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return new Dictionary<string, string>();
		}

		public static async Task<Dictionary<string, string>> StatusForDirectoryAsync(string svn, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return new Dictionary<string, string>();
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"status {workdir}");
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var tokenizer = new MicrStringTokenizer(cmdRet.StandardOutput, CRLF);
				return tokenizer.Where(v => v.Length >= 8 && !v.EndsWith(":", StringComparison.Ordinal))
					.ToDictionary(
						static line => line.Subsegment(8).Trim().ToString(),
						static line => line.Subsegment(0, 8).Trim().ToString());
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return new Dictionary<string, string>();
		}
		public static async Task<string[]> GetFilesWithStatusAsync(string svn, string workdir, char status)
		{
			var files = await StatusForDirectoryAsync(svn, workdir);
			return files.Where(kv => kv.Value.IndexOf(status) != -1).Select(kv => kv.Key).ToArray();
		}

		public static async Task<Dictionary<string, string>> ModifiedFilesForDirectoryAsync(string svn, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return new Dictionary<string, string>();
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"diff --summarize")
				.WithWorkingDirectory(workdir);
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var tokenizer = new MicrStringTokenizer(cmdRet.StandardOutput, CRLF);
				return tokenizer.Where(v => v.Length >= 8).ToDictionary(
					static line => line.Subsegment(8).Trim().ToString(),
					static line => line.Subsegment(0, 8).Trim().ToString());
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return new Dictionary<string, string>();
		}
		public static async Task<string[]> GetModifiedFilesAsync(string svn, string workdir, char status)
		{
			var files = await ModifiedFilesForDirectoryAsync(svn, workdir);
			return files.Where(kv => kv.Value.IndexOf(status) != -1).Select(kv => kv.Key).ToArray();
		}

		[GeneratedRegex(@"Conflict Previous Working File: (.+)\r\n")]
		private static partial Regex MineNameRegex();
		[GeneratedRegex(@"Conflict Previous Base File: (.+)\r\n")]
		private static partial Regex BaseNameRegex();
		[GeneratedRegex(@"Conflict Current Base File: (.+)\r\n")]
		private static partial Regex OtherNameRegex();
		public static async Task<(string MineName, string BaseName, string OtherName)> GetConflictName(string svn, string path)
		{
			(string MineName, string BaseName, string OtherName) conflic = (string.Empty, string.Empty, string.Empty);
			if (string.IsNullOrEmpty(svn) || !File.Exists(svn) || string.IsNullOrEmpty(path) || !File.Exists(path)) {
				return conflic;
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"info {path}")
				.WithWorkingDirectory(Path.GetDirectoryName(path)!);
			try {
				var cmdRet = await cmd.ExecuteBufferedAsync().ConfigureAwait(false);
				var mMatch = MineNameRegex().Match(cmdRet.StandardOutput);
				if (mMatch.Success) {
					conflic.MineName = mMatch.Groups[1].Value;
				}
				var bMatch = BaseNameRegex().Match(cmdRet.StandardOutput);
				if (bMatch.Success) {
					conflic.BaseName = bMatch.Groups[1].Value;
				}
				var oMatch = OtherNameRegex().Match(cmdRet.StandardOutput);
				if (oMatch.Success) {
					conflic.OtherName = oMatch.Groups[1].Value;
				}
				return conflic;
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return conflic;
		}
		#endregion

		#region WithUI
		private static async Task RundCmdWithUIAsync(string svn, string scmd, IEnumerable<string> files, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return;
			}
			if (string.IsNullOrEmpty(scmd) || files.Count() == 0) {
				return;
			}
			//closeonend:0 不自动关闭对话框
			//closeonend:1 如果没发生错误则自动关闭对话框
			//closeonend:2 如果没发生错误和冲突则自动关闭对话框
			//closeonend:3 如果没有错误、冲突和合并，会自动关闭
			//closeonend:4 如果没有错误、冲突和合并，会自动关闭
			var sb = StringBuilderCache.Acquire(256);
			sb.Append("/c start /b ");
			sb.Append(svn);
			sb.Append(" /command:");
			sb.Append(scmd);
			sb.Append(" /closeonend:3");
			sb.Append(" /Path:");
			sb.AppendJoin('*', files);
			var param = StringBuilderCache.GetStringAndRelease(sb);
			var cmd = Cli.Wrap("cmd.exe")
				.WithArguments(param)
				.WithWorkingDirectory(workdir);
			try {
				await cmd.ExecuteAsync().ConfigureAwait(false);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return;
		}
		private static async Task RundCmdWithUIAsync(string svn, string scmd, string file, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || string.IsNullOrEmpty(scmd) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return;
			}
			var cmd = Cli.Wrap("cmd.exe")
				.WithArguments($"/c start /b {svn} /command:{scmd} /closeonend:3 /path:{file}")
				.WithWorkingDirectory(workdir);
			try {
				await cmd.ExecuteAsync().ConfigureAwait(false);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return;
		}
		private static async Task RundCmdWithUIAsync(string svn, string scmd, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || string.IsNullOrEmpty(scmd) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return;
			}
			var cmd = Cli.Wrap("cmd.exe")
				.WithArguments($"/c start /b {svn} /command:{scmd} /closeonend:3 /path:{workdir}")
				.WithWorkingDirectory(workdir);
			try {
				await cmd.ExecuteAsync().ConfigureAwait(false);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return;
		}
		private static async Task RundCmdWithUIWaitFinishedAsync(string svn, string scmd, string workdir)
		{
			if (string.IsNullOrEmpty(svn) || string.IsNullOrEmpty(scmd) || string.IsNullOrEmpty(workdir) || !Directory.Exists(workdir)) {
				return;
			}
			var cmd = Cli.Wrap(svn)
				.WithArguments($"/command:{scmd} /closeonend:3 /path:{workdir}")
				.WithWorkingDirectory(workdir);
			try {
				await cmd.ExecuteAsync().ConfigureAwait(false);
			}
			catch (Exception) {
				// 不关心执行结果
			}
			return;
		}
		public static async Task CommitWithUIAsync(string svn, string workdir)
		{
			await RundCmdWithUIAsync(svn, "commit", workdir);
		}
		public static async Task CommitWithUIAsync(string svn, IEnumerable<string> files, string workdir)
		{
			await RundCmdWithUIAsync(svn, "commit", files, workdir);
		}
		public static async Task UpdateWithUIAsync(string svn, string workdir)
		{
			await RundCmdWithUIAsync(svn, "update", workdir);
		}
		public static async Task UpdateWithUIWaitFinishedAsync(string svn, string workdir)
		{
			await RundCmdWithUIWaitFinishedAsync(svn, "update", workdir);
		}
		public static async Task UpdateWithUIAsync(string svn, IEnumerable<string> files, string workdir)
		{
			await RundCmdWithUIAsync(svn, "update", files, workdir);
		}
		public static async Task RevertWithUIAsync(string svn, string workdir)
		{
			await RundCmdWithUIAsync(svn, "revert", workdir);
		}
		public static async Task RevertWithUIAsync(string svn, IEnumerable<string> files, string workdir)
		{
			await RundCmdWithUIAsync(svn, "revert", files, workdir);
		}
		public static async Task LockWithUIAsync(string svn, IEnumerable<string> files, string workdir)
		{
			await RundCmdWithUIAsync(svn, "lock", files, workdir);
		}
		public static async Task UnLockWithUIAsync(string svn, IEnumerable<string> files, string workdir)
		{
			await RundCmdWithUIAsync(svn, "unlock", files, workdir);
		}
		public static async Task LogWithUIAsync(string svn, string file, string workdir)
		{
			await RundCmdWithUIAsync(svn, "log", file, workdir);
		}
		public static async Task LogWithUIAsync(string svn, string workdir)
		{
			await RundCmdWithUIAsync(svn, "log", workdir, workdir);
		}
		#endregion
	}
}
