using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

/// <summary>
/// A simple image node for use with displaying game icons.
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public class IconImageNode : SimpleImageNode {
	private uint internalIconId;
	
	[JsonIgnore] public uint IconId {
		get => internalIconId;
		set {
			if (internalIconId != value) {
				PartsList[0].LoadIcon(value);
				internalIconId = value;
			}
		}
	}
    
	public uint? LoadedIconId 
		=> PartsList[0].GetLoadedIconId();
}