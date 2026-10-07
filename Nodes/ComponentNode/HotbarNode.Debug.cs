using System.Diagnostics;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace KamiToolKit.Nodes;

/// <summary>
/// Partial class for HotbarNode to render debug information for this slot.
/// This file should be considered temporary and may disappear at any time.
/// </summary>
public partial class HotbarNode {

    private class SlotDebugWindow : Window {
        private readonly HotbarNode node;

        public unsafe SlotDebugWindow(HotbarNode node) : base($"HotbarNode Debug Data##{(nint)node.ResNode:x8}") {
            this.node = node;
            Size = new Vector2(300.0f, 550.0f);
            SizeCondition = ImGuiCond.FirstUseEver;
            IsOpen = true;
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar("Tabs");
            if (!tabBar) return;

            using (var tabItem = ImRaii.TabItem("HotbarSlot")) {
                if (tabItem) {
                    using var child = ImRaii.Child("child", ImGui.GetContentRegionAvail());
                    if (!child) return;

                    ImGui.Text($"ApparentSlotType: {node.hotbarData.ApparentSlotType}");
                    ImGui.Text($"PopUpHelp: {node.hotbarData.PopUpHelp}");
                    ImGui.Text($"ApparentActionId: {node.hotbarData.ApparentActionId}");
                    ImGui.Text($"CommandType: {node.hotbarData.CommandType}");
                    ImGui.Text($"ApparentActionMode: {node.hotbarData.ApparentActionMode}");
                    ImGui.Text($"ApparentActionModeParam: {node.hotbarData.ApparentActionModeParam}");
                    ImGui.Text($"CommandId: {node.hotbarData.CommandId}");
                    ImGui.Text($"CostDisplayMode: {node.hotbarData.CostDisplayMode}");
                    ImGui.Text($"CostTextString: {node.hotbarData.CostTextString}");
                    ImGui.Text($"CostType: {node.hotbarData.CostType}");
                    ImGui.Text($"CostValue: {node.hotbarData.CostValue}");
                    ImGui.Text($"IconId: {node.hotbarData.IconId}");
                    ImGui.Text($"IsEmpty: {node.hotbarData.IsEmpty}");
                    ImGui.Text($"IsLoaded: {node.hotbarData.IsLoaded}");
                    ImGui.Text($"KeybindHintString: {node.hotbarData.KeybindHintString}");
                    ImGui.Text($"OriginalApparentActionId: {node.hotbarData.OriginalApparentActionId}");
                    ImGui.Text($"OriginalApparentSlotType: {node.hotbarData.OriginalApparentSlotType}");
                    ImGui.Text($"PopUpKeybindHintString: {node.hotbarData.PopUpKeybindHintString}");
                    ImGui.Text($"RecipeCraftType: {node.hotbarData.RecipeCraftType}");
                    ImGui.Text($"RecipeDataLoaded: {node.hotbarData.RecipeDataLoaded}");
                    ImGui.Text($"RecipeItemId: {node.hotbarData.RecipeItemId}");
                    ImGui.Text($"RecipeValid: {node.hotbarData.RecipeValid}");
                    ImGui.Text($"UsesGeneralDragDropType: {node.hotbarData.UsesGeneralDragDropType}");
                }
            }

            using (var tabItem = ImRaii.TabItem("Intermediate")) {
                if (tabItem) {
                    using var child = ImRaii.Child("child", ImGui.GetContentRegionAvail());
                    if (!child) return;

                    ImGui.Text($"PopUpHelpText: {node.hotbarState.PopUpHelpText}");
                    ImGui.Text($"ActionAvailable1: {node.hotbarState.ActionAvailable1}");
                    ImGui.Text($"ActionAvailable2: {node.hotbarState.ActionAvailable2}");
                    ImGui.Text($"ActionId: {node.hotbarState.ActionId}");
                    ImGui.Text($"ActionTargetSatisfied: {node.hotbarState.ActionTargetSatisfied}");
                    ImGui.Text($"ChargePercent: {node.hotbarState.ChargePercent}");
                    ImGui.Text($"CooldownMode: {node.hotbarState.CooldownMode}");
                    ImGui.Text($"CooldownPercent: {node.hotbarState.CooldownPercent}");
                    ImGui.Text($"CooldownSeconds: {node.hotbarState.CooldownSeconds}");
                    ImGui.Text($"CostDisplayMode: {node.hotbarState.CostDisplayMode}");
                    ImGui.Text($"CostText: {node.hotbarState.CostText}");
                    ImGui.Text($"CostType: {node.hotbarState.CostType}");
                    ImGui.Text($"CostValue: {node.hotbarState.CostValue}");
                    ImGui.Text($"CurrentCharges: {node.hotbarState.CurrentCharges}");
                    ImGui.Text($"DrawAnts: {node.hotbarState.DrawAnts}");
                    ImGui.Text($"IconId: {node.hotbarState.IconId}");
                    ImGui.Text($"IsTransformationActionUsable: {node.hotbarState.IsTransformationActionUsable}");
                    ImGui.Text($"LastChargePercent: {node.hotbarState.LastChargePercent}");
                    ImGui.Text($"LastCooldownPercent: {node.hotbarState.LastCooldownPercent}");
                    ImGui.Text($"Type: {node.hotbarState.Type}");
                }
            }
        }

        public override void OnClose() {
            base.OnClose();

            windowSystem?.RemoveWindow(this);
        }
    }

    private static WindowSystem? windowSystem;
    private static int refCount;

    [Conditional("DEBUG")]
    internal void RegisterDebugWindow() {
        if (windowSystem is null) {
            windowSystem = new WindowSystem("KTK-HotbarNodeDebug");
            KamiToolKitLibrary.PluginInterface.UiBuilder.Draw += windowSystem.Draw;
        }

        refCount++;
        windowSystem.AddWindow(new SlotDebugWindow(this));
    }

    [Conditional("DEBUG")]
    internal static void UnregisterDebugWindow() {
        refCount--;

        if (refCount is 0 && windowSystem is not null) {
            KamiToolKitLibrary.PluginInterface.UiBuilder.Draw -= windowSystem.Draw;
            windowSystem.RemoveAllWindows();
            windowSystem = null;
        }
    }
}
