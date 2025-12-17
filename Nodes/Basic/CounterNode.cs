using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Lumina.Text.Payloads;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
///     A counter node for displaying numbers
/// </summary>
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
                Node->PartsList = null;
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

    public uint NumberWidth {
        get => Node->NumberWidth;
        set => Node->NumberWidth = (byte)value;
    }

    public uint CommaWidth {
        get => Node->CommaWidth;
        set => Node->CommaWidth = (byte)value;
    }

    public uint SpaceWidth {
        get => Node->SpaceWidth;
        set => Node->SpaceWidth = (byte)value;
    }

    public AlignmentType TextAlignment {
        get => (AlignmentType) Node->TextAlign;
        set => Node->TextAlign = (ushort) value;
    }

    public float CounterWidth {
        get => Node->CounterWidth;
        set => Node->CounterWidth = value;
    }

    public int Number {
        get => int.Parse(Node->NodeText.ToString());
        set => Node->SetText(ParseNumber(value));
    }

    public ReadOnlySeString String {
        get => Node->NodeText.AsSpan();
        set => Node->SetText(ParseString(value));
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

    private static ReadOnlySeString ParseString(ReadOnlySeString value) {
        using var builder = new RentedSeStringBuilder();
        return builder.Builder.Append(value).GetViewAsSpan();
    }

    private static ReadOnlySeString ParseNumber(int value) {
        using var rentedBuilder = new RentedSeStringBuilder();

        // <kilo(lnum1,\,)>
        var evaluatedString = DalamudInterface.Instance.SeStringEvaluator.EvaluateFromAddon(18, [ value ]);

        foreach (var payload in evaluatedString) {
            switch (payload.Type) {

                // Fix for French thousands separators.
                // The game calls FormatAddonText2 that does this.
                case ReadOnlySePayloadType.Macro when payload.MacroCode is MacroCode.NonBreakingSpace:
                    rentedBuilder.Builder.Append(' ');
                    break;

                default:
                    rentedBuilder.Builder.Append(payload);
                    break;
            }
        }

        return rentedBuilder.Builder.GetViewAsSpan();
    }
}
