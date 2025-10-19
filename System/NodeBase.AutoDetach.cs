using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public unsafe partial class NodeBase {

    private bool IsAttached { get; set; }

    internal void RegisterAutoDetach(AtkUnitBase* addon) {
        if (IsAttached) {
            Log.Verbose("尝试为已附加的节点注册自动分离，操作已忽略。");
            return;
        }

        Log.Verbose($"正在为节点 {GetType()} 注册自动分离，监听 {addon->NameString} 的 Finalize 事件。");
        DalamudInterface.Instance.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addon->NameString, OnAddonFinalize);
        IsAttached = true;
    }

    internal void UnregisterAutoDetach() {
        if (!IsAttached) return;

        Log.Verbose($"正在为节点 {GetType()} 平稳注销自动分离。");
        DalamudInterface.Instance.AddonLifecycle.UnregisterListener(OnAddonFinalize);
        IsAttached = false;
    }

    private void OnAddonFinalize(AddonEvent type, AddonArgs args)
        => DetachNode();

    private void TryForceDetach(bool warn) {
        if (!IsAttached) return;

        if (warn) Log.Warning("节点未能正常分离，已强制执行分离操作。");
        DetachNode();
    }
}
