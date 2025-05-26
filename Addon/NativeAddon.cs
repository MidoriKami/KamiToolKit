using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.ComponentNodes.Window;
using KamiToolKit.System;

namespace KamiToolKit.Addon;

[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Using native function pointers, pinning the pointer is required.")]
public abstract unsafe partial class NativeAddon {

	private readonly AtkUnitBase.AtkUnitBaseVirtualTable* virtualTable;
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
		virtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * 73);
		NativeMemory.Copy(InternalAddon->VirtualTable, virtualTable,0x8 * 73);
		InternalAddon->VirtualTable = virtualTable;
		DalamudInterface.Instance.Log.Debug("Vtable Allocated, and Copied");

		destructorFunction = Destructor;
		initializeFunction = Initialize;
		finalizerFunction = Finalizer;
		softHideFunction = SoftHide;
		
		virtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
		virtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(initializeFunction);
		virtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(finalizerFunction);
		virtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(softHideFunction);
		
		DalamudInterface.Instance.Log.Debug("Delegates Set");

		InternalAddon->Flags1A2 |= 0b0100_0000; // don't save/load AddonConfig
		InternalAddon->OpenSoundEffectId = 23;
		DalamudInterface.Instance.Log.Debug("Flags Set");
	}

	public void Open(int depthLayer = 4) {
		if (InternalAddon is null) return;
		
		InternalAddon->Open((uint) depthLayer);
	}

	public void Close() {
		if (InternalAddon is null) return;

		InternalAddon->Close(false);
	}

	public virtual void OnDestroy() { }
	
	private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
		DalamudInterface.Instance.Log.Debug("Destructor Called");
		
		OnDestroy();

		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

		if ((flags & 1) == 1) {
			InternalAddon = null;
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
		
		InternalAddon->SetSize((ushort) 500.0f, (ushort) 350.0f);
		InternalAddon->UpdateCollisionNodeList(false);

		OnInitialize();
	}

	public virtual void OnFinalize() { }
	
	private void Finalizer(AtkUnitBase* thisPtr) {
		OnFinalize();
		
		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}

	private void SoftHide(AtkUnitBase* addon)
		=> AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
}