using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using KamiToolKit.BaseTypes;

namespace KamiToolKit.Debug;

internal static class NodeDebugTab {
    public static unsafe void Draw() {
        using var tabItem = ImRaii.TabItem("Nodes");
        if (!tabItem) return;

        using var table = ImRaii.Table("NodeTable", 3, ImGuiTableFlags.ScrollY);
        if (!table) return;

        ImGui.TableSetupScrollFreeze(3, 1);

        ImGui.TableSetupColumn("Node Type");
        ImGui.TableSetupColumn("Parent Addon");
        ImGui.TableSetupColumn("Parent AtkUldManager");

        ImGui.TableHeadersRow();

        foreach (var node in NodeBase.CreatedNodes) {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(node.GetType().Name);

            ImGui.TableNextColumn();
            ImGui.Text($"{(nint)node.ParentAddon:X8}");

            ImGui.TableNextColumn();
            ImGui.Text($"{(nint)node.ParentUldManager:X8}");
        }
    }
}
