using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using KamiToolKit.BaseTypes;

namespace KamiToolKit.Debug;

internal static class AddonDebugTab {
    public static unsafe void Draw() {
        using var tabItem = ImRaii.TabItem("Addons");
        if (!tabItem) return;

        using var table = ImRaii.Table("AddonTable", 2, ImGuiTableFlags.ScrollY);
        if (!table) return;

        ImGui.TableSetupScrollFreeze(2, 1);

        ImGui.TableSetupColumn("Addon Type");
        ImGui.TableSetupColumn("Addon Pointer");

        ImGui.TableHeadersRow();

        foreach (var addon in NativeAddon.CreatedAddons) {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(addon.GetType().Name);

            ImGui.TableNextColumn();
            ImGui.Text($"{(nint)addon.InternalAddon:X8}");
        }
    }
}
