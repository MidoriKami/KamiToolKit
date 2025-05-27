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
	private readonly AtkUnitBase.Delegates.Draw drawFunction;
	private readonly AtkUnitBase.Delegates.Update updateFunction;
	private readonly AtkUnitBase.Delegates.Show showFunction;
	private readonly AtkUnitBase.Delegates.Hide hideFunction;

	protected virtual void OnInitialize(AtkUnitBase* addon) { }
	protected virtual void OnFinalize(AtkUnitBase* addon) { }
	protected virtual void OnSetup(AtkUnitBase* addon) { }
	protected virtual void OnDraw(AtkUnitBase* addon) { }
	protected virtual void OnUpdate(AtkUnitBase* addon) { }
	protected virtual void OnShow(AtkUnitBase* addon) { }
	protected virtual void OnHide(AtkUnitBase* addon) { }

	protected NativeAddon() {
		InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();

		InternalAddon->NameString = Name;

		// Overwrite virtual table with a custom copy
		virtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * 73);
		NativeMemory.Copy(InternalAddon->VirtualTable, virtualTable,0x8 * 73);
		InternalAddon->VirtualTable = virtualTable;

		destructorFunction = Destructor;
		initializeFunction = Initialize;
		finalizerFunction = Finalizer;
		softHideFunction = SoftHide;
		onSetupFunction = Setup;
		drawFunction = Draw;
		updateFunction = Update;
		showFunction = Show;
		hideFunction = Hide;
		
		virtualTable->Dtor = (delegate* unmanaged<AtkUnitBase*, byte, AtkEventListener*>) Marshal.GetFunctionPointerForDelegate(destructorFunction);
		virtualTable->Initialize = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(initializeFunction);
		virtualTable->Finalizer = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(finalizerFunction);
		virtualTable->Hide2 = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(softHideFunction);
		virtualTable->OnSetup = (delegate* unmanaged<AtkUnitBase*, uint, AtkValue*, void>) Marshal.GetFunctionPointerForDelegate(onSetupFunction);
		virtualTable->Draw = (delegate* unmanaged<AtkUnitBase*, void>) Marshal.GetFunctionPointerForDelegate(drawFunction);
		virtualTable->Update = (delegate* unmanaged<AtkUnitBase*, float, void>) Marshal.GetFunctionPointerForDelegate(updateFunction);
		virtualTable->Show = (delegate* unmanaged<AtkUnitBase*, bool, uint, void>) Marshal.GetFunctionPointerForDelegate(showFunction);
		virtualTable->Hide = (delegate* unmanaged<AtkUnitBase*, bool, bool, uint, void>) Marshal.GetFunctionPointerForDelegate(hideFunction);

		InternalAddon->Flags1A2 |= 0b0100_0000; // don't save/load AddonConfig
		InternalAddon->OpenSoundEffectId = 23;
	}

	private void Initialize(AtkUnitBase* thisPtr) {
		AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);

		thisPtr->UldManager.InitializeResourceRendererManager();

		var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
		widgetInfo->Id = 1;
		widgetInfo->NodeCount = 0;
		widgetInfo->NodeList = null;
		widgetInfo->AlignmentType = 4;
		widgetInfo->X = 50.0f;
		widgetInfo->Y = 50.0f;

		InternalAddon->UldManager.Objects = (AtkUldObjectInfo*) widgetInfo;
		InternalAddon->UldManager.ObjectCount = 1;
		InternalAddon->UldManager.Flags1 |= 4;

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

		InternalAddon->UldManager.UpdateDrawNodeList();
		InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;

		InternalAddon->Flags198 |= 2 << 0x1C; // UldManager finished loading the uld
		InternalAddon->Flags1A2 |= 4;  // LoadUldByName called

		windowNode.Size = new Vector2(500.0f, 350.0f);
		InternalAddon->UpdateCollisionNodeList(false);

		OnInitialize(thisPtr);
	}

	private void Finalizer(AtkUnitBase* addon) {
		OnFinalize(addon);

		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}

	private void SoftHide(AtkUnitBase* addon)
		=> AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
	
	
	private void Hide(AtkUnitBase* thisPtr, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
		OnHide(thisPtr);
		
		AtkUnitBase.StaticVirtualTablePointer->Hide(thisPtr, unkBool, callHideCallback, setShowHideFlags);
	}

	private void Show(AtkUnitBase* thisPtr, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
		OnShow(thisPtr);
		
		AtkUnitBase.StaticVirtualTablePointer->Show(thisPtr, silenceOpenSoundEffect, unsetShowHideFlags);
	}

	private void Update(AtkUnitBase* addon, float delta) {
		OnUpdate(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Update(addon, delta);
	}

	private void Draw(AtkUnitBase* addon) {
		OnDraw(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Draw(addon);
	}

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
		if (InternalAddon is null) return;

		InternalAddon->Close(false);
	}
	
	private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

		if ((flags & 1) == 1) {
			InternalAddon = null;
		}

		return result;
	}
}