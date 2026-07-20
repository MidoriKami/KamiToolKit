using System;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.BaseTypes.ComponentNode;

/// <summary>
/// Generic Implementation of the games ComponentNode as a base class for use in KTK.
/// </summary>
/// <typeparam name="T">The component type</typeparam>
/// <typeparam name="TU">The component uld data type</typeparam>
public abstract unsafe class ComponentNode<T, TU> : ComponentNode where T : unmanaged, ICreatable<T> where TU : unmanaged {

    /// <inheritdoc/>>
    public sealed override CollisionNode CollisionNode { get; }

    /// <inheritdoc/>>
    public sealed override AtkComponentBase* ComponentBase
        => Node->Component;

    /// <summary>
    /// Gets the typed component.
    /// </summary>
    public T* Component
        => (T*)ComponentBase;

    /// <inheritdoc/>>
    public sealed override AtkUldComponentDataBase* DataBase
        => Node->Component->UldManager.ComponentData;

    /// <summary>
    /// Gets the typed uld data.
    /// </summary>
    public TU* Data => (TU*)DataBase;

    /// <summary>
    /// Implicit conversion to AtkEventListener for seamless game interop.
    /// </summary>
    public static implicit operator AtkEventListener*(ComponentNode<T, TU> node)
        => &node.ComponentBase->AtkEventListener;

    /// <summary>
    /// Implicit conversion to the components type for seamless game interop.
    /// </summary>
    public static implicit operator T*(ComponentNode<T, TU> node)
        => node.Component;

    /// <summary>
    /// Implicit conversion to the components uld data type for seamless game interop.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static implicit operator TU*(ComponentNode<T, TU> node)
        => node.Data;

    /// <summary>
    /// Gets or sets whether the component is in an enabled state. Default is enabled.
    /// </summary>
    public virtual bool IsEnabled {
        get => NodeFlags.HasFlag(NodeFlags.Enabled);
        set {
            if (IsEnabled != value) {
                ComponentBase->SetEnabledState(value);
            }
        }
    }

    /// <summary>
    /// Sets this node as focused using the <see cref="ComponentNode.FocusNode"/> property.
    /// </summary>
    public void SetFocus() {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByNode(this);
        if (addon is null) return;

        AtkStage.Instance()->AtkInputManager->SetFocus(FocusNode, addon, 0);
    }

    /// <summary>
    /// Sets the AtkUldComponent's internal type.
    /// </summary>
    /// <param name="type"></param>
    protected void SetInternalComponentType(ComponentType type) {
        var componentInfo = (AtkUldComponentInfo*)ComponentBase->UldManager.Objects;

        componentInfo->ComponentType = type;
    }

    /// <summary>
    /// Performs post-construction initialization of components based on their actual created type.
    /// </summary>
    /// <remarks>
    /// The game does a bunch of its own magic here to wire things up for us.
    /// </remarks>
    protected void InitializeComponentEvents() {
        ComponentBase->InitializeFromComponentData(DataBase);
        ComponentBase->Setup();
        ComponentBase->SetEnabledState(true);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        CollisionNode.Size = Size;
        ComponentBase->UldManager.RootNodeHeight = (ushort)Height;
        ComponentBase->UldManager.RootNodeWidth = (ushort)Width;
    }

    /// <summary>
    /// Constructs a new instance of <see cref="ComponentNode"/>
    /// </summary>
    protected ComponentNode() : base(NodeType.Component) {
        Node->Component = (AtkComponentBase*)NativeMemoryHelper.Create<T>();
        Node->Component->UldManager.ComponentData = (AtkUldComponentDataBase*)NativeMemoryHelper.UiAlloc<TU>();

        RegisterVirtualTable();

        ComponentBase->Initialize();

        CollisionNode = new CollisionNode {
            NodeId = 1,
            LinkedComponent = ComponentBase,
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision |
                        NodeFlags.RespondToMouse | NodeFlags.Focusable | NodeFlags.EmitsEvents | NodeFlags.Fill,
        };

        FocusNode = CollisionNode;

        CollisionNode.ResNode->ParentNode = ResNode;
        CollisionNode.ParentUldManager = &((AtkComponentBase*)Component)->UldManager;

        ChildNodes.Add(CollisionNode);

        ComponentBase->OwnerNode = Node;
        ComponentBase->ComponentFlags = 1;

        ref var uldManager = ref ComponentBase->UldManager;

        uldManager.Objects = (AtkUldObjectInfo*)NativeMemoryHelper.UiAlloc<AtkUldComponentInfo>();
        ref var objects = ref uldManager.Objects;
        uldManager.ObjectCount = 1;

        SetInternalComponentType(ComponentType.Base);

        objects->NodeList = (AtkResNode**)NativeMemoryHelper.Malloc(8);
        objects->NodeList[0] = CollisionNode;
        objects->NodeCount = 1;
        objects->Id = 1000;

        uldManager.InitializeResourceRendererManager();
        uldManager.RootNode = CollisionNode;

        uldManager.UpdateDrawNodeList();
        uldManager.ResourceFlags = AtkUldManagerResourceFlag.Initialized | AtkUldManagerResourceFlag.ArraysAllocated;
        uldManager.LoadedState = AtkLoadState.Loaded;

        AddNodeFlags(NodeFlags.EmitsEvents);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        try {
            if (!isNativeDestructor && Node is not null && Node->Component is not null) {
                Node->Component->Deinitialize();
                Node->Component->Dtor(1);
                Node->Component = null;
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        } finally {
            base.Dispose(isNativeDestructor);
        }
    }
}
