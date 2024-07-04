namespace KamiToolKit.Nodes;

public class IconImageNode : ImageNode {
	private uint internalIconId;
	
	public uint IconId {
		get => internalIconId;
		set {
			if (internalIconId != value) {
				LoadIcon(value);
				internalIconId = value;
			}
		}
	}
}