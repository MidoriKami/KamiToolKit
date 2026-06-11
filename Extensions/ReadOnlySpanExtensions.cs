using System;
using System.Text;

namespace KamiToolKit.Extensions;

/// <summary>
/// ReadOnlySpan extensions.
/// </summary>
public static class ReadOnlySpanExtensions {
    extension(ReadOnlySpan<byte> span) {

        /// <summary>
        /// Gets the span as a UTF8String
        /// </summary>
        public string String
            => Encoding.UTF8.GetString(span);
    }
}
