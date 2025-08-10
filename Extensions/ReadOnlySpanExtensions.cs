using System;
using System.Text;

namespace KamiToolKit.Extensions;

public static class ReadOnlySpanExtensions {
    public static string GetString(this ReadOnlySpan<byte> span)
        => Encoding.UTF8.GetString(span);
}
