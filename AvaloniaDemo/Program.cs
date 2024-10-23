using Avalonia;
using Avalonia.Media;
using System;

namespace AvaloniaDemo
{
	internal sealed class Program
	{
		// 处理全局异常 https://docs.avaloniaui.net/zh-Hans/docs/concepts/unhandledexceptions
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args) => BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.WithInterFont()
				.LogToTrace()
				// 设置字体
				.With(new FontManagerOptions()
				{
					DefaultFamilyName = "Microsoft YaHei",
					FontFallbacks = [
						new FontFallback { FontFamily = "SimHei" },
						new FontFallback { FontFamily = "SimKai" },
					],
				});
	}
}
