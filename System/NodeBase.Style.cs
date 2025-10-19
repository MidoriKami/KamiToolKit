using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using KamiToolKit.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KamiToolKit.System;

public partial class NodeBase {

    private List<PropertyInfo> taggedFields = [];

    private static readonly Dictionary<string, string> PropertyDisplayNameMap = new() {
        { "Position", "位置" },
        { "Size", "尺寸" },
        { "Scale", "缩放" },
        { "Rotation", "旋转" },
        { "RotationDegrees", "旋转角度" },
        { "Origin", "原点" },
        { "IsVisible", "是否可见" },
        { "Color", "颜色" },
        { "AddColor", "附加颜色" },
        { "MultiplyColor", "乘色" },
        { "NodeFlags", "节点标志" },
        { "DrawFlags", "绘制标志" },
        { "ClipCount", "裁剪数量" },
        { "Priority", "优先级" },
        { "TextColor", "文字颜色" },
        { "TextOutlineColor", "文字描边颜色" },
        { "BackgroundColor", "背景颜色" },
        { "AlignmentType", "对齐方式" },
        { "FontType", "字体" },
        { "TextFlags", "文本标志" },
        { "FontSize", "字体大小" },
        { "LineSpacing", "行距" },
        { "CharSpacing", "字距" },
        { "String", "文本内容" },
        { "TooltipString", "提示文本" },
        { "TextId", "文本 ID" },
        { "Tooltip", "提示文本" },
        { "Icon", "图标" },
        { "IconId", "图标 ID" },
        { "TexturePath", "材质路径" },
        { "TextureCoordinates", "材质坐标" },
        { "TextureSize", "材质尺寸" },
        { "LeftOffset", "左侧偏移" },
        { "RightOffset", "右侧偏移" },
        { "TopOffset", "顶部偏移" },
        { "BottomOffset", "底部偏移" },
        { "Padding", "内边距" },
        { "Margin", "外边距" },
        { "Spacing", "间距" },
        { "LineHeight", "行高" },
        { "LineWidth", "线宽" },
        { "LineThickness", "线条粗细" },
        { "LineColor", "线条颜色" },
        { "HighlightColor", "高亮颜色" },
        { "FillColor", "填充颜色" },
        { "OutlineColor", "描边颜色" },
        { "GaugeColor", "计量条颜色" },
        { "GaugeEdgeColor", "计量条边缘颜色" },
    };

    private static string GetPropertyDisplayName(PropertyInfo info)
        => PropertyDisplayNameMap.TryGetValue(info.Name, out var name) ? name : info.Name;

    /// <summary>
    ///     Setting these properties will prevent Load operations from setting those properties.
    /// </summary>
    /// <remarks>This only applies to the object that .Load is being called on.</remarks>
    /// <example>Setting "Position" will prevent a position value from the json from overwriting the nodes position</example>
    public virtual List<string> OnLoadOmittedProperties { get; set; } = [];

    private bool TagListGenerated { get; set; }
    private IOrderedEnumerable<PropertyInfo> OrderedPrimitives => taggedFields.OrderByDescending(property => property.PropertyType.Name);

