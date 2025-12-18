using System;
using System.Text;

namespace KamiToolKit.Extensions;

public static class ReadOnlySpanExtensions {
    extension(ReadOnlySpan<byte> span) {
        public string String => Encoding.UTF8.GetString(span);
    }
}
