using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

/// <summary>
///     A counter node for displaying numbers
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public unsafe class CounterNode : NodeBase<AtkCounterNode> {

    public readonly PartsList PartsList;

    public CounterNode() : base(NodeType.Counter) {
        PartsList = new PartsList();
        PartsList.Add(new Part());

        Node->PartsList = PartsList.InternalPartsList;

        NumberWidth = 10;
        CommaWidth = 8;
        SpaceWidth = 6;
        TextAlignment = 5;
        CounterWidth = 32;
    }

    public string TexturePath {
        get => PartsList[0]->GetLoadedPath();
        set => PartsList[0]->LoadTexture(value);
    }

    public Vector2 TextureCoordinates {
        get => new(PartsList[0]->U, PartsList[0]->V);
        set {
            PartsList[0]->U = (ushort) value.X;
            PartsList[0]->V = (ushort) value.X;
        }
    }

    public Vector2 TextureSize {
        get => new(PartsList[0]->Width, PartsList[0]->Height);
        set {
            PartsList[0]->Width = (ushort) value.X;
            PartsList[0]->Height = (ushort) value.X;
        }
    }

    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

    [JsonProperty] public uint NumberWidth {
        get => Node->NumberWidth;
        set => Node->NumberWidth = (byte)value;
    }

    [JsonProperty] public uint CommaWidth {
        get => Node->CommaWidth;
        set => Node->CommaWidth = (byte)value;
    }

    [JsonProperty] public uint SpaceWidth {
        get => Node->SpaceWidth;
        set => Node->SpaceWidth = (byte)value;
    }

    [JsonProperty] public ushort TextAlignment {
        get => Node->TextAlign;
        set => Node->TextAlign = value;
    }

    [JsonProperty] public float CounterWidth {
        get => Node->CounterWidth;
        set => Node->CounterWidth = value;
    }

    public int Number {
        get => int.Parse(Node->NodeText.ToString());
        set => Node->SetNumber(value);
    }

    public string String {
        get => Node->NodeText.ToString();
        set => Node->SetText($"{int.Parse(value):n0}");
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();

            base.Dispose(disposing);
        }
    }
}
