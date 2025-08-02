using System.Numerics;

namespace KamiToolKit.Classes;

public static class FlagHelper {
    public static bool ReadFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
        => (flagsField & T.One << BitOperations.Log2((uint)flag)) != T.Zero;

    public static void SetFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
        => flagsField |= T.One << BitOperations.Log2((uint)flag);

    public static void ClearFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
        => flagsField &= ~(T.One << BitOperations.Log2((uint)flag));

    public static void UpdateFlag<T>(ref T flagsField, int flag, bool enable) where T : struct, IBinaryInteger<T> {
        if (enable) {
            SetFlag(ref flagsField, flag);
        }
        else {
            ClearFlag(ref flagsField, flag);
        }
    }
}
