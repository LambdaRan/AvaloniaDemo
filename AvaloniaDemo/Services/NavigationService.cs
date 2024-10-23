using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using AvaloniaDemo.ViewModels;
using AvaloniaDemo.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AvaloniaDemo.Services
{
    public class NavigationService
    {
        private static readonly ConcurrentDictionary<Type, Type> _ViewModelMap = new ConcurrentDictionary<Type, Type>();

        public static void Register<TViewModel, TView>() where TView : UserControl
        {
            if (!_ViewModelMap.TryAdd(typeof(TViewModel), typeof(TView)))
            {
                throw new InvalidOperationException($"ViewModel already registered '{typeof(TViewModel).FullName}'");
            }
        }
        public static Type GetView<TViewModel>()
        {
            return GetView(typeof(TViewModel));
        }

		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		public static Type GetView(Type viewModel)
        {
            if (_ViewModelMap.TryGetValue(viewModel, out Type? view))
            {
                return view;
            }
            throw new InvalidOperationException($"View not registered for ViewModel '{viewModel.FullName}'");
        }
        public static Type GetViewModel(Type view)
        {
            var type = _ViewModelMap.Where(r => r.Value == view).Select(r => r.Key).FirstOrDefault();
            if (type == null)
            {
                throw new InvalidOperationException($"View not registered for ViewModel '{view.FullName}'");
            }
            return type;
        }
        public static object GetViewInstance<TViewModel>()
        {
            return GetViewInstance(typeof(TViewModel));
        }
        public static object GetViewInstance(Type viewModel)
        {
            if (viewModel == null)
            {
                return new TextBlock() { Text = "ViewModel was null!" };
            }

            return CreateViewInstance(GetView(viewModel));
        }
        public static object CreateViewInstance(Type view)
        {
            if (view == null)
            {
                return new TextBlock() { Text = "View was null!" };
            }
            return Activator.CreateInstance(view)!;
        }

		public static async Task CreateNewViewAsync<TViewModel>(object? parameter = null)
		{
			await CreateNewViewAsync(typeof(TViewModel), parameter);
		}
		public static async Task CreateNewViewAsync(Type viewModelType, object? parameter = null)
		{
			await Dispatcher.UIThread.InvokeAsync(() => {
                var window = new Window()
                {
                    Title = "test",
					ShowActivated = true,
				};
                window.Content = GetViewInstance(viewModelType);
				if (App.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                    window.Show(desktop.MainWindow!);
				}
			}, DispatcherPriority.Normal);
		}
    }
}
