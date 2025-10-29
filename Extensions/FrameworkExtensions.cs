using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace KamiToolKit.Extensions;

public static class FrameworkExtensions {
    public static bool IsUnloading(ref this Framework framework)
        => framework.IsDestroying;
}
