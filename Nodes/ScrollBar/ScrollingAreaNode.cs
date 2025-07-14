using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class ScrollingAreaNode : ResNode {

	public readonly ResNode ContentAreaNode;
	public readonly ResNode ContentAreaClipNode;
	public readonly CollisionNode ScrollingCollisionNode;
	public readonly ScrollBarNode ScrollBarNode;

	public ScrollingAreaNode() {
		ScrollingCollisionNode = new CollisionNode {
			IsVisible = true,
			EventFlagsSet = true,
		};
		
		ScrollingCollisionNode.AttachNode(this);

		ContentAreaClipNode = new ResNode {
			NodeFlags = NodeFlags.Clip, 
			IsVisible = true,
		};
		ContentAreaClipNode.AttachNode(this);
		
		ContentAreaNode = new ResNode {
			IsVisible = true,
		};

		ContentAreaNode.AttachNode(ContentAreaClipNode);
		
		ScrollBarNode = new ScrollBarNode {
			ContentNode = ContentAreaNode,
			ContentCollisionNode = ScrollingCollisionNode,
			IsVisible = true,
		};

		ScrollBarNode.AttachNode(this);
		
		ScrollingCollisionNode.InternalResNode->AtkEventManager.RegisterEvent(
			AtkEventType.MouseWheel, 
			5, 
			null, 
			(AtkEventTarget*) ScrollingCollisionNode.InternalResNode,
			(AtkEventListener*) ScrollBarNode.Component, 
			false);
		
		ContentAreaNode.InternalResNode->AtkEventManager.RegisterEvent(
			AtkEventType.MouseWheel, 
			5, 
			null, 
			(AtkEventTarget*) ScrollingCollisionNode.InternalResNode,
			(AtkEventListener*) ScrollBarNode.Component, 
			false);
	}

	public ResNode ContentNode => ContentAreaNode;

	public int ScrollPosition {
		get => ScrollBarNode.ScrollPosition;
		set => ScrollBarNode.ScrollPosition = value;
	}

	public int ScrollSpeed {
		get => ScrollBarNode.ScrollSpeed;
		set => ScrollBarNode.ScrollSpeed = value;
	}

	public required float ContentHeight {
		get => ContentAreaNode.Height;
		set {
			ContentAreaNode.Height = value;
			ScrollBarNode.UpdateScrollParams();
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			ContentAreaNode.Width = value - 16.0f;
			ScrollingCollisionNode.Width = value - 16.0f;
			ContentAreaClipNode.Width = value - 16.0f;
			ScrollBarNode.Width = 8.0f;
			ScrollBarNode.X = Width - 8.0f; 
			ScrollBarNode.UpdateScrollParams();
		}
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			ScrollingCollisionNode.Height = value;
			ContentAreaClipNode.Height = value;
			ScrollBarNode.Height = value;
			ScrollBarNode.UpdateScrollParams();
		}
	}
}