    public void Save(string filePath) {
        try {
            Log.Debug($"[NodeBase] 正在保存 {GetShortPath(filePath)}");
            var fileText = JsonConvert.SerializeObject(this, Formatting.Indented);
            FilesystemUtil.WriteAllTextSafe(filePath, fileText);
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    public void Load(string filePath) {
        try {
            Log.Debug($"[NodeBase] 正在加载 {GetShortPath(filePath)}");
            var fileData = File.ReadAllText(filePath);
            if (OnLoadOmittedProperties.Count != 0) {
                var jObject = JObject.Parse(fileData);
                foreach (var property in OnLoadOmittedProperties) {
                    jObject.Remove(property);
                }
                fileData = jObject.ToString();
            }

            if (!fileData.IsNullOrEmpty()) {
                JsonConvert.PopulateObject(fileData, this);
                MarkDirty();
            }
        }
        catch (FileNotFoundException) {
            Log.Debug("[NodeBase] 未找到文件，正在创建新文件。");
            Save(filePath);
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    public void Load(NodeBase otherNode, params string[] omittedProperties) {
        var serializedOther = JsonConvert.SerializeObject(otherNode, Formatting.Indented);
        var reserialized = (JObject?)JsonConvert.DeserializeObject(serializedOther);
        if (reserialized is not null) {
            foreach (var property in omittedProperties) {
                reserialized.Remove(property);
            }

            JsonConvert.PopulateObject(reserialized.ToString(), this);
        }
    }

    private static string GetShortPath(string filePath) {
        var pluginDirectoryPath = DalamudInterface.Instance.PluginInterface.ConfigDirectory.FullName;

        if (filePath.StartsWith(pluginDirectoryPath, StringComparison.OrdinalIgnoreCase)) {
            return filePath[pluginDirectoryPath.Length..].TrimStart(Path.DirectorySeparatorChar);
        }

        return filePath;
    }

    private void DrawTaggedFields() {
        foreach (var primitive in OrderedPrimitives) {
            DrawConfigForType(primitive);
        }
    }

    private void GenerateTypeList(Type type) {
        foreach (var memberInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            if (memberInfo is not { MemberType: MemberTypes.Field or MemberTypes.Property }) continue;
            if (!memberInfo.GetCustomAttributesData().Any(attribute => attribute.AttributeType == typeof(JsonPropertyAttribute))) continue;

            taggedFields.Add(memberInfo);
        }
    }

    private void GeneratePropertyList() {
        var stopWatch = Stopwatch.StartNew();

        if (!TagListGenerated) {
            GenerateTypeList(GetType());
            TagListGenerated = true;
            Log.Debug($"属性树生成耗时 {stopWatch.Elapsed}");
        }
    }

    private void DrawConfigForType(PropertyInfo info) {
        ImGui.TableNextColumn();
        ImGui.Text(GetPropertyDisplayName(info));

        ImGui.TableNextColumn();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

        if (info.PropertyType == typeof(Vector4)) {
            var value = (Vector4)info.GetValue(this)!;
            if (ImGui.ColorEdit4($"##{info.Name}", ref value, ImGuiColorEditFlags.AlphaPreviewHalf)) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(Vector3)) {
            var value = (Vector3)info.GetValue(this)!;
            if (ImGui.ColorEdit3($"##{info.Name}", ref value, ImGuiColorEditFlags.AlphaPreviewHalf)) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(Vector2)) {
            var value = (Vector2)info.GetValue(this)!;
            if (ImGui.InputFloat2($"##{info.Name}", ref value)) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(float)) {
            var value = (float)info.GetValue(this)!;
            if (ImGui.InputFloat($"##{info.Name}", ref value)) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(bool)) {
            var value = (bool)info.GetValue(this)!;
            if (ImGui.Checkbox($"##{info.Name}", ref value)) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(uint)) {
            var value = Convert.ToInt32(info.GetValue(this)!);
            if (ImGui.InputInt($"##{info.Name}", ref value)) {
                info.SetValue(this, (uint)value);
            }
        }
        else if (info.PropertyType.IsEnum) {
            var hasFlags = info.PropertyType.GetCustomAttribute<FlagsAttribute>() != null;

            var value = (Enum)info.GetValue(this)!;
            if (ComboHelper.EnumCombo($"##{info.Name}", ref value, hasFlags)) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(string)) {
            var standardHeight = ImGuiHelpers.GetButtonSize("A").Y;

            var value = (string)info.GetValue(this)!;
            if (ImGui.InputTextMultiline($"##{info.Name}", ref value, 2000, ImGui.GetContentRegionAvail() with {
                    Y = standardHeight * 2.0f,
                })) {
                info.SetValue(this, value);
            }
        }
        else if (info.PropertyType == typeof(Spacing)) {
            var rawValue = (Spacing)info.GetValue(this)!;
            Vector4 vector = rawValue;

            if (ImGui.InputFloat4($"##{info.Name}", ref vector)) {
                Spacing spacing = vector;
                info.SetValue(this, spacing);
            }
        }
        else {
            using (ImRaii.PushColor(ImGuiCol.Text, KnownColor.Orange.Vector())) {
                ImGui.Text($"未定义的属性类型：{info.PropertyType} {info.Name}");
            }
        }
    }

    public virtual void DrawConfig() {
        GeneratePropertyList();

        using var table = ImRaii.Table("basic_property_table", 2);
        if (table) {

            ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
            ImGui.TableSetupColumn("##configuration", ImGuiTableColumnFlags.WidthStretch, 2.0f);

            ImGui.TableNextRow();

            DrawTaggedFields();
        }
    }
}
