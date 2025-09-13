using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        Dictionary<Type, List<MemberInfo>> combinedTypes = [];

        foreach (var (type, members) in NativeController.ChildMembers) {
            combinedTypes.TryAdd(type, []);

            foreach (var member in members) {
                combinedTypes[type].Add(member);
            }
        }

        foreach (var (type, members) in NativeController.EnumerableMembers) {
            combinedTypes.TryAdd(type, []);

            foreach (var member in members) {
                combinedTypes[type].Add(member);
            }
        }
        
        foreach (var (type, subtypes) in combinedTypes.OrderBy(pair => pair.Key.Name)) {
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
