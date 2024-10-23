using CliWrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaDemo.Utils
{
	// cmd命令：https://learn.microsoft.com/zh-cn/windows-server/administration/windows-commands/start
	public static class CmdUtil
    {
		private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();
		public static async Task OpenFolderAsync(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            var cmd = Cli.Wrap("cmd.exe")
                .WithArguments($@"/c start /b {path}");
            try
            {
                await cmd.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // 不关心执行结果
            }
            return;
        }
        public static async Task MoveFileAsync(string workDir, string from, string to)
        {
            var cmd = Cli.Wrap("cmd.exe")
                        .WithArguments($@"/c move /Y {from} {to}")
                        .WithWorkingDirectory(workDir);
            await cmd.ExecuteAsync().ConfigureAwait(false);
            return;
        }

        public static async Task RunCmdBatAsync(string workDir, string bat, string arg)
        {
            var cmd = Cli.Wrap("cmd.exe")
                .WithArguments($@"/c start {bat} {arg}")
                .WithWorkingDirectory(workDir)
                .WithValidation(CommandResultValidation.None);
            try
            {
                await cmd.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // 不关心执行结果
            }
            return;
        }
		public static async Task<bool> RunCmdBatWaitFinishedAsync(string workDir, string bat, string arg, CancellationToken ctoken)
		{
			var cmd = Cli.Wrap("cmd.exe")
				.WithArguments($@"/c {bat} {arg}")
				.WithWorkingDirectory(workDir)
				.WithStandardInputPipe(PipeSource.FromString("exit"))
                .WithStandardOutputPipe(PipeTarget.ToStream(Stream.Null))
				.WithValidation(CommandResultValidation.None);
			try {
				await cmd.ExecuteAsync(ctoken).ConfigureAwait(false);
			}
            catch (OperationCanceledException) {
                return false;
            }
			catch (Exception ex) {
                // 不关心执行结果
                _Logger.Error(ex, "RunCmdBatWaitFinishedAsync");
                return false;
			}
			return true;
		}
		public static async Task RunPowerShellAsync(string workDir, string bat, string arg)
        {
            var cmd = Cli.Wrap("PowerShell.exe")
                .WithArguments($@"-WindowStyle Hidden -NonInteractive -Command ""{bat} {arg}""")
                .WithWorkingDirectory(workDir)
                .WithValidation(CommandResultValidation.None);
            try
            {
                await cmd.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // 不关心执行结果
            }
            return;
        }

    }
}
