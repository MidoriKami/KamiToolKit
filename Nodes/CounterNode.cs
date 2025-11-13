using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.System;
using Lumina.Text.ReadOnly;
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
        TextAlignment = AlignmentType.Right;
        CounterWidth = 32;
        Font = CounterFont.MoneyFont;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            if (!isNativeDestructor) {
                PartsList.Dispose();
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }

    protected string TexturePath {
        get => PartsList[0]->GetLoadedPath();
        set => PartsList[0]->LoadTexture(value);
    }

    protected Vector2 TextureCoordinates {
        get => new(PartsList[0]->U, PartsList[0]->V);
        set {
            PartsList[0]->U = (ushort) value.X;
            PartsList[0]->V = (ushort) value.X;
        }
    }

    protected Vector2 TextureSize {
        get => new(PartsList[0]->Width, PartsList[0]->Height);
        set {
            PartsList[0]->Width = (ushort) value.X;
            PartsList[0]->Height = (ushort) value.X;
        }
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

    [JsonProperty] public AlignmentType TextAlignment {
        get => (AlignmentType) Node->TextAlign;
        set => Node->TextAlign = (ushort) value;
    }

    [JsonProperty] public float CounterWidth {
        get => Node->CounterWidth;
        set => Node->CounterWidth = value;
    }

    public int Number {
        get => int.Parse(Node->NodeText.ToString());
        set => String = DalamudInterface.Instance.SeStringEvaluator.EvaluateFromAddon(18, [ value ]);
    }

    public ReadOnlySeString String {
        get => Node->NodeText.ToString();
        set => Node->SetText(value);
    }

    public CounterFont Font {
        get;
        set {
            field = value;

            var fontPath = string.Empty;
            var partSize = Vector2.Zero;

            switch (value) {
                case CounterFont.MoneyFont:
                    fontPath = "ui/uld/Money_Number.tex";
                    partSize = new Vector2(22.0f, 22.0f);
                    break;

                case CounterFont.ChocoboRace:
                    fontPath = "ui/uld/RaceChocoboNum.tex";
                    partSize = new Vector2(30.0f, 60.0f);
                    break;
            }

            if (fontPath != string.Empty && partSize != Vector2.Zero) {
                PartsList[0]->Width = (ushort)partSize.X;
                PartsList[0]->Height = (ushort)partSize.Y;
                PartsList[0]->LoadTexture(fontPath);
            }
        }
    }
}
