// ReSharper disable RedundantUnsafeContext
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

/// <summary>
/// Warning, anything in this class is subject to change at any time.
/// This is mostly a staging platform for features that haven't made it into live ClientStructs.
/// These are not intended for external use, other than for experimenting.
/// </summary>
public unsafe class Experimental {
    [StructLayout(LayoutKind.Explicit, Size = 0x1A8)]
    public struct ListItemRenderer {
        [FieldOffset(0x150)] public AtkResNode* EventNode;
        [FieldOffset(0x158)] public AtkResNode** CollisionNodeList;
        [FieldOffset(0x194)] public ushort CollisionNodeListCount;
    }
}
