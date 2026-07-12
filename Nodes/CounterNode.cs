using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using Lumina.Text.Payloads;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games CounterNode.
/// </summary>
public unsafe class CounterNode : NodeBase<AtkCounterNode> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public PartsList PartsList { get; }

    /// <summary>
    /// Gets or sets the width of each digit.
    /// </summary>
    public uint NumberWidth {
        get => Node->NumberWidth;
        set => Node->NumberWidth = (byte)value;
    }

    /// <summary>
    /// Gets or sets the width of the numeric separator.
    /// </summary>
    public uint CommaWidth {
        get => Node->CommaWidth;
        set => Node->CommaWidth = (byte)value;
    }

    /// <summary>
    /// Gets or sets the width of spaces.
    /// </summary>
    public uint SpaceWidth {
        get => Node->SpaceWidth;
        set => Node->SpaceWidth = (byte)value;
    }

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    public AlignmentType TextAlignment {
        get => (AlignmentType)Node->TextAlign;
        set => Node->TextAlign = (ushort)value;
    }

    /// <summary>
    /// The width of the counter itself.
    /// </summary>
    public float CounterWidth {
        get => Node->CounterWidth;
        set => Node->CounterWidth = value;
    }

    /// <summary>
    /// Gets or sets the number displayed.
    /// </summary>
    /// <remarks>
    /// The value is actually saved as a string, so this incurs parsing costs.
    /// </remarks>
    public int Number {
        get => int.Parse(Node->NodeText.ToString());
        set => Node->SetText(ParseNumber(value));
    }

    /// <summary>
    /// Gets or sets the string displayed.
    /// </summary>
    public ReadOnlySeString String {
        get => Node->NodeText.AsSpan();
        set => Node->SetText(ParseString(value));
    }

    /// <summary>
    /// Gets or sets the font used for the counter.
    /// </summary>
    /// <remarks>
    /// Defaults to MoneyFont.
    /// </remarks>
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

    /// <summary>
    /// Constructs a new <see cref="CounterNode"/>
    /// </summary>
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

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        if (!isNativeDestructor) {
            PartsList.Dispose();
            Node->PartsList = null;
        }

        base.Dispose(isNativeDestructor);
    }

    /// <summary>
    /// Gets or sets the texture path for the font used by this counter node.
    /// </summary>
    protected string TexturePath {
        get => PartsList[0]->LoadedPath;
        set => PartsList[0]->LoadTexture(value);
    }

    /// <summary>
    /// Gets or sets the texture coordinates used for the font used by this counter node.
    /// </summary>
    protected Vector2 TextureCoordinates {
        get => new(PartsList[0]->U, PartsList[0]->V);
        set {
            PartsList[0]->U = (ushort)value.X;
            PartsList[0]->V = (ushort)value.X;
        }
    }

    /// <summary>
    /// Gets or sets the texture size of the font texture used by this counter node.
    /// </summary>
    protected Vector2 TextureSize {
        get => new(PartsList[0]->Width, PartsList[0]->Height);
        set {
            PartsList[0]->Width = (ushort)value.X;
            PartsList[0]->Height = (ushort)value.X;
        }
    }

    private static ReadOnlySeString ParseString(ReadOnlySeString value) {
        using var builder = new RentedSeStringBuilder();
        return builder.Builder.Append(value).GetViewAsSpan();
    }

    private static ReadOnlySeString ParseNumber(int value) {
        using var rentedBuilder = new RentedSeStringBuilder();

        // <kilo(lnum1,\,)>
        var evaluatedString = Services.SeStringEvaluator.EvaluateFromAddon(18, [value]);

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
