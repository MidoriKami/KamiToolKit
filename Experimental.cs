// ReSharper disable RedundantUnsafeContext
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.InteropServices;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using InteropGenerator.Runtime;

namespace KamiToolKit;

/// <summary>
/// Warning, anything in this class is subject to change at any time.
/// This is mostly a staging platform for features that haven't made it into live ClientStructs.
/// These are not intended for external use, other than for experimenting.
/// </summary>
public unsafe class Experimental {

    public unsafe delegate void PrepareSlotForRender(
        RaptureHotbarModule* thisPtr,
        RaptureHotbarModule.HotbarSlot* slot,
        HotbarUiIntermediate* outIntermediate);

    [Signature("E8 ?? ?? ?? ?? FF C6 83 C5 11")]
    public static PrepareSlotForRender? UpdateHotbarSlotIntermediateData = null;

    [StructLayout(LayoutKind.Explicit, Size = 0x43)]
    public unsafe struct HotbarUiIntermediate {
        [FieldOffset(0x00)] public Utf8String* PopUpHelpText;   // to StringArray idx slotBase + 14
        [FieldOffset(0x08)] public CStringPointer CostTextPtr;  // to StringArray idx slotBase + 1
        [FieldOffset(0x10)] public uint IntermediateActionType; // to NumberArray idx slotBase + 0
        [FieldOffset(0x14)] public uint ActionId;               // to NumberArray idx slotBase + 3
        [FieldOffset(0x18)] public uint IconId;                 // to NumberArray idx slotBase + 4
        [FieldOffset(0x1C)] public uint CooldownMode;           // to NumberArray idx slotBase + 7
        [FieldOffset(0x20)] public uint CooldownSeconds;
        [FieldOffset(0x24)] public uint CooldownPercent; // to NumberArray idx slotBase + 8
        [FieldOffset(0x28)] public uint LastCooldownPercent;
        [FieldOffset(0x2C)] public uint ChargePercent; // to NumberArray idx slotBase + 9
        [FieldOffset(0x30)] public uint LastChargePercent;
        [FieldOffset(0x34)] public uint CurrentCharges;        // to NumberArray idx slotBase + 13
        [FieldOffset(0x38)] public uint CostValue;             // to NumberArray idx slotBase + 10
        [FieldOffset(0x3C)] public byte CostType;              // to NumberArray idx slotBase + 1
        [FieldOffset(0x3D)] public byte CostDisplayMode;       // to NumberArray idx slotBase + 2
        [FieldOffset(0x3E)] public bool ActionAvailable1;      // to NumberArray idx slotBase + 5
        [FieldOffset(0x3F)] public bool ActionAvailable2;      // to NumberArray idx slotBase + 6
        [FieldOffset(0x40)] public bool ActionTargetSatisfied; // to NumberArray idx slotBase + 15
        [FieldOffset(0x41)] public bool DrawAnts;              // to NumberArray idx slotBase + 14
        [FieldOffset(0x42)] public byte Unk0x42;
    }
}
