using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.Utils
{
   /// <summary>
   /// Tokenizes a <see cref="string"/> into <see cref="StringSegment"/>s.
   /// </summary>
	public readonly struct RanStringTokenizer : IEnumerable<StringSegment>
	{
		private readonly StringSegment _value;
		private readonly string _separators;
		/// <summary>
		/// Initializes a new instance of <see cref="StringTokenizer"/>.
		/// </summary>
		/// <param name="value">The <see cref="string"/> to tokenize.</param>
		/// <param name="separator">The string to tokenize by.</param>
		public RanStringTokenizer(string value, string separator)
		{
			Guard.IsNotNullOrEmpty(value);
			Guard.IsNotNullOrEmpty(separator);
			_value = value;
			_separators = separator;
		}
		public RanStringTokenizer(StringSegment value, string separator)
		{
			Guard.IsTrue(value.HasValue);
			Guard.IsNotNullOrEmpty(separator);
			_value = value;
			_separators = separator;
		}

		public Enumerator GetEnumerator() => new Enumerator(in _value, _separators);

		IEnumerator<StringSegment> IEnumerable<StringSegment>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Enumerates the <see cref="string"/> tokens represented by <see cref="StringSegment"/>.
		/// </summary>
		public struct Enumerator : IEnumerator<StringSegment>
		{
			private readonly StringSegment _value;
			private readonly string _separators;
			private int _index;

			internal Enumerator(in StringSegment value, string separators)
			{
				_value = value;
				_separators = separators;
				Current = default;
				_index = 0;
			}
			public Enumerator(ref RanStringTokenizer tokenizer)
			{
				_value = tokenizer._value;
				_separators = tokenizer._separators;
				Current = default(StringSegment);
				_index = 0;
			}
			public StringSegment Current { get; private set; }

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			/// <summary>
			/// Advances the enumerator to the next token in the <see cref="StringTokenizer"/>.
			/// </summary>
			/// <returns><see langword="true"/> if the enumerator was successfully advanced to the next token; <see langword="false"/> if the enumerator has passed the end of the <see cref="StringTokenizer"/>.</returns>
			public bool MoveNext()
			{
				if (!_value.HasValue || _index > _value.Length) {
					Current = default(StringSegment);
					return false;
				}
				int next = _value.AsSpan(_index).IndexOf(_separators, StringComparison.Ordinal);
				//int next = _value.IndexOfAny(_separators, _index);
				if (next == -1) {
					// No separator found. Consume the remainder of the string.
					next = _value.Length;
				}
				else {
					next = _index + next;
				}

				Current = _value.Subsegment(_index, next - _index);
				_index = next + _separators.Length;
				return true;
			}
			public void Reset()
			{
				Current = default(StringSegment);
				_index = 0;
			}
		}
	}
}
