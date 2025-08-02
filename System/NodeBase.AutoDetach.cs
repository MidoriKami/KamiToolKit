using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public unsafe partial class NodeBase {

    private bool IsAttached { get; set; }

    internal void RegisterAutoDetach(AtkUnitBase* addon) {
        if (IsAttached) {
            Log.Verbose("Tried to register auto detach, to already attached node.");
            return;
        }

        Log.Verbose($"Registering auto detach, setting up finalize listener for {addon->NameString} for node {GetType()}");
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addon->NameString, OnAddonFinalize);
        IsAttached = true;
    }

    internal void UnregisterAutoDetach() {
        if (!IsAttached) return;

        Log.Verbose($"Unregistering auto detach gracefully for node {GetType()}");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonFinalize);
        IsAttached = false;
    }

    private void OnAddonFinalize(AddonEvent type, AddonArgs args)
        => DetachNode();

    private void TryForceDetach(bool warn) {
        if (!IsAttached) return;

        if (warn) Log.Warning("Node was not gracefully detached. Forcing detach.");
        DetachNode();
    }
}
