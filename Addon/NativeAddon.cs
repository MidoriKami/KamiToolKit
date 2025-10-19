using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

    private GCHandle? disposeHandle;

    internal AtkUnitBase* InternalAddon;

    public ResNode RootNode = null!;

    public WindowNode WindowNode {
        get;
        set {
            if (value is null) throw new Exception("窗口节点不能为空。");

            if (InternalAddon->WindowNode is not null) {
                InternalAddon->WindowNode = null;
                field.DetachNode();
            }

            field = value;
            value.AttachNode(RootNode, NodePosition.AsFirstChild);
            InternalAddon->WindowNode = value.InternalComponentNode;
        }
    } = null!;

    private void AllocateAddon() {
        if (InternalAddon is not null) {
            Log.Warning("尝试为已分配的 Addon 再次分配内存，操作已取消。");
            return;
        }

        var currentAddonCount = RaptureAtkUnitManager.Instance()->AllLoadedUnitsList.Count;
        if (currentAddonCount >= 200) {
            Log.Warning($"警告：当前 Addon 数量接近上限 ({currentAddonCount}/250)，请确认自定义 Addon 未被当作覆盖层重复使用。");
        }

        if (currentAddonCount >= 225) {
            Log.Error($"错误：当前 Addon 数量过多，已停止分配 ({currentAddonCount}/250)。");
            return;
        }

        Log.Verbose($"[{InternalName}] 开始分配原生 Addon");

        InitializeExtras();

        InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();

        RegisterVirtualTable();

        InternalAddon->Flags1A2 |= 0b0100_0000; // don't save/load AddonConfig

        RootNode = new ResNode {
            NodeId = 1, 
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | 
                        NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
        };

        WindowNode = new WindowNode();

        InternalAddon->NameString = GetInternalNameSafe();

        InternalAddon->OpenSoundEffectId = (short)OpenWindowSoundEffectId;

        Log.Verbose($"[{InternalName}] 完成分配");
    }

    private void InitializeAddon() {
        Log.Verbose($"[{InternalName}] 正在初始化 Addon");

        var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
        widgetInfo->Id = 1;
        widgetInfo->NodeCount = 0;
        widgetInfo->NodeList = null;
        widgetInfo->WidgetAlignment = new AtkWidgetAlignment {
            AlignmentType = AlignmentType.Center, 
            X = 50.0f, 
            Y = 50.0f,
        };

        InternalAddon->UldManager.Objects = (AtkUldObjectInfo*)widgetInfo;
        InternalAddon->UldManager.ObjectCount = 1;
        InternalAddon->UldManager.ResourceFlags |= AtkUldManagerResourceFlag.ArraysAllocated;

        InternalAddon->RootNode = RootNode.InternalResNode;
        InternalAddon->UldManager.AddNodeToObjectList(RootNode.InternalResNode);

        LoadTimeline();

        InternalAddon->UldManager.UpdateDrawNodeList();
        InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;

        // UldManager finished loading the uld
        InternalAddon->Flags198 |= 2 << 0x1C;

        // LoadUldByName called
        InternalAddon->Flags1A2 |= 4;

        InternalAddon->UpdateCollisionNodeList(false);

        // Now that we have constructed this instance, track it for auto-dispose
        CreatedAddons.Add(this);

        Log.Verbose($"[{InternalName}] 初始化完成");
    }

    /// <summary>
    ///     Initializes and Opens this instance of Addon
    /// </summary>
    public void Open()
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"[{InternalName}] 收到打开请求");

            if (InternalAddon is null) {
                AllocateAddon();

                if (InternalAddon is not null) {
                    AtkStage.Instance()->RaptureAtkUnitManager->InitializeAddon(InternalAddon, InternalName);
                    InternalAddon->Open((uint)DepthLayer);
                    disposeHandle = GCHandle.Alloc(this);
                }
            }
            else {
                Log.Verbose($"[{InternalName}] 已处于打开状态，本次请求忽略。");
            }
        });

    public void Close()
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            Log.Verbose($"[{InternalName}] 正在关闭");

            if (InternalAddon is null) return;
            InternalAddon->Close(false);
        });

    public void Toggle() {
        if (IsOpen) {
            Close();
        }
        else {
            Open();
        }
    }

    private void LoadTimeline() {
        RootNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 89)
            .AddLabel(1, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(20, 103, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(30, 104, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(40, 105, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(50, 106, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(60, 107, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(70, 108, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(80, 109, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build());
    }

    private string GetInternalNameSafe() {
        var noSpaces = InternalName.Replace(" ", string.Empty);

        if (noSpaces.Length > 31) {
            noSpaces = noSpaces[..31];
        }

        noSpaces += char.MinValue;
        return noSpaces;
    }
}
