using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.Services
{
	public sealed class CommonServices<TModel> : ICommonServices<TModel>
	{
		public CommonServices(MessageService messager, ILogger<TModel> logger, MainDialogHost dialog)
		{
			MessageService = messager;
			Logger = logger;
			DialogHost = dialog;
		}
		public MessageService MessageService { get; }
		public ILogger<TModel> Logger { get; }
		public MainDialogHost DialogHost { get; }
	}
}
