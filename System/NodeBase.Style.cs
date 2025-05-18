using System;
using System.IO;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ImGuiNET;
using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.System;

public partial class NodeBase {
	public void Save(string filePath) {
		try {
			var fileText = JsonConvert.SerializeObject(this, Formatting.Indented);
			FilesystemUtil.WriteAllTextSafe(filePath, fileText);
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}

	public void Load(string filePath) {
		try {
			var fileData = File.ReadAllText(filePath);
			if (!fileData.IsNullOrEmpty()) {
				JsonConvert.PopulateObject(fileData, this);
			}
		}
		catch (FileNotFoundException) {
			Save(filePath);
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}

	public virtual void DrawConfig() {
		using var treeNode = ImRaii.TreeNode("Base Data");
		if (!treeNode) return;

		using var table = ImRaii.Table("basic_property_table", 2);
		if (!table) return;
		
		ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
		ImGui.TableSetupColumn("##configuration", ImGuiTableColumnFlags.WidthStretch, 2.0f);

		ImGui.TableNextRow();
		// Four Value Properties

		ImGui.TableNextColumn();
		ImGui.Text("Color");

		ImGui.TableNextColumn();
		var color = Color;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.ColorEdit4("##Color", ref color, ImGuiColorEditFlags.AlphaPreviewHalf)) {
			Color = color;
		}

		ImGui.Spacing();
		// Three Value Properties
		
		ImGui.TableNextColumn();
		ImGui.Text("Add Color");

		ImGui.TableNextColumn();
		var addColor = AddColor;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.ColorEdit3("##AddColor", ref addColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
			AddColor = addColor;
		}
		
		ImGui.TableNextColumn();
		ImGui.Text("Multiply Color");

		ImGui.TableNextColumn();
		var multiplyColor = MultiplyColor;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.ColorEdit3("##MultiplyColor", ref multiplyColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
			MultiplyColor = multiplyColor;
		}
		
		ImGui.Spacing();
		// Two Value Properties
		
		ImGui.TableNextColumn();
		ImGui.Text("Position");
		
		ImGui.TableNextColumn();
		var position = Position;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.DragFloat2("##Position", ref position)) {
			Position = position;
		}
		
		ImGui.TableNextColumn();
		ImGui.Text("Size");
		
		ImGui.TableNextColumn();
		var size = Size;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.DragFloat2("##Size", ref size)) {
			Size = size;
		}

		ImGui.TableNextColumn();
		ImGui.Text("Scale");
		
		ImGui.TableNextColumn();
		var scale = Scale;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.DragFloat2("##Scale", ref scale, 0.005f, 0.10f, 6.00f)) {
			Scale = scale;
		}
		
		ImGui.TableNextColumn();
		ImGui.Text("Origin");
		
		ImGui.TableNextColumn();
		var origin = Origin;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.DragFloat2("##Origin", ref origin)) {
			Origin = origin;
		}
		
		ImGui.Spacing();
		// One Value Properties
		ImGui.TableNextColumn();
		ImGui.Text("Tooltip");
		
		ImGui.TableNextColumn();
		using (ImRaii.Disabled(!EventFlagsSet)) {
			var tooltip = TooltipString;
			ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
			if (ImGui.InputTextMultiline("##tooltip", ref tooltip, 2000, ImGui.GetContentRegionAvail() with { Y = ImGuiHelpers.GetButtonSize("A").Y * 2.0f } ,ImGuiInputTextFlags.AutoSelectAll)) {
				TooltipString = tooltip;
			}
		}
		
		ImGui.Spacing();
		
		ImGui.TableNextColumn();
		ImGui.Text("Rotation");
		
		ImGui.TableNextColumn();
		var rotation = Rotation * ( 180.0f / MathF.PI );
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.DragFloat("##Rotation", ref rotation, 0.5f, 0.0f, 360.0f, "%.0f")) {
			Rotation = rotation * MathF.PI / 180.0f;
		}

		ImGui.Spacing();
		// Checkboxes
		
		ImGui.TableNextColumn();
		ImGui.Text("Visible");
		
		ImGui.TableNextColumn();
		var visible = IsVisible;
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.Checkbox("##visible", ref visible)) {
			IsVisible = visible;
		}
	}
}