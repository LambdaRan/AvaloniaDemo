using System;
using System.Diagnostics;

namespace AvaloniaDemo.Utils
{
	public struct ValueStopwatch
	{
		public static ValueStopwatch StartNew()
		{
			return new ValueStopwatch { startedAt = Stopwatch.GetTimestamp() };
		}

		public static ValueStopwatch WithTime(TimeSpan time)
		{
			var offset = Stopwatch.Frequency == TimeSpan.TicksPerSecond
				? time.Ticks
				: time.Ticks * ((double)Stopwatch.Frequency / TimeSpan.TicksPerSecond);
			return new ValueStopwatch
			{
				startedAt = Stopwatch.GetTimestamp() - (long)offset
			};
		}

		long elapsed;
		long startedAt;

		public TimeSpan Elapsed {
			get {
				long totalElapsed = elapsed;
				if (IsRunning)
					totalElapsed += Stopwatch.GetTimestamp() - startedAt;

				return Stopwatch.Frequency == TimeSpan.TicksPerSecond
						? TimeSpan.FromTicks(totalElapsed)
						: TimeSpan.FromTicks((long)(totalElapsed / ((double)Stopwatch.Frequency / TimeSpan.TicksPerSecond)));
			}
		}

		public long ElapsedMilliseconds => (long)Elapsed.TotalMilliseconds;

		public long ElapsedTicks => Elapsed.Ticks;

		public bool IsRunning => startedAt != 0;

		public string TotalSeconds => $"{Elapsed.TotalSeconds:#0.000}";

		public void Reset()
		{
			elapsed = 0;
			startedAt = 0;
		}

		public void Restart()
		{
			elapsed = 0;
			startedAt = Stopwatch.GetTimestamp();
		}

		public void Start()
		{
			if (!IsRunning)
				startedAt = Stopwatch.GetTimestamp();
		}

		public void Stop()
		{
			if (IsRunning) {
				elapsed += Stopwatch.GetTimestamp() - startedAt;
				startedAt = 0;
			}
		}
		public string StopReturnUseSeconds()
		{
			Stop();
			return TotalSeconds;
		}
	}
}
