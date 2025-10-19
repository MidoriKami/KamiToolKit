using System;
using System.Collections.Generic;
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
    private static readonly Dictionary<string, Dictionary<string, string>> EnumTranslationMap = new() {
        ["FFXIVClientStructs.FFXIV.Component.GUI.AlignmentType"] = new(StringComparer.OrdinalIgnoreCase) {
            ["TopLeft"] = "左上",
            ["Top"] = "上",
            ["TopCenter"] = "正上",
            ["TopRight"] = "右上",
            ["Left"] = "左",
            ["Center"] = "居中",
            ["Right"] = "右",
            ["BottomLeft"] = "左下",
            ["Bottom"] = "下",
            ["BottomCenter"] = "正下",
            ["BottomRight"] = "右下",
            ["Justify"] = "两端对齐",
            ["Unknown"] = "未知",
        },
        ["FFXIVClientStructs.FFXIV.Component.GUI.FontType"] = new(StringComparer.OrdinalIgnoreCase) {
            ["Axis"] = "Axis（常规）",
            ["Jupiter"] = "Jupiter（标题）",
            ["Emphasis"] = "Emphasis（强调）",
            ["Meidinger"] = "Meidinger（细体）",
            ["MeidingerItalic"] = "Meidinger（斜体）",
            ["MeidingerBold"] = "Meidinger（粗体）",
            ["MeidingerCaps"] = "Meidinger（小型大写）",
            ["Miedinger"] = "Miedinger（细体）",
            ["MiedingerItalic"] = "Miedinger（斜体）",
            ["MiedingerBold"] = "Miedinger（粗体）",
            ["MiedingerCaps"] = "Miedinger（小型大写）",
            ["TrumpGothic"] = "Trump Gothic",
            ["Unknown"] = "未知",
        },
        ["FFXIVClientStructs.FFXIV.Component.GUI.TextFlags"] = new(StringComparer.OrdinalIgnoreCase) {
            ["None"] = "无",
            ["Bold"] = "粗体",
            ["Italic"] = "斜体",
            ["Edge"] = "描边",
            ["Shadow"] = "阴影",
            ["Glow"] = "发光",
            ["Glare"] = "炫光",
            ["Emboss"] = "浮雕",
            ["MultiLine"] = "多行",
            ["WordWrap"] = "自动换行",
            ["Ellipsis"] = "自动省略",
            ["AutoAdjustNodeSize"] = "自动调整节点大小",
            ["AutoAdjustHeight"] = "自动调整高度",
            ["AutoAdjustWidth"] = "自动调整宽度",
            ["AutoAdjustFontSize"] = "自动调整字体",
            ["UseSdf"] = "使用 SDF 字体",
            ["FixedFontResolution"] = "固定字体分辨率",
            ["Unknown"] = "未知",
        },
        ["FFXIVClientStructs.FFXIV.Component.GUI.NodeFlags"] = new(StringComparer.OrdinalIgnoreCase) {
            ["Visible"] = "可见",
            ["Enabled"] = "启用交互",
            ["ClipChildren"] = "裁剪子节点",
            ["AnchorLeft"] = "左侧固定",
            ["AnchorTop"] = "顶部固定",
            ["AnchorRight"] = "右侧固定",
            ["AnchorBottom"] = "底部固定",
            ["Focusable"] = "可聚焦",
            ["Locked"] = "锁定",
            ["AutoArrange"] = "自动排列",
            ["Unknown"] = "未知",
        },
        ["FFXIVClientStructs.FFXIV.Component.GUI.DrawFlags"] = new(StringComparer.OrdinalIgnoreCase) {
            ["Visible"] = "可见",
            ["Enabled"] = "启用",
            ["Tint"] = "应用色调",
            ["Clip"] = "启用裁剪",
            ["UseDepth"] = "使用深度",
            ["Unknown"] = "未知",
        },
    };

    public static string GetDescription(this Enum value) {
        var type = value.GetType();
        var typeName = type.FullName ?? type.Name;

        if (EnumTranslationMap.TryGetValue(typeName, out var translationMap)) {
            var raw = value.ToString();
            if (raw.Contains(", ")) {
                var parts = raw.Split(", ", StringSplitOptions.RemoveEmptyEntries);
                var translatedParts = new List<string>(parts.Length);
                foreach (var part in parts) {
                    translatedParts.Add(translationMap.TryGetValue(part, out var translated) ? translated : part);
                }
                if (translatedParts.Count > 0) {
                    return string.Join("，", translatedParts);
                }
            }
            else if (translationMap.TryGetValue(raw, out var translatedValue)) {
                return translatedValue;
            }
        }

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
