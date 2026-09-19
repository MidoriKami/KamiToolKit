using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for HotbarSlot to indicate if the slot actually contains meaningful data.
/// </summary>
public static class HotbarSlotExtensions {
    extension(RaptureHotbarModule.HotbarSlot slot) {

        /// <summary>
        /// Gets if this slot is valid and contains meaningful data.
        /// </summary>
        public bool IsValid {
            get {

                if (slot.ApparentSlotType is RaptureHotbarModule.HotbarSlotType.Empty) return false;
                if (slot.CommandType is RaptureHotbarModule.HotbarSlotType.Empty) return false;
                if (slot.OriginalApparentSlotType is RaptureHotbarModule.HotbarSlotType.Empty) return false;
                if (slot.CommandId == uint.MaxValue) return false;

                return true;
            }
        }
    }
}
