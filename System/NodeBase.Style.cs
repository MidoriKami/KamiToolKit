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
            Log.Debug($"[NodeBase] Saving {GetShortPath(filePath)}");
            var fileText = JsonConvert.SerializeObject(this, Formatting.Indented);
            FilesystemUtil.WriteAllTextSafe(filePath, fileText);
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    public void Load(string filePath) {
        try {
            Log.Debug($"[NodeBase] Loading {GetShortPath(filePath)}");
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
            Log.Debug("[NodeBase] File not found, creating a new one.");
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

    private string GetShortPath(string filePath) {
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
            Log.Debug($"Attribute Tree Generated in {stopWatch.Elapsed}");
        }
    }

    private void DrawConfigForType(PropertyInfo info) {
        ImGui.TableNextColumn();
        ImGui.Text(info.Name);

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
                ImGui.Text($"Undefined Property Type: {info.PropertyType} {info.Name}");
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
