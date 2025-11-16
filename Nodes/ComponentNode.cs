using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public abstract unsafe class ComponentNode(NodeType nodeType) : NodeBase<AtkComponentNode>(nodeType) {
    public abstract AtkComponentBase* ComponentBase { get; }
    public abstract AtkUldComponentDataBase* DataBase { get; }
    public abstract AtkComponentNode* InternalComponentNode { get; }
}

public abstract unsafe class ComponentNode<T, TU> : ComponentNode where T : unmanaged, ICreatable where TU : unmanaged {

    public readonly CollisionNode CollisionNode;

    public override AtkComponentBase* ComponentBase => (AtkComponentBase*)Component;
    public override AtkUldComponentDataBase* DataBase => (AtkUldComponentDataBase*)Data;
    public override AtkComponentNode* InternalComponentNode => (AtkComponentNode*)ResNode;

    protected ComponentNode() : base(NodeType.Component) {
        Component = NativeMemoryHelper.Create<T>();
        var componentBase = (AtkComponentBase*)Component;

        Data = NativeMemoryHelper.UiAlloc<TU>();

        componentBase->Initialize();

        CollisionNode = new CollisionNode {
            NodeId = 1,
            LinkedComponent = componentBase,
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.Focusable | NodeFlags.EmitsEvents | NodeFlags.Fill,
        };

        CollisionNode.ResNode->ParentNode = ResNode;
        CollisionNode.ParentUldManager = &((AtkComponentBase*)Component)->UldManager;

        ChildNodes.Add(CollisionNode);

        componentBase->OwnerNode = Node;
        componentBase->ComponentFlags = 1;

        ref var uldManager = ref componentBase->UldManager;

        uldManager.Objects = (AtkUldObjectInfo*)NativeMemoryHelper.UiAlloc<AtkUldComponentInfo>();
        ref var objects = ref uldManager.Objects;
        uldManager.ObjectCount = 1;

        objects->NodeList = (AtkResNode**)NativeMemoryHelper.Malloc(8);
        objects->NodeList[0] = CollisionNode;
        objects->NodeCount = 1;
        objects->Id = 1001;

        uldManager.InitializeResourceRendererManager();
        uldManager.RootNode = CollisionNode;

        uldManager.UpdateDrawNodeList();
        uldManager.ResourceFlags = AtkUldManagerResourceFlag.Initialized | AtkUldManagerResourceFlag.ArraysAllocated;
        uldManager.LoadedState = AtkLoadState.Loaded;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            if (!isNativeDestructor) {
                NativeMemoryHelper.UiFree(Data);
                Data = null;

                ComponentBase->Deinitialize();
                ComponentBase->Dtor(1);
                Node->Component = null;
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }

    public static implicit operator AtkEventListener*(ComponentNode<T, TU> node) => &node.ComponentBase->AtkEventListener;

    protected void SetInternalComponentType(ComponentType type) {
        var componentInfo = (AtkUldComponentInfo*)ComponentBase->UldManager.Objects;

        componentInfo->ComponentType = type;
    }

    protected void InitializeComponentEvents() {
        ComponentBase->InitializeFromComponentData(DataBase);

        ComponentBase->Setup();
        ComponentBase->SetEnabledState(true);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        CollisionNode.Size = Size;
        ComponentBase->UldManager.RootNodeHeight = (ushort)Height;
        ComponentBase->UldManager.RootNodeWidth = (ushort)Width;
    }

    public override int ChildCount => ComponentBase->UldManager.NodeListCount;

    internal T* Component {
        get => (T*)Node->Component;
        set => Node->Component = (AtkComponentBase*)value;
    }

    internal TU* Data {
        get => (TU*)Node->Component->UldManager.ComponentData;
        set => Node->Component->UldManager.ComponentData = (AtkUldComponentDataBase*)value;
    }
}
