using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace KamiToolKit.Extensions;

public static unsafe class FrameworkExtensions {
    public static bool IsUnloading(ref this Framework framework) {
        fixed (Framework* pointer = &framework) {
            return Marshal.ReadByte((nint)pointer, 0x08) == 1;
        }
    }
}
