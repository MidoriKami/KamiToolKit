using System.Numerics;

namespace KamiToolKit.Classes;

/// <summary>
/// Helper class for reading and setting bitflags.
/// </summary>
public static class FlagHelper {

    /// <summary>
    /// Read the flag value of bit index.
    /// </summary>
    public static bool ReadFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
        => (flagsField & T.One << BitOperations.Log2((uint)flag)) != T.Zero;

    /// <summary>
    /// Read the flag value of bit index.
    /// </summary>
    public static void SetFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
        => flagsField |= T.One << BitOperations.Log2((uint)flag);

    /// <summary>
    /// Clears the flag value of bit index.
    /// </summary>
    public static void ClearFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
        => flagsField &= ~(T.One << BitOperations.Log2((uint)flag));

    /// <summary>
    /// Sets of Clears the flag value of bit index with the value of enable.
    /// </summary>
    public static void UpdateFlag<T>(ref T flagsField, int flag, bool enable) where T : struct, IBinaryInteger<T> {
        if (enable) {
            SetFlag(ref flagsField, flag);
        }
        else {
            ClearFlag(ref flagsField, flag);
        }
    }
}
