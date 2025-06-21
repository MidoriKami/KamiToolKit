using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class DragDropNode : ComponentNode<AtkComponentDragDrop, AtkUldComponentDataDragDrop> {

	public readonly ImageNode DragDropBackgroundNode;
	public readonly IconNode IconNode;
	
	public DragDropNode() {
		SetInternalComponentType(ComponentType.DragDrop);
		
		DragDropBackgroundNode = new SimpleImageNode {
			NodeId = 3,
			Size = new Vector2(44.0f, 44.0f),
			TexturePath = "ui/uld/DragTargetA.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(44.0f, 44.0f),
			WrapMode = 1,
			ImageNodeFlags = 0,
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
		};
		
		DragDropBackgroundNode.AttachNode(this);

		IconNode = new IconNode {
			NodeId = 2, 
			Size = new Vector2(44.0f, 48.0f), 
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
		};

		IconNode.AttachNode(this);

		LoadTimelines();

		Data->Nodes[0] = IconNode.NodeId;

		InitializeComponentEvents();
		
		// todo: investigate why these aren't being set automatically
		Component->AtkComponentIcon = IconNode.Component;
		Component->AtkDragDropInterface.ComponentNode = IconNode.InternalComponentNode;
	}

	public int IconId {
		get => (int) IconNode.IconId;
		set => IconNode.IconId = (uint) value;
	}

	private void LoadTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 59)
			.AddLabelPair(1, 10, 1)
			.AddLabelPair(11, 19, 2)
			.AddLabelPair(20, 29, 3)
			.AddLabelPair(30, 39, 7)
			.AddLabelPair(40, 49, 6)
			.AddLabelPair(50, 59, 4)
			.EndFrameSet()
			.Build());
	}
}