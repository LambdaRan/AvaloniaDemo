using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaDemo.ViewModels;
using AvaloniaDemo.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AvaloniaDemo
{
	public partial class App : Application
	{
		private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();
		private static readonly ServiceCollection _serviceCollection = new ServiceCollection();

		public static IServiceProvider Services => Ioc.Default;
		private bool _CanClose = false;

		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			Name = "AvaloniaDemo";
			// ��������
			InitApp();
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
				// Line below is needed to remove Avalonia data validation.
				// Without this line you will get duplicate validations from both Avalonia and CT
				BindingPlugins.DataValidators.RemoveAt(0);
				desktop.MainWindow = new MainWindow();
				//desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
				desktop.ShutdownRequested += DesktopOnShutdownRequested;
				desktop.Startup += OnAppStartup;
				desktop.Exit += OnAppExist;
			}
			base.OnFrameworkInitializationCompleted();
			// ��ʼ����־��_Logger ����
			var log = Services.GetRequiredService<ILogger<App>>();
			log.LogInformation("OnFrameworkInitializationCompleted");
		}

		public void InitApp()
		{
			_serviceCollection.AddSingleton<AppConfig>();
			_serviceCollection.AddSingleton<BusyDialog>();
			_serviceCollection.AddSingleton<MainDialogHost>();
			_serviceCollection.AddSingleton<MessageService>();
			_serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(ICommonServices<>), typeof(CommonServices<>)));

			// ע�ᴰ�� AddTransientÿ����ʾ��Ӧ��View�����ᴴ��ViewModel
			_serviceCollection.AddTransient<MainWindowViewModel>();
			NavigationService.Register<DataDemoViewModel, DataDemoView>();
			_serviceCollection.AddTransient<DataDemoViewModel>();
			NavigationService.Register<SettingViewModel, SettingView>();
			_serviceCollection.AddTransient<SettingViewModel>();
			NavigationService.Register<AboutViewModel, AboutView>();
			_serviceCollection.AddTransient<AboutViewModel>();
			// ģ̬����
			NavigationService.Register<NewFileViewModel, NewFileView>();
			_serviceCollection.AddTransient<NewFileViewModel>();
			NavigationService.Register<NoticeViewModel, NoticeView>();
			_serviceCollection.AddTransient<NoticeViewModel>();

			var logOptions = new NLogProviderOptions();
			var logConfig = GetNLogConfiguration();
			_serviceCollection.AddLogging(builder => {
				builder.ClearProviders();
				builder.AddNLog(logConfig, logOptions);
				builder.AddFilter<NLogLoggerProvider>("Microsoft", LogLevel.Warning);
			});

			Ioc.Default.ConfigureServices(_serviceCollection.BuildServiceProvider());
		}

		private async void OnAppStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs earg)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			// ����UI�߳�δ������쳣��Ҫ��main������ʹ��try-catch
			// ע�� GBK ����
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var appcfg = Ioc.Default.GetRequiredService<AppConfig>();
			await appcfg.InitAsync();
			_Logger.Info("OnAppStartup");
		}

		private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
		{
			_Logger.Info("DestopOnShutdownRequested");
			// ��ȡ�����ȱ����������ٹر�
			e.Cancel = !_CanClose;
			if (!_CanClose) {
				// To save 
				await Task.CompletedTask;
				_CanClose = true;
				if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
					desktop.Shutdown();
				}
			}
		}
		private async void OnAppExist(object? sender, ControlledApplicationLifetimeExitEventArgs earg)
		{
			var appcfg = Ioc.Default.GetRequiredService<AppConfig>();
			await appcfg.SaveUserConfigAsync();
			_Logger.Info("OnAppExist");
		}

		// �����UI�߳�δ�����쳣
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			_Logger.Error(e.ExceptionObject as Exception, "CurrentDomain_UnhandledException:");
			//string eMsg = $"{_ExceptionMessage}\r\n{e.ExceptionObject.ToString()}";
			//MessageBox.Show(eMsg, "CurrentDomain Exception", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		// https://github.com/NLog/NLog/wiki/Configure-from-code
		private NLog.Config.LoggingConfiguration GetNLogConfiguration()
		{
			var config = new NLog.Config.LoggingConfiguration();
			// �ļ�
			var logfile = new NLog.Targets.FileTarget("logfile")
			{
				FileName = "logs/applog_${date:format=yyyy-MM-dd_HH}.txt",
				Layout = "[${longdate}|${level:uppercase=true}] ${message} ${all-event-properties} ${exception:format=tostring}",
				ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
				MaxArchiveFiles = 10,
				MaxArchiveDays = 7,
			};
			// �ն�
			var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
			{
				Layout = "[${longdate}|${level:uppercase=true}] ${message} ${all-event-properties} ${exception:format=tostring}}"
			};
			// Rules for mapping loggers to targets            
			config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);
			config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);
			return config;
		}
	}
}