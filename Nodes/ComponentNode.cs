﻿using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public abstract unsafe class ComponentNode(NodeType nodeType) : NodeBase<AtkComponentNode>(nodeType) {
	internal abstract AtkComponentBase* ComponentBase { get; }
	internal abstract AtkUldComponentDataBase* DataBase { get; }
	internal abstract AtkComponentNode* InternalComponentNode { get; }
}

public abstract unsafe class ComponentNode<T, TU> : ComponentNode where T : unmanaged, ICreatable where TU : unmanaged {

	protected readonly CollisionNode CollisionNode;
	internal override AtkComponentBase* ComponentBase => (AtkComponentBase*) Component;
	internal override AtkUldComponentDataBase* DataBase => (AtkUldComponentDataBase*) Data;
	internal override AtkComponentNode* InternalComponentNode => (AtkComponentNode*) InternalResNode;
	
	protected ComponentNode() : base((NodeType) 1001) {
		Component = NativeMemoryHelper.Create<T>();
		var componentBase = (AtkComponentBase*) Component;
		
		Data = NativeMemoryHelper.UiAlloc<TU>();

		componentBase->Initialize();
					
		CollisionNode = new CollisionNode {
			NodeId = 1,
			LinkedComponent = componentBase,
			NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.HasCollision | NodeFlags.RespondToMouse | NodeFlags.Focusable | NodeFlags.EmitsEvents,
		};

		CollisionNode.InternalResNode->ParentNode = InternalResNode;
		
		componentBase->OwnerNode = InternalNode;
		componentBase->AtkResNode = CollisionNode.InternalResNode;
		componentBase->ComponentFlags = 1;
		
		ref var uldManager = ref componentBase->UldManager;

		uldManager.Objects = (AtkUldObjectInfo*) NativeMemoryHelper.UiAlloc<AtkUldComponentInfo>();
		ref var objects = ref uldManager.Objects;
		uldManager.ObjectCount = 1;

		objects->NodeList = (AtkResNode**) NativeMemoryHelper.Malloc(8);
		objects->NodeList[0] = CollisionNode.InternalResNode;
		objects->NodeCount = 1;
		objects->Id = 1001;
		
		uldManager.InitializeResourceRendererManager();
		uldManager.RootNode = CollisionNode.InternalResNode;
		
		uldManager.UpdateDrawNodeList();
		uldManager.ResourceFlags = AtkUldManagerResourceFlag.Initialized |
		                           AtkUldManagerResourceFlag.KeepUldResourceHandle |
		                           AtkUldManagerResourceFlag.ArraysAllocated |
		                           AtkUldManagerResourceFlag.AssetsHaveThemeSupport;

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
			
			ComponentBase->Dtor(1);

			base.Dispose(disposing);
		}
	}

	protected void SetInternalComponentType(ComponentType type) {
		var componentInfo = (AtkUldComponentInfo*) ComponentBase->UldManager.Objects;
		
		componentInfo->ComponentType = type;
	}

	protected void InitializeComponentEvents() {
		ComponentBase->InitializeFromComponentData(DataBase);
		
		ComponentBase->Setup();
		ComponentBase->SetEnabledState(true);
	}
		
	public override float Width {
		get => base.Width;
		set {
			CollisionNode.Width = value;
			ComponentBase->UldManager.RootNodeWidth = (ushort) value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			CollisionNode.Height = value;
			ComponentBase->UldManager.RootNodeWidth = (ushort) value;
			base.Height = value;
		}
	}
}