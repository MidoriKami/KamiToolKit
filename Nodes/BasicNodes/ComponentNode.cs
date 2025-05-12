using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract unsafe class ComponentNode(NodeType nodeType) : NodeBase<AtkComponentNode>(nodeType) {
	internal abstract AtkComponentBase* ComponentBase { get; }
	internal abstract AtkUldComponentDataBase* DataBase { get; }
}

public unsafe class ComponentNode<T, TU> : ComponentNode where T : unmanaged, ICreatable where TU : unmanaged {

	protected readonly CollisionNode CollisionNode;
	internal override AtkComponentBase* ComponentBase => (AtkComponentBase*) Component;
	internal override AtkUldComponentDataBase* DataBase => (AtkUldComponentDataBase*) Data;
	
	protected ComponentNode() : base((NodeType) 1000) {
		Component = NativeMemoryHelper.Create<T>();
		var componentBase = (AtkComponentBase*) Component;

		componentBase->InitializeAtkUldManager();
					
		CollisionNode = new CollisionNode {
			IsVisible = true,
			NodeID = 1,
		};

		CollisionNode.InternalResNode->ParentNode = InternalResNode;
		
		componentBase->OwnerNode = InternalNode;
		componentBase->AtkResNode = CollisionNode.InternalResNode;
		
		ref var uldManager = ref componentBase->UldManager;

		uldManager.Objects = (AtkUldObjectInfo*) NativeMemoryHelper.UiAlloc<AtkUldComponentInfo>();
		ref var objects = ref uldManager.Objects;

		objects->NodeList = (AtkResNode**) NativeMemoryHelper.Malloc(8);
		objects->NodeList[0] = CollisionNode.InternalResNode;
		objects->NodeCount = 1;
		objects->Id = 1;
		
		uldManager.InitializeResourceRendererManager();
		uldManager.RootNode = CollisionNode.InternalResNode;
		
		uldManager.UpdateDrawNodeList();
		uldManager.Flags1 = 1;
		uldManager.LoadedState = AtkLoadState.Loaded;
	}

	internal T* Component {
		get => (T*)InternalNode->Component;
		set => InternalNode->Component = (AtkComponentBase*) value;
	}

	internal TU* Data {
		get => (TU*) InternalNode->Component->UldManager.ComponentData;
		set => InternalNode->Component->UldManager.ComponentData = (AtkUldComponentDataBase*) value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			CollisionNode.DetachNode();
			CollisionNode.Dispose();
			
			ComponentBase->DeinitializeAtkUldManager();
			ComponentBase->Dtor(1);

			base.Dispose(disposing);
		}
	}

	protected void SetInternalComponentType(ComponentType type) {
		var componentInfo = (AtkUldComponentInfo*) ComponentBase->UldManager.Objects;
		
		componentInfo->ComponentType = type;
	}
}