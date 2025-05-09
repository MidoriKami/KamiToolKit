using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class ComponentNode<T, TU> : NodeBase<AtkComponentNode> where T : unmanaged, ICreatable where TU : unmanaged {
	
	protected readonly CollisionNode CollisionNode;
	
	protected ComponentNode() : base((NodeType) 1000) {
		Component = NativeMemoryHelper.Create<T>();

		CollisionNode = new CollisionNode {
			IsVisible = true,
		};

		CollisionNode.InternalResNode->ParentNode = InternalResNode;
		
		ComponentBase->OwnerNode = InternalNode;
		
		ref var uldManager = ref ComponentBase->UldManager;

		uldManager.Objects = NativeMemoryHelper.UiAlloc<AtkUldObjectInfo>();
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

	internal AtkComponentBase* ComponentBase {
		get => InternalNode->Component;
		set => InternalNode->Component = value;
	}

	internal T* Component {
		get => (T*)InternalNode->Component;
		set => InternalNode->Component = (AtkComponentBase*) value;
	}

	internal AtkUldComponentDataBase* DataBase {
		get => InternalNode->Component->UldManager.ComponentData;
		set => InternalNode->Component->UldManager.ComponentData = value;
	}

	internal TU* Data {
		get => (TU*) InternalNode->Component->UldManager.ComponentData;
		set => InternalNode->Component->UldManager.ComponentData = (AtkUldComponentDataBase*) value;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			NativeMemoryHelper.Free(ComponentBase->UldManager.Objects->NodeList, 8);
			NativeMemoryHelper.UiFree(ComponentBase->UldManager.Objects);
			NativeMemoryHelper.UiFree(Component);
		
			CollisionNode.Dispose();
		
			base.Dispose(disposing);
		}
	}

	protected void SetInternalComponentType(ComponentType type) {
		var componentInfo = (AtkUldComponentInfo*) ComponentBase->UldManager.Objects;
		
		componentInfo->ComponentType = type;
	}
}