using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.ComponentNodes.Window;

namespace KamiToolKit.Addon;

[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Using native function pointers, pinning the pointer is required.")]
public abstract unsafe class NativeAddon :IDisposable {

	private readonly AtkUnitBase.AtkUnitBaseVirtualTable* virtualTable;
	internal AtkUnitBase* InternalAddon;
	public required string InternalName { get; init; } = "NameNotSet";
	public required string Title { get; set; } = "TitleNotSet";
	public string Subtitle { get; set; } = string.Empty;

	private bool isDisposed;

	private ResNode rootNode;
	private WindowNode windowNode;

	private readonly AtkUnitBase.Delegates.Dtor destructorFunction;
	private readonly AtkUnitBase.Delegates.Initialize initializeFunction;
	private readonly AtkUnitBase.Delegates.Finalizer finalizerFunction;
	private readonly AtkUnitBase.Delegates.Hide2 softHideFunction;
	private readonly AtkUnitBase.Delegates.OnSetup onSetupFunction;
	private readonly AtkUnitBase.Delegates.Draw drawFunction;
	private readonly AtkUnitBase.Delegates.Update updateFunction;
	private readonly AtkUnitBase.Delegates.Show showFunction;
	private readonly AtkUnitBase.Delegates.Hide hideFunction;

	protected virtual void OnSetup(AtkUnitBase* addon) { }
	protected virtual void OnShow(AtkUnitBase* addon) { }
	protected virtual void OnHide(AtkUnitBase* addon) { }
	protected virtual void OnDraw(AtkUnitBase* addon) { }
	protected virtual void OnUpdate(AtkUnitBase* addon) { }
	protected virtual void OnFinalize(AtkUnitBase* addon) { }

	protected NativeAddon() {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Beginning Native Addon Construction");
		
		InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();

		// Ensure InternalName doesn't contain any spaces
		InternalName = InternalName.Replace(" ", string.Empty);
		InternalAddon->NameString = InternalName;

		// Overwrite virtual table with a custom copy
		virtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * 73);
		NativeMemory.Copy(InternalAddon->VirtualTable, virtualTable,0x8 * 73);
		InternalAddon->VirtualTable = virtualTable;

		destructorFunction = Destructor;
		initializeFunction = Initialize;
		finalizerFunction = Finalizer;
		softHideFunction = Hide2;
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
		
		rootNode = new ResNode {
			NodeId = 1,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
		};
		
		windowNode = new WindowNode();
		
		Log.Verbose($"[KamiToolKit] [{InternalName}] Construction Complete");
	}

	private void Initialize(AtkUnitBase* thisPtr) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Initialize");
		
		AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);

		thisPtr->UldManager.InitializeResourceRendererManager();

		var widgetInfo = NativeMemoryHelper.UiAlloc<AtkUldWidgetInfo>(1, 16);
		widgetInfo->Id = 1;
		widgetInfo->NodeCount = 0;
		widgetInfo->NodeList = null;
		widgetInfo->WidgetAlignment = new AtkWidgetAlignment {
			AlignmentType = AlignmentType.Center, 
			X = 50.0f, 
			Y = 50.0f,
		};

		InternalAddon->UldManager.Objects = (AtkUldObjectInfo*) widgetInfo;
		InternalAddon->UldManager.ObjectCount = 1;
		InternalAddon->UldManager.ResourceFlags |= AtkUldManagerResourceFlag.ArraysAllocated;

		InternalAddon->RootNode = rootNode.InternalResNode;
		NodeLinker.AddNodeToUldObjectList(&InternalAddon->UldManager, rootNode.InternalResNode);

		LoadTimeline();

		windowNode.AttachNode(rootNode, NodePosition.AsFirstChild);
		InternalAddon->WindowNode = (AtkComponentNode*) windowNode.InternalResNode;

		InternalAddon->UldManager.UpdateDrawNodeList();
		InternalAddon->UldManager.LoadedState = AtkLoadState.Loaded;

		// UldManager finished loading the uld
		InternalAddon->Flags198 |= 2 << 0x1C;
		
		// LoadUldByName called
		InternalAddon->Flags1A2 |= 4;

		InternalAddon->UpdateCollisionNodeList(false);
	}

	private void Finalizer(AtkUnitBase* addon) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Finalize");
		
		OnFinalize(addon);

		AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
	}

	private void Hide2(AtkUnitBase* addon) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] SoftHide (Hide2)");
		
		AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
	}

	private void Hide(AtkUnitBase* thisPtr, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Hide");
		
		OnHide(thisPtr);
		
		AtkUnitBase.StaticVirtualTablePointer->Hide(thisPtr, unkBool, callHideCallback, setShowHideFlags);
	}

	private void Show(AtkUnitBase* thisPtr, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Show");
		
		OnShow(thisPtr);
		
		AtkUnitBase.StaticVirtualTablePointer->Show(thisPtr, silenceOpenSoundEffect, unsetShowHideFlags);
	}

	private void Update(AtkUnitBase* addon, float delta) {
		Log.Excessive($"[KamiToolKit] [{InternalName}] Update");
		
		OnUpdate(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Update(addon, delta);
	}

	private void Draw(AtkUnitBase* addon) {
		Log.Excessive($"[KamiToolKit] [{InternalName}] Draw");
		
		OnDraw(addon);
		
		AtkUnitBase.StaticVirtualTablePointer->Draw(addon);
	}

	private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Setup");

		windowNode.SetTitle(Title, Subtitle);
		InternalAddon->SetSize((ushort) Size.X, (ushort) Size.Y);
		windowNode.Size = Size;

		AtkUnitBase.StaticVirtualTablePointer->OnSetup(addon, valueCount, values);

		OnSetup(addon);
	}

	public void Open(int depthLayer = 4) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Open");

		if (InternalAddon is null) return;

		InternalAddon->Open((uint) depthLayer);
	}

	public void Close() {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Close");
		
		if (InternalAddon is null) return;

		InternalAddon->Close(false);
	}
	
	private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Destructor");
		
		var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

		if ((flags & 1) == 1) {
			InternalAddon = null;
		}

		return result;
	}

	public Vector2 Size { get; set; }

	private void LoadTimeline() {
		rootNode.AddTimeline(new Timeline {
			Mask = (AtkTimelineMask) 0xFF,
			LabelEndFrameIdx = 89,
			LabelFrameIdxDuration = 88,
			LabelSets = [
				new TimelineLabelSet {
					StartFrameId = 1, EndFrameId = 89, Labels = [
						new TimelineLabelFrame { FrameIndex = 1,  LabelId = 101, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 10, LabelId = 102, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 20, LabelId = 103, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 30, LabelId = 104, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 40, LabelId = 105, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 50, LabelId = 106, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 60, LabelId = 107, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 70, LabelId = 108, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
						new TimelineLabelFrame { FrameIndex = 80, LabelId = 109, JumpBehavior = AtkTimelineJumpBehavior.PlayOnce },
					],
				},
			],
		});
	}
	
	~NativeAddon() => Dispose(false);

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			Dispose();
		}
	}

	public void Dispose() {
		if (!isDisposed) {
			Log.Debug($"[KamiToolKit] Disposing addon {GetType()}");

			Close();
            GC.SuppressFinalize(this);
		}
        
		isDisposed = true;
	}
}