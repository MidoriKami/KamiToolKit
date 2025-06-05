using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Window;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

	internal AtkUnitBase* InternalAddon;

	private ResNode rootNode = null!;
	private WindowNode windowNode = null!;
	
	private GCHandle? disposeHandle;

	private void AllocateAddon() {
		if (InternalAddon is not null) {
			Log.Warning("Tried to allocate addon that was already allocated.");
			return;
		}

		Log.Verbose($"[KamiToolKit] [{InternalName}] Beginning Native Addon Allocation");

		InternalAddon = NativeMemoryHelper.Create<AtkUnitBase>();

		// Overwrite virtual table with a custom copy
		virtualTable = (AtkUnitBase.AtkUnitBaseVirtualTable*) NativeMemoryHelper.Malloc(0x8 * 73);
		NativeMemory.Copy(InternalAddon->VirtualTable, virtualTable,0x8 * 73);
		InternalAddon->VirtualTable = virtualTable;
		
		RegisterVirtualTable();

		InternalAddon->Flags1A2 |= 0b0100_0000; // don't save/load AddonConfig

		rootNode = new ResNode {
			NodeId = 1,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.Focusable | NodeFlags.EmitsEvents,
		};

		windowNode = new WindowNode();

		InternalAddon->NameString = GetInternalNameSafe();
		
		Log.Verbose($"[KamiToolKit] [{InternalName}] Allocation Complete");
	}

	private void InitializeAddon() {
		Log.Verbose($"[KamiToolKit] [{InternalName}] Initializing Addon");

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
		
		// Now that we have constructed this instance, track it for auto-dispose
		CreatedAddons.Add(this);

		Log.Verbose($"[KamiToolKit] [{InternalName}] Initialization Complete");
	}

	/// <summary>
	/// Initializes and Opens this instance of Addon
	/// </summary>
	/// <param name="depthLayer">Which UI layer to attach the Addon to</param>
	public void Open(int depthLayer = 4)
		=> DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
			Log.Verbose($"[KamiToolKit] [{InternalName}] Open Called");

			if (InternalAddon is null) {
				AllocateAddon();

				if (InternalAddon is not null) {
					AtkStage.Instance()->RaptureAtkUnitManager->InitializeAddon(InternalAddon, InternalName);
					InternalAddon->Open((uint) depthLayer);
					disposeHandle = GCHandle.Alloc(this);
				}
			}
			else {
				Log.Verbose($"[KamiToolKit] [{InternalName}] Already open, skipping call.");
			}
		});

	public void Close() 
		=> DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
			Log.Verbose($"[KamiToolKit] [{InternalName}] Close");

			if (InternalAddon is null) return;
			InternalAddon->Close(false);
		});

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

	private string GetInternalNameSafe() {
		var noSpaces = InternalName.Replace(" ", string.Empty);

		if (noSpaces.Length > 31) {
			noSpaces = noSpaces[..31];
		}
		
		noSpaces += char.MinValue;
		return noSpaces;
	}
}