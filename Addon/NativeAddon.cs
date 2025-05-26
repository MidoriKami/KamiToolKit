using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.ComponentNodes.Window;
using KamiToolKit.System;

namespace KamiToolKit.Addon;

[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Using native function pointers, pinning the pointer is required.")]
public unsafe partial class NativeAddon {

	internal AtkUnitBase.AtkUnitBaseVirtualTable* VirtualTable;
	internal AtkUnitBase* InternalAddon;
	public required string Name { get; set; } = "NameNotSet";

	private bool isDisposed;

	private ResNode? rootNode;
	private WindowNode? windowNode;

	private readonly AtkUnitBase.Delegates.Dtor destructorFunction;
	private readonly AtkUnitBase.Delegates.Initialize initializeFunction;
	private readonly AtkUnitBase.Delegates.Finalizer finalizerFunction;
	private readonly AtkUnitBase.Delegates.Hide2 softHideFunction;
	
	public NativeAddon() {
		DalamudInterface.Instance.Log.Debug("Starting Construction");
		
		InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();
		DalamudInterface.Instance.Log.Debug("Memory Allocated");

		InternalAddon->NameString = Name;
		DalamudInterface.Instance.Log.Debug("Name Set");

		// Overwrite virtual table with a custom copy
		VirtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * 73);
		NativeMemory.Copy(InternalAddon->VirtualTable, VirtualTable,0x8 * 73);
		InternalAddon->VirtualTable = VirtualTable;
		DalamudInterface.Instance.Log.Debug("Vtable Allocated, and Copied");

		destructorFunction = Destructor;
		initializeFunction = Initialize;
		finalizerFunction = Finalizer;
		softHideFunction = SoftHide;
		
		VirtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
		VirtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(initializeFunction);
		VirtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(finalizerFunction);
		VirtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(softHideFunction);
		
		DalamudInterface.Instance.Log.Debug("Delegates Set");

		InternalAddon->Flags1A2 |= 0b0100_0000; // don't save/load AddonConfig
		InternalAddon->OpenSoundEffectId = 23;
		DalamudInterface.Instance.Log.Debug("Flags Set");
	}

	public void Show()
		=> InternalAddon->Open(4);
	
	public virtual void OnDestroy() { }
	
	private AtkEventListener* Destructor(AtkUnitBase* thisPtr, byte flags) {
		DalamudInterface.Instance.Log.Debug("Destructor Called");
		
		OnDestroy();
		DalamudInterface.Instance.Log.Debug("Destroy Delegated");

		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(thisPtr, flags);
		DalamudInterface.Instance.Log.Debug("Calling Original");
		
		if ((flags & 1) != 0) {
			DalamudInterface.Instance.Log.Debug("Free Declared");
			
			NativeMemoryHelper.Free(VirtualTable, 0x8 * 73);
			VirtualTable = null;
			
			DalamudInterface.Instance.Log.Debug("Virtual Table Freed");
			
			NativeMemoryHelper.UiFree(InternalAddon);
			InternalAddon = null;
			
			DalamudInterface.Instance.Log.Debug("Addon Table Freed");
		}

		return result;
	}

	public virtual void OnInitialize() { }

	private void Initialize(AtkUnitBase* thisPtr) {
		DalamudInterface.Instance.Log.Debug("Initialize Called");
		
		AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);
		DalamudInterface.Instance.Log.Debug("Base Initialized Called");
		
		thisPtr->UldManager.InitializeResourceRendererManager();
		DalamudInterface.Instance.Log.Debug("Uld Resource Renderer Initialized");
		
		var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
		widgetInfo->Id = 1;
		widgetInfo->NodeCount = 0;
		widgetInfo->NodeList = null;
		widgetInfo->AlignmentType = 4;
		widgetInfo->X = 50.0f;
		widgetInfo->Y = 50.0f;
		DalamudInterface.Instance.Log.Debug("WidgetInfo Initialized");
		
		InternalAddon->UldManager.Objects = (AtkUldObjectInfo*) widgetInfo;
		InternalAddon->UldManager.ObjectCount = 1;
		InternalAddon->UldManager.Flags1 |= 4;
		DalamudInterface.Instance.Log.Debug("UldManager Flags");

		rootNode = new ResNode {
			NodeId = 1,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
		};
		
		InternalAddon->RootNode = rootNode.InternalResNode;
		NodeBase.AddNodeToUldObjectList(&InternalAddon->UldManager, rootNode.InternalResNode);
		
		windowNode = new WindowNode {
			Title = Name,
		};
		
		windowNode.AttachNode(rootNode, NodePosition.AsFirstChild);

		InternalAddon->WindowNode = (AtkComponentNode*) windowNode.InternalResNode;
		
		DalamudInterface.Instance.Log.Debug("Pre-Drawlist Update");
		InternalAddon->UldManager.UpdateDrawNodeList();
		DalamudInterface.Instance.Log.Debug("Post-Drawlist Update");
		
		DalamudInterface.Instance.Log.Debug("Virtual Delegate Called");
		
		InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;
		InternalAddon->Flags198 |= 2 << 0x1C; // UldManager finished loading the uld
		InternalAddon->Flags1A2 |= 4;  // LoadUldByName called
		DalamudInterface.Instance.Log.Debug("Addon Flags");
		
		InternalAddon->SetSize((ushort) 996.0f, (ushort) 670.0f);
		InternalAddon->UpdateCollisionNodeList(false);

		OnInitialize();
	}

	public virtual void OnFinalize() { }
	
	private void Finalizer(AtkUnitBase* thisPtr) {
		OnFinalize();
		
		// windowNode?.DetachNode();
		// windowNode?.Dispose();
		// 	
		// rootNode?.DetachNode();
		// rootNode?.Dispose();
		
		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}

	private void SoftHide(AtkUnitBase* addon) {
		AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
	}
}