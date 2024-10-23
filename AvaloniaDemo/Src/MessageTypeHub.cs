using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.Src
{
	// 底部状态信息
	public sealed class StatusMessage
	{
		public string Status { get; set; }
		public bool IsError { get; set; }
		public bool IsToLog { get; set; }
		public StatusMessage(string status, bool isError, bool log)
		{
			Status = status;
			IsError = isError;
			IsToLog = log;
		}
		public static StatusMessage Empty => new StatusMessage(string.Empty, false, false);
		public static StatusMessage Normal(string message) => new StatusMessage(message, false, false);
		public static StatusMessage Error(string message) => new StatusMessage(message, true, false);
		public static StatusMessage NormalWithLog(string message) => new StatusMessage(message, false, true);
		public static StatusMessage ErrorWithLog(string message) => new StatusMessage(message, true, true);
	}
	// 日志消息
	public sealed class LogMessage
	{
		public string Log { get; set; } = string.Empty;
		public static LogMessage LogMsg(string log) => new LogMessage() { Log = log };
	}
	// 通知消息
	// Notification
}
