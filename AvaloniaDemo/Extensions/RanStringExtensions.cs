using System;

namespace AvaloniaDemo.Extensions
{
	public static class RanStringExtensions
	{
		public static bool EqualsOrdinal(this string left, string? value) {
			return left.Equals(value, StringComparison.Ordinal);
		}
		public static bool EqualsOrdinalIgnoreCase(this string left, string? value)
		{
			return left.Equals(value, StringComparison.OrdinalIgnoreCase);
		}

		public static bool StartsWithOrdinal(this string left, string value)
			=> left.StartsWith(value, StringComparison.Ordinal);
		public static bool StartsWithOrdinalIgnoreCase(this string left, string value)
			=> left.StartsWith(value, StringComparison.OrdinalIgnoreCase);
	}
}
