using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

/// <summary>
/// A counter node for displaying numbers
/// </summary>
public unsafe class CounterNode : NodeBase<AtkCounterNode> {
    protected readonly PartsList PartsList;
    
    public CounterNode() : base(NodeType.Counter) {
        PartsList = new PartsList();
        PartsList.Add( new Part() );
        
        InternalNode->PartsList = PartsList.InternalPartsList;

        NumberWidth = 10;
        CommaWidth = 8;
        SpaceWidth = 6;
        TextAlignment = 5;
        CounterWidth = 32;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();
            
            base.Dispose(disposing);
        }
    }

    [JsonIgnore] public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = value;
    }
    
    public uint NumberWidth {
        get => InternalNode->NumberWidth;
        set => InternalNode->NumberWidth = (byte)value;
    }

    public uint CommaWidth {
        get => InternalNode->CommaWidth;
        set => InternalNode->CommaWidth = (byte)value;
    }

    public uint SpaceWidth {
        get => InternalNode->SpaceWidth;
        set => InternalNode->SpaceWidth = (byte) value;
    }

    public ushort TextAlignment {
        get => InternalNode->TextAlign;
        set => InternalNode->TextAlign = value;
    }

    public float CounterWidth {
        get => InternalNode->CounterWidth;
        set => InternalNode->CounterWidth = value;
    }

    [JsonIgnore] public int Number {
        get => int.Parse(InternalNode->NodeText.ToString());
        set => InternalNode->SetNumber(value);
    }

    [JsonIgnore] public string Text {
        get => InternalNode->NodeText.ToString();
        set => InternalNode->SetText($"{int.Parse(value):n0}");
    }

    public override void DrawConfig() {
        base.DrawConfig();
        
        using var textNode = ImRaii.TreeNode("Counter Node");
        if (!textNode) return;
        
        using var table = ImRaii.Table("counter_property_table", 2);
        if (!table) return;
		
        ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
        ImGui.TableSetupColumn("##configuration", ImGuiTableColumnFlags.WidthStretch, 2.0f);

        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.Text("Counter Width");
        
        ImGui.TableNextColumn();
        var counterWidth = CounterWidth;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.DragFloat("##CounterWidth", ref counterWidth, 0.5f)) {
            Rotation = counterWidth;
        }
        
		ImGui.Spacing();
        
        ImGui.TableNextColumn();
        ImGui.Text("Number Width");
        
        ImGui.TableNextColumn();
        var numberWidth = (int)NumberWidth;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##NumberWidth", ref numberWidth, 0, 0)) {
            NumberWidth = (uint)numberWidth;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Comma Width");
        
        ImGui.TableNextColumn();
        var commaWidth = (int)CommaWidth;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##CommaWidth", ref commaWidth, 0, 0)) {
            CommaWidth = (uint)commaWidth;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Comma Width");
        
        ImGui.TableNextColumn();
        var spaceWidth = (int)SpaceWidth;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##spaceWidth", ref spaceWidth, 0, 0)) {
            SpaceWidth = (uint)spaceWidth;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Text Alignment");
        
        ImGui.TableNextColumn();
        var textAlignment = (int)TextAlignment;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##textAlignment", ref textAlignment, 0, 0)) {
            TextAlignment = (ushort)textAlignment;
        }
    }
}
