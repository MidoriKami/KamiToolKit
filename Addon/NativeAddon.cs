using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.ComponentNodes.Window;

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
	private readonly AtkUnitBase.Delegates.OnSetup onSetupFunction;
	
	public NativeAddon() {
		Log.Debug("Starting Construction");
		
		InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();
		Log.Debug("Memory Allocated");

		InternalAddon->NameString = Name;
		Log.Debug("Name Set");

		// Overwrite virtual table with a custom copy
		virtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * 73);
		NativeMemory.Copy(InternalAddon->VirtualTable, virtualTable,0x8 * 73);
		InternalAddon->VirtualTable = virtualTable;
		Log.Debug("Vtable Allocated, and Copied");

		destructorFunction = Destructor;
		initializeFunction = Initialize;
		finalizerFunction = Finalizer;
		softHideFunction = SoftHide;
		onSetupFunction = Setup;
		
		virtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
		virtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(initializeFunction);
		virtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(finalizerFunction);
		virtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(softHideFunction);
		virtualTable->OnSetup = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, void>) Marshal.GetFunctionPointerForDelegate(onSetupFunction);
		
		Log.Debug("Delegates Set");

		InternalAddon->Flags1A2 |= 0b0100_0000; // don't save/load AddonConfig
		InternalAddon->OpenSoundEffectId = 23;
		Log.Debug("Flags Set");
	}

	protected virtual void OnSetup(AtkUnitBase* addon) { }

	private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
		InternalAddon->SetSize((ushort) 500.0f, (ushort) 350.0f);
		
		OnSetup(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->OnSetup(addon, valueCount, values);
	}

	public void Open(int depthLayer = 4) {
		if (InternalAddon is null) return;
		
		InternalAddon->Open((uint) depthLayer);
	}

	public void Close() {
		Log.Debug("Close Attempted");
		if (InternalAddon is null) return;

		Log.Debug("Closing");
		InternalAddon->Close(false);
		
		Log.Debug("Closed");
	}

	public virtual void OnDestroy() { }
	
	private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
		Log.Debug("Destructor Called");
		
		OnDestroy();

		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

		if ((flags & 1) == 1) {
			InternalAddon = null;
		}

		return result;
	}

	public virtual void OnInitialize() { }

	private void Initialize(AtkUnitBase* thisPtr) {
		Log.Debug("Initialize Called");
		
		AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);
		Log.Debug("Base Initialized Called");
		
		thisPtr->UldManager.InitializeResourceRendererManager();
		Log.Debug("Uld Resource Renderer Initialized");
		
		var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
		widgetInfo->Id = 1;
		widgetInfo->NodeCount = 0;
		widgetInfo->NodeList = null;
		widgetInfo->AlignmentType = 4;
		widgetInfo->X = 50.0f;
		widgetInfo->Y = 50.0f;
		Log.Debug("WidgetInfo Initialized");
		
		InternalAddon->UldManager.Objects = (AtkUldObjectInfo*) widgetInfo;
		InternalAddon->UldManager.ObjectCount = 1;
		InternalAddon->UldManager.Flags1 |= 4;
		Log.Debug("UldManager Flags");

		rootNode = new ResNode {
			NodeId = 1,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
			SuppressDispose = true,
		};
		
		InternalAddon->RootNode = rootNode.InternalResNode;
		NodeLinker.AddNodeToUldObjectList(&InternalAddon->UldManager, rootNode.InternalResNode);
		
		windowNode = new WindowNode {
			Title = Name,
			SuppressDispose = true,
		};
		
		windowNode.AttachNode(rootNode, NodePosition.AsFirstChild);
		InternalAddon->WindowNode = (AtkComponentNode*) windowNode.InternalResNode;
		
		Log.Debug("Pre-Drawlist Update");
		InternalAddon->UldManager.UpdateDrawNodeList();
		Log.Debug("Post-Drawlist Update");
		
		Log.Debug("Virtual Delegate Called");
		
		InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;
		InternalAddon->Flags198 |= 2 << 0x1C; // UldManager finished loading the uld
		InternalAddon->Flags1A2 |= 4;  // LoadUldByName called
		Log.Debug("Addon Flags");
		
		windowNode.Size = new Vector2(500.0f, 350.0f);
		InternalAddon->UpdateCollisionNodeList(false);

		OnInitialize();
	}

	public virtual void OnFinalize() { }
	
	private void Finalizer(AtkUnitBase* addon) {
		Log.Debug("Finalizer Called");
		
		OnFinalize();
		
		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}

	private void SoftHide(AtkUnitBase* addon)
		=> AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
}