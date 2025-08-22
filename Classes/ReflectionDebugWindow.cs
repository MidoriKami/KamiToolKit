using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace KamiToolKit.Classes;

public class ReflectionDebugWindow() : Window("Reflection Debug Window") {

    public static void Open() {
        var newDebugWindow = new ReflectionDebugWindow();
        DalamudInterface.Instance.PluginInterface.UiBuilder.Draw += newDebugWindow.Draw;
        newDebugWindow.IsOpen = true;
    }

    public override void Draw() {
        foreach (var (type, subtypes) in NativeController.ChildMembers) {
            using var header = ImRaii.TreeNode(type.ToString());
            if (header) {
                foreach (var subtype in subtypes) {
                    ImGui.Text(subtype.Name);
                }
            }
        }
    }

    public override void OnClose() {
        DalamudInterface.Instance.PluginInterface.UiBuilder.Draw -= Draw;
    }
}
