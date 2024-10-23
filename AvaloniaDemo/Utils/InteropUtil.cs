using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo
{
	//https://learn.microsoft.com/zh-cn/dotnet/standard/native-interop/tutorial-custom-marshaller
	// runtime src/libraries/Common/src/Interop/Windows
	public static partial class InteropUtil
	{
		[LibraryImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		public static partial long WritePrivateProfileString(string section, string key, string val, string filePath);

		// 窗口
		// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
		private const int WS_SHOWNORMAL = 1;
		private const int SW_SHOWMAXIMIZED = 3;

		[LibraryImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

		[LibraryImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool ShowWindow(IntPtr hWnd, int cmdShow);

		[LibraryImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool SetForegroundWindow(IntPtr hWnd);

		public static bool SetWndTopNormal(IntPtr hWnd)
		{
			return ShowWindowAsync(hWnd, WS_SHOWNORMAL) && SetForegroundWindow(hWnd);
		}
		public static bool SetWndTopMaximized(IntPtr hWnd)
		{
			return ShowWindowAsync(hWnd, SW_SHOWMAXIMIZED) && SetForegroundWindow(hWnd);
		}

		[LibraryImport("user32.dll")]
		public static partial uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
	}
}
