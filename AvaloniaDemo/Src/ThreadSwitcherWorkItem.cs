using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaDemo.Src
{
	internal class ThreadSwitcherWorkItem : IThreadPoolWorkItem
	{
		static readonly Action EmptyAction = () => { };
		static readonly Stack<ThreadSwitcherWorkItem> Cache = new Stack<ThreadSwitcherWorkItem>();

		public Action Continuation { get; private set; } = EmptyAction;

		public static ThreadSwitcherWorkItem GetOrCreate(Action action)
		{
			lock (Cache) {
				if (Cache.Count == 0) {
					return new ThreadSwitcherWorkItem { Continuation = action };
				}
				else {
					var worker = Cache.Pop();
					worker.Continuation = action;
					return worker;
				}
			}
		}

		public void Execute()
		{
			var continuation = Continuation;
			Continuation = EmptyAction;
			lock (Cache)
				Cache.Push(this);
			continuation();
		}
	}
}
