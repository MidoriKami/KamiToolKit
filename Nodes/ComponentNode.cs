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

    protected ComponentNode() : base((NodeType)1001) {
        Component = NativeMemoryHelper.Create<T>();
        var componentBase = (AtkComponentBase*)Component;

        Data = NativeMemoryHelper.UiAlloc<TU>();

        componentBase->Initialize();

        CollisionNode = new CollisionNode {
            NodeId = 1, LinkedComponent = componentBase, NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.Focusable | NodeFlags.EmitsEvents,
        };

        CollisionNode.InternalResNode->ParentNode = InternalResNode;

        componentBase->OwnerNode = Node;
        componentBase->AtkResNode = CollisionNode.InternalResNode;
        componentBase->ComponentFlags = 1;

        ref var uldManager = ref componentBase->UldManager;

        uldManager.Objects = (AtkUldObjectInfo*)NativeMemoryHelper.UiAlloc<AtkUldComponentInfo>();
        ref var objects = ref uldManager.Objects;
        uldManager.ObjectCount = 1;

        objects->NodeList = (AtkResNode**)NativeMemoryHelper.Malloc(8);
        objects->NodeList[0] = CollisionNode.InternalResNode;
        objects->NodeCount = 1;
        objects->Id = 1001;

        uldManager.InitializeResourceRendererManager();
        uldManager.RootNode = CollisionNode.InternalResNode;

        uldManager.UpdateDrawNodeList();
        uldManager.ResourceFlags = AtkUldManagerResourceFlag.Initialized | AtkUldManagerResourceFlag.ArraysAllocated;
        uldManager.LoadedState = AtkLoadState.Loaded;
    }

    public override AtkComponentBase* ComponentBase => (AtkComponentBase*)Component;
    public override AtkUldComponentDataBase* DataBase => (AtkUldComponentDataBase*)Data;
    public override AtkComponentNode* InternalComponentNode => (AtkComponentNode*)InternalResNode;

    internal T* Component {
        get => (T*)Node->Component;
        set => Node->Component = (AtkComponentBase*)value;
    }

    internal TU* Data {
        get => (TU*)Node->Component->UldManager.ComponentData;
        set => Node->Component->UldManager.ComponentData = (AtkUldComponentDataBase*)value;
    }

    public override int ChildCount => ComponentBase->UldManager.NodeListCount;

    protected override void Dispose(bool disposing) {
        if (disposing) {
            NativeMemoryHelper.UiFree(Data);
            Data = null;

            ComponentBase->Dtor(1);

            base.Dispose(disposing);
        }
    }

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
}
