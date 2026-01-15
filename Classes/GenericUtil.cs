using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KamiToolKit.Classes;

internal static class GenericUtil {
    public static bool AreEqual<T>(T? left, T? right) {
        if (default(T) == null) return ReferenceEquals(left, right);

        if (left == null || right == null) return left == null && right == null;

        var leftSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref left), Unsafe.SizeOf<T>());
        var rightSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref right), Unsafe.SizeOf<T>());

        return leftSpan.SequenceEqual(rightSpan);
    }
}
