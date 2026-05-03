using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Dalamud;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Action = System.Action;

namespace KamiToolKit.Overlay.MapOverlay;

public unsafe class MapMarkerNode : SimpleOverlayNode {
    private IconImageNode? iconNode;
    private ImGuiImageNode? imGuiImageNode;

    public override bool IsVisible { get; set; } = true;

    public new Vector2 Size { get; set; }

    public float MarkerScale { get; set; } = 1.0f;

    public new Vector2 Position { get; set; }

    public new ReadOnlySeString TextTooltip { get; set; } = string.Empty;

    public uint MapId { get; set; }

    public bool AllowAnyMap { get; set; }

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

    public Action? OnClick { get; set; }

    public bool IsActuallyVisible() => ResNode is not null && ResNode->IsActuallyVisible;
}
