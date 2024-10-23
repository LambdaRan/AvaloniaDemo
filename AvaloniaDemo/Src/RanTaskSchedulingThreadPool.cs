using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaDemo.Src
{
	public sealed class RanTaskSchedulingThreadPool
	{
		private readonly ILogger _Logger;
		private readonly CancellationTokenSource shutdownCancellation = new CancellationTokenSource();
		private CountdownEvent runningTasksCountdown = null!;
		private Action<Task> completeTask = null!;
		private SemaphoreSlim concurrencySemaphore = null!;

		private int maxConcurrency;
		public const int DefaultMaxConcurrency = 10;

		private TaskScheduler scheduler = null!;
		private bool isInitialized;

		public TaskScheduler Scheduler {
			get => scheduler;
			set {
				if (!isInitialized) scheduler = value;
			}
		}

		private TaskScheduler GetDefaultScheduler()
		{
			return TaskScheduler.Default;
		}

		public int MaxConcurrency {
			get => maxConcurrency;
			set {
				if (!isInitialized) maxConcurrency = value;
			}
		}

		public int ThreadCount {
			get => MaxConcurrency;
			set => MaxConcurrency = value;
		}


		public string InstanceId { get; set; } = null!;

		public string InstanceName { get; set; } = null!;

		public RanTaskSchedulingThreadPool() : this(DefaultMaxConcurrency, NullLoggerFactory.Instance)
		{
		}

		public RanTaskSchedulingThreadPool(int maxConcurrency, ILoggerFactory loggerFactory)
		{
			_Logger = loggerFactory.CreateLogger<RanTaskSchedulingThreadPool>();
			MaxConcurrency = maxConcurrency;
		}

		public void Initialize()
		{
			if (Scheduler == null) {
				Scheduler = GetDefaultScheduler();
			}
			concurrencySemaphore = new SemaphoreSlim(MaxConcurrency);
			runningTasksCountdown = new CountdownEvent(1);
			completeTask = SignalTaskComplete;
			isInitialized = true;

			_Logger.LogInformation("TaskSchedulingThreadPool configured with max concurrency of {MaxConcurrency} and TaskScheduler {SchedulerName}.",
				MaxConcurrency, Scheduler.GetType().Name);
		}

		public int BlockForAvailableThreads()
		{
			if (isInitialized && !shutdownCancellation.IsCancellationRequested) {
				try {
					concurrencySemaphore.Wait(shutdownCancellation.Token);
					return 1 + concurrencySemaphore.Release();
				}
				catch (OperationCanceledException) {
				}
			}
			return 0;
		}

		public bool RunInThread(Func<Task> runnable)
		{
			if (runnable == null || !isInitialized || shutdownCancellation.IsCancellationRequested) return false;

			// Acquire the semaphore (return false if shutdown occurs while waiting)
			try {
				concurrencySemaphore.Wait(shutdownCancellation.Token);
			}
			catch (OperationCanceledException) {
				return false;
			}

			// Wrap the runnable in a Task to start it asynchronously
			var task = new Task<Task>(runnable);

			// Unrap the task so that we can work with the underlying task
			var unwrappedTask = task.Unwrap();

			lock (runningTasksCountdown) {
				// Now that the lock is held, shutdown can't proceed,
				// so double-check that no shutdown has started since the initial check.
				if (shutdownCancellation.IsCancellationRequested) {
					concurrencySemaphore.Release();
					return false;
				}

				// Record an extra running task
				runningTasksCountdown.AddCount();
			}

			// Register a callback to remove the task from the running list once it has completed
#pragma warning disable MA0134
			unwrappedTask.ContinueWith(completeTask);
#pragma warning restore MA0134

			// Start the task using the task scheduler
			task.Start(Scheduler);

			return true;
		}

		private void SignalTaskComplete(Task completedTask)
		{
			concurrencySemaphore.Release();
			runningTasksCountdown.Signal();
		}

		public void Shutdown(bool waitForJobsToComplete = true)
		{
			_Logger.LogDebug("Shutting down threadpool...");

			// Cancel using our shutdown token
			shutdownCancellation.Cancel();

			// If waitForJobsToComplete is true, wait for running tasks to complete
			if (waitForJobsToComplete) {
				lock (runningTasksCountdown) {
					// Cancellation has been signaled, so no new tasks will begin once
					// shutdown has acquired this lock
					_Logger.LogDebug("Waiting for {ThreadCount} threads to complete.", runningTasksCountdown.CurrentCount.ToString());
				}

				// Signal the initial count that we used to make sure the CountDownEvent didn't start
				// in "signaled" state
				runningTasksCountdown.Signal();

				// Wait for pending tasks to complete
				runningTasksCountdown.Wait();

				_Logger.LogDebug("No executing jobs remaining, all threads stopped.");
			}

			_Logger.LogDebug("Shutdown of threadpool complete.");
		}
	}
}
