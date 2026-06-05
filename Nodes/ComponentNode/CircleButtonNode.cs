using System.Numerics;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Enums;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Nodes;

/// <summary>
/// Custom implementation of the games Circle Button Node.
/// </summary>
public class CircleButtonNode : ButtonBase {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleImageNode ImageNode { get; }

    /// <summary>
    /// Gets or sets the icon displayed for this node.
    /// </summary>
    public CircleButtonIcon Icon {
        get;
        set {
            field = value;
            var uldInfo = GetTextureCoordinateForIcon(value);
            ImageNode.TextureCoordinates = uldInfo.TextureCoordinates;
            ImageNode.TextureSize = uldInfo.TextureSize;
        }
    }

    public CircleButtonNode() {
        ImageNode = new SimpleImageNode {
            TexturePath = "ui/uld/CircleButtons.tex",
            TextureSize = new Vector2(24.0f, 24.0f),
            TextureCoordinates = new Vector2(0.0f, 112.0f),
            WrapMode = WrapMode.Stretch,
        };
        ImageNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ImageNode.Size = Size;
    }

    private static UldTextureInfo GetTextureCoordinateForIcon(CircleButtonIcon icon) => icon switch {
        CircleButtonIcon.GearCog => new UldTextureInfo(0.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.Filter => new UldTextureInfo(28.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.Sort => new UldTextureInfo(56.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.QuestionMark => new UldTextureInfo(84.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.Refresh => new UldTextureInfo(112.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.ChatBubble => new UldTextureInfo(140.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.LeftArrow => new UldTextureInfo(168.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.UpArrow => new UldTextureInfo(196.0f, 0.0f, 28.0f, 28.0f),
        CircleButtonIcon.Chest => new UldTextureInfo(224.0f, 0.0f, 28.0f, 28.0f),

        CircleButtonIcon.Document => new UldTextureInfo(0.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.Edit => new UldTextureInfo(28.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.Add => new UldTextureInfo(56.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.RightArrow => new UldTextureInfo(84.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.MusicNote => new UldTextureInfo(112.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.Sprout => new UldTextureInfo(140.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.Dice => new UldTextureInfo(168.0f, 28.0f, 28.0f, 28.0f),
        CircleButtonIcon.ArrowDown => new UldTextureInfo(196.0f, 28.0f, 28.0f, 28.0f),

        CircleButtonIcon.Eye => new UldTextureInfo(0.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.Envelope => new UldTextureInfo(28.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.Volume => new UldTextureInfo(56.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.Mute => new UldTextureInfo(84.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.WavePulse => new UldTextureInfo(112.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.CheckedBox => new UldTextureInfo(140.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.Cross => new UldTextureInfo(168.0f, 56.0f, 28.0f, 28.0f),
        CircleButtonIcon.Globe => new UldTextureInfo(196.0f, 56.0f, 28.0f, 28.0f),

        CircleButtonIcon.ActiveGearCog => new UldTextureInfo(0.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.ActiveFilter => new UldTextureInfo(28.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.Update => new UldTextureInfo(56.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.ActiveRing => new UldTextureInfo(84.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.Exclamation => new UldTextureInfo(112.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.InsetDocument => new UldTextureInfo(140.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.GearCogWithChatBubble => new UldTextureInfo(168.0f, 84.0f, 28.0f, 28.0f),
        CircleButtonIcon.FlatbedCartBoxes => new UldTextureInfo(196.0f, 84.0f, 28.0f, 28.0f),

        CircleButtonIcon.MagnifyingGlass => new UldTextureInfo(0.0f, 112.0f, 24.0f, 24.0f),
        CircleButtonIcon.EditSmall => new UldTextureInfo(24.0f, 112.0f, 24.0f, 24.0f),
        CircleButtonIcon.WeaponDraw => new UldTextureInfo(48.0f, 112.0f, 24.0f, 24.0f),
        CircleButtonIcon.Headgear => new UldTextureInfo(72.0f, 112.0f, 24.0f, 24.0f),
        CircleButtonIcon.Sword => new UldTextureInfo(96.0f, 112.0f, 24.0f, 24.0f),
        CircleButtonIcon.Emotes => new UldTextureInfo(120.0f, 112.0f, 24.0f, 24.0f),
        CircleButtonIcon.PersonStanding => new UldTextureInfo(144.0f, 112.0f, 24.0f, 24.0f),

        CircleButtonIcon.PaintBucket => new UldTextureInfo(0.0f, 136.0f, 24.0f, 24.0f),
        CircleButtonIcon.EyeSmall => new UldTextureInfo(24.0f, 136.0f, 24.0f, 24.0f),
        CircleButtonIcon.Undo => new UldTextureInfo(48.0f, 136.0f, 24.0f, 24.0f),
        CircleButtonIcon.PinPaper => new UldTextureInfo(72.0f, 136.0f, 24.0f, 24.0f),
        CircleButtonIcon.CrossSmall => new UldTextureInfo(96.0f, 136.0f, 24.0f, 24.0f),

        _ => new UldTextureInfo(0.0f, 0.0f, 28.0f, 28.0f),
    };

    private void LoadTimelines()
        => LoadTwoPartTimelines(this, ImageNode);
}
