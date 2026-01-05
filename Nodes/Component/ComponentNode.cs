using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe class ComponentNode(NodeType nodeType) : NodeBase<AtkComponentNode>(nodeType) {
    public abstract CollisionNode CollisionNode { get; }
    public abstract AtkComponentBase* ComponentBase { get; }
    public abstract AtkUldComponentDataBase* DataBase { get; }
}

public abstract unsafe class ComponentNode<T, TU> : ComponentNode where T : unmanaged, ICreatable where TU : unmanaged {
    public override sealed CollisionNode CollisionNode { get; }
    public override sealed AtkComponentBase* ComponentBase => Node->Component;
    public override sealed AtkUldComponentDataBase* DataBase => Node->Component->UldManager.ComponentData;

    protected ComponentNode() : base(NodeType.Component) {
        Node->Component = (AtkComponentBase*) NativeMemoryHelper.Create<T>();
        Node->Component->UldManager.ComponentData = (AtkUldComponentDataBase*)NativeMemoryHelper.UiAlloc<TU>();

        ComponentBase->Initialize();

        CollisionNode = new CollisionNode {
            NodeId = 1,
            LinkedComponent = ComponentBase,
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision | 
                        NodeFlags.RespondToMouse | NodeFlags.Focusable | NodeFlags.EmitsEvents | NodeFlags.Fill,
        };

        CollisionNode.ResNode->ParentNode = ResNode;
        CollisionNode.ParentUldManager = &((AtkComponentBase*)Component)->UldManager;

        ChildNodes.Add(CollisionNode);

        ComponentBase->OwnerNode = Node;
        ComponentBase->ComponentFlags = 1;

        ref var uldManager = ref ComponentBase->UldManager;

        uldManager.Objects = (AtkUldObjectInfo*)NativeMemoryHelper.UiAlloc<AtkUldComponentInfo>();
        ref var objects = ref uldManager.Objects;
        uldManager.ObjectCount = 1;

        objects->NodeList = (AtkResNode**)NativeMemoryHelper.Malloc(8);
        objects->NodeList[0] = CollisionNode;
        objects->NodeCount = 1;
        objects->Id = 1000;

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
                Node->Component->UldManager.ComponentData = null;

                Node->Component->Deinitialize();
                Node->Component->Dtor(1);
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

    public virtual bool IsEnabled {
        get => NodeFlags.HasFlag(NodeFlags.Enabled);
        set => ComponentBase->SetEnabledState(value);
    }

    public override int ChildCount => ComponentBase->UldManager.NodeListCount;

    internal T* Component => (T*)ComponentBase;

    internal TU* Data => (TU*)DataBase;
}
