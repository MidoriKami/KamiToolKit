using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Vector2 = System.Numerics.Vector2;

namespace KamiToolKit.Premade.Nodes;

public unsafe class MapMarkerNode : SimpleOverlayNode {
    private IconImageNode? iconNode;
    private ImGuiImageNode? imGuiImageNode;
    
    public override bool IsVisible { get; set; } = true;

    public override Vector2 Position {
        get => base.Position;
        set {
            if (DalamudInterface.Instance.DataManager.GetExcelSheet<Map>().TryGetRow(MapId, out var mapRow)) {
                base.Position = value * (mapRow.SizeFactor / 100.0f) + (new Vector2(mapRow.OffsetX, mapRow.OffsetY) + new Vector2(1024.0f, 1024.0f)) - Size * Scale / 2.0f;
            }
            else {
                base.Position = value;
            }
        }
    }

    public new ReadOnlySeString TextTooltip { get; set; } = string.Empty;

    public uint MapId { get; set; }

    public uint? IconId {
        get;
        set {
            if (value is null) return;
            field = value;
            
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

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        iconNode?.Size = Size;
        imGuiImageNode?.Size = Size;
    }

    public void Update() {
        OnUpdate();

        base.IsVisible = IsVisible && AgentMap.Instance()->SelectedMapId == MapId;
    }

    protected virtual void OnUpdate() { }
}
