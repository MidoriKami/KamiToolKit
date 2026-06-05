using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Action = System.Action;

namespace KamiToolKit.Overlay.MapOverlay;

/// <summary>
/// Inheritable node intended for use with <see cref="MapOverlayController"/>.
/// </summary>
public unsafe class MapMarkerNode : ResNode {

    /// <summary>
    /// Gets or sets the action to be called when this marker is clicked on.
    /// </summary>
    public Action? OnClick { get; set; }

    /// <summary>
    /// Gets whether this node is actually being shown.
    /// </summary>
    public bool IsActuallyVisible
        => ResNode is not null && ResNode->IsActuallyVisible;

    /// <summary>
    /// Gets or sets the markers visibility.
    /// </summary>
    public override bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the markers size. Default is 32x32.
    /// </summary>
    public new Vector2 Size { get; set; }

    /// <summary>
    /// Gets or sets the markers scale. Default is 1.0f.
    /// </summary>
    public float MarkerScale { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the markers position on the map.
    /// </summary>
    /// <remarks>
    /// Expects a value between 0.0f and 1024.0f, where 0,0 is the center of the map.
    /// </remarks>
    public new Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the tooltip shown when hovering over this marker.
    /// </summary>
    public new ReadOnlySeString TextTooltip { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mapId this marker is allowed to be in.
    /// Use <see cref="AllowAnyMap"/> to allow on any map.
    /// </summary>
    public uint MapId { get; set; }

    /// <summary>
    /// Gets or sets whether this marker is allowed to be shown when viewing any map.
    /// </summary>
    public bool AllowAnyMap { get; set; }

    /// <summary>
    /// Gets or sets the iconId to be shown with this marker.
    /// </summary>
    /// <remarks>
    /// Setting this will unload any <see cref="Texture"/> or <see cref="TexturePath"/> that is set.
    /// </remarks>
    public uint? IconId {
        get;
        set {
            if (value is null) return;
            field = value;

            if (iconNode is not null) {
                iconNode.IconId = value.Value;
                return;
            }

            imGuiImageNode?.Dispose();
            imGuiImageNode = null;

            iconNode = new IconImageNode {
                IconId = value.Value,
                FitTexture = true,
            };

            iconNode.AttachNode(this);
        }
    } = 0;

    /// <summary>
    /// Gets or sets the DalamudTextureWrap to be shown with this marker.
    /// </summary>
    /// <remarks>
    /// Setting this will unload any <see cref="IconId"/> or <see cref="TexturePath"/> that is set.
    /// </remarks>
    public IDalamudTextureWrap? Texture {
        get;
        set {
            if (value is null) return;
            field = value;

            iconNode?.Dispose();
            iconNode = null;

            imGuiImageNode?.Dispose();
            imGuiImageNode = null;

            imGuiImageNode = new ImGuiImageNode {
                LoadedTexture = value,
                FitTexture = true,
            };
            imGuiImageNode.AttachNode(this);
        }
    } = null;

    /// <summary>
    /// Gets or sets the texture path to load and to be shown with this marker.
    /// </summary>
    /// <remarks>
    /// Setting this will unload any <see cref="IconId"/> or <see cref="Texture"/> that is set.
    /// </remarks>
    public string? TexturePath {
        get;
        set {
            if (value is null) return;
            field = value;

            iconNode?.Dispose();
            iconNode = null;

            imGuiImageNode?.Dispose();
            imGuiImageNode = null;

            imGuiImageNode = new ImGuiImageNode {
                TexturePath = value,
                FitTexture = true,
            };
            imGuiImageNode.AttachNode(this);
        }
    } = null;

    /// <summary>
    /// Updates the nodes size position and scale according to the params of the specific map being shown.
    /// Triggers <see cref="OnUpdate"/>.
    /// </summary>
    public void Update() {
        OnUpdate();

        if (!Services.DataManager.GetExcelSheet<Map>().TryGetRow(MapId, out var mapRow)) {
            IsVisible = false;
            return;
        }

        var mapScale = mapRow.SizeFactor / 100.0f;
        var mapOffset = new Vector2(mapRow.OffsetX, mapRow.OffsetY) * (mapScale - 1);
        var centerOffset = new Vector2(1024.0f, 1024.0f);

        base.Size = Size * MarkerScale;
        base.Origin = base.Size / 2.0f;

        iconNode?.Size = base.Size;
        iconNode?.Origin = base.Size / 2.0f;

        imGuiImageNode?.Size = base.Size;
        imGuiImageNode?.Origin = base.Size / 2.0f;

        base.Position = (Position * mapScale) + mapOffset + centerOffset - (base.Size / 2.0f);
        base.IsVisible = IsVisible && (AllowAnyMap || AgentMap.Instance()->SelectedMapId == MapId);
    }

    protected virtual void OnUpdate() { }

    private IconImageNode? iconNode;
    private ImGuiImageNode? imGuiImageNode;
}
