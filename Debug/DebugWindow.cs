using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace KamiToolKit.Debug;

internal class DebugWindow : Window {

    public DebugWindow() : base($"KamiToolKit Debug Window - {KamiToolKitLibrary.PluginInterface.InternalName}") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300.0f, 300.0f),
        };
    }

    public override void Draw() {
        using var tabBar = ImRaii.TabBar("DebugTabsTabBar");
        if (!tabBar) return;

        NodeDebugTab.Draw();
        AddonDebugTab.Draw();
    }
}
