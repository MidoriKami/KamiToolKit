using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Layout node representing a collapsing header that will collapse/hide its contained nodes.
/// </summary>
public class CollapsingHeaderNode : LayoutListNode {

    /// <summary>
    /// Gets or sets the nodes collapsed state.
    /// </summary>
    public bool IsCollapsed { get; set; }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    public ReadOnlySeString String {
        get => LabelTextNode.String;
        set => LabelTextNode.String = value;
    }

    /// <summary>
    /// Gets the node used to display the body texture of this node.
    /// </summary>
    public SimpleNineGridNode ButtonTextureNode { get; }

    /// <summary>
    /// Gets the text node used to display this buttons label.
    /// </summary>
    public TextNode LabelTextNode { get; }

    /// <summary>
    /// Gets the image node used to display the collapsed/uncollapsed arrow node.
    /// </summary>
    public ImageNode ToggleArrowImageNode { get; }

    /// <summary>
    /// Constructs a new <see cref="CollapsingHeaderNode"/>
    /// </summary>
    public CollapsingHeaderNode() {
        ButtonTextureNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/img05/ListItemB.tex",
            TextureSize = new Vector2(48.0f, 28.0f),
            TextureCoordinates = new Vector2(0.0f, 24.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.Focusable,
            TopOffset = 10,
            BottomOffset = 12,
            LeftOffset = 12,
            RightOffset = 12,
        };
        ButtonTextureNode.AttachNode(this);

        LabelTextNode = new TextNode {
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled,
        };
        LabelTextNode.AttachNode(this);

        ToggleArrowImageNode = new ImageNode {
            FitTexture = true,
            PartId = 1,
        };
        ToggleArrowImageNode.AddPart([
            new Part { Id = 1, TexturePath = "ui/uld/img05/ListItemB.tex", TextureCoordinates = new Vector2(24.0f, 0.0f), Size = new Vector2(24.0f, 24.0f) },
            new Part { Id = 2, TexturePath = "ui/uld/img05/ListItemB.tex", TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(24.0f, 24.0f) },
        ]);
        ToggleArrowImageNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {

    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {

    }
}
