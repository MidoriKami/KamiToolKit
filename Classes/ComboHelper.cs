using System;
using System.ComponentModel;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace KamiToolKit.Classes;

public static class ComboHelper {
    public static bool EnumCombo<T>(string label, ref T refValue, bool flagCombo = false) where T : Enum {
        using var combo = ImRaii.Combo(label, refValue.GetDescription());
        if (!combo) return false;

        if (flagCombo) {
            foreach (Enum enumValue in Enum.GetValues(refValue.GetType())) {
                if (ImGui.Selectable(enumValue.GetDescription(), refValue.HasFlag(enumValue))) {
                    if (!refValue.HasFlag(enumValue)) {
                        var intRefValue = Convert.ToInt32(refValue);
                        var intFlagValue = Convert.ToInt32(enumValue);
                        var result = intRefValue | intFlagValue;
                        refValue = (T)Enum.ToObject(refValue.GetType(), result);
                    }
                    else {
                        var intRefValue = Convert.ToInt32(refValue);
                        var intFlagValue = Convert.ToInt32(enumValue);
                        var result = intRefValue & ~intFlagValue;
                        refValue = (T)Enum.ToObject(refValue.GetType(), result);
                    }

                    return true;
                }
            }
        }
        else {
            foreach (Enum enumValue in Enum.GetValues(refValue.GetType())) {
                if (!ImGui.Selectable(enumValue.GetDescription(), enumValue.Equals(refValue))) continue;
                refValue = (T)enumValue;
                return true;
            }
        }

        return false;
    }
}

internal static class EnumExtensions {
    public static string GetDescription(this Enum value) {
        var type = value.GetType();
        if (Enum.GetName(type, value) is { } name) {
            if (type.GetField(name) is { } field) {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) {
                    return attr.Description;
                }
            }
        }

        return value.ToString();
    }
}
