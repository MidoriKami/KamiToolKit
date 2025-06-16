using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public unsafe class ScrollingAreaNode : ResNode {

	protected readonly ResNode ContentAreaNode;
	protected readonly CollisionNode ScrollingCollisionNode;
	protected readonly ScrollBarNode ScrollBarNode;

	public ScrollingAreaNode() {
		NodeFlags = NodeFlags.Clip;
		
		ScrollingCollisionNode = new CollisionNode {
			IsVisible = true,
			EventFlagsSet = true,
		};
		
		ScrollingCollisionNode.AttachNode(this);
		
		ContentAreaNode = new ResNode {
			IsVisible = true,
		};

		ContentAreaNode.AttachNode(this);
		
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
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			ContentAreaNode.Dispose();
			ScrollingCollisionNode.Dispose();
			ScrollBarNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public ResNode ContentNode => ContentAreaNode;

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
			ScrollBarNode.Height = value;
			ScrollBarNode.UpdateScrollParams();
		}
	}
}