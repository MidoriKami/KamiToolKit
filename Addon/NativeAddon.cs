﻿using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
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
		rootNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 89)
			.AddLabel(1, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(10, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(20, 103, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(30, 104, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(40, 105, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(50, 106, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(60, 107, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(70, 108, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(80, 109, AtkTimelineJumpBehavior.PlayOnce, 0)
			.EndFrameSet()
			.Build());
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