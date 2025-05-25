using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes.ComponentNodes.Window;

public unsafe class WindowBackgroundNode : NineGridNode {
	public WindowBackgroundNode(bool selectedPath) {
		var basePath = $"ui/uld/WindowA_Bg{(selectedPath ? "Selected" : "Normal")}";
		
		PartsList.Add(
			new Part {
			TextureCoordinates = new Vector2(0.0f, 0.0f), 
			Size = new Vector2(16.0f, 64.0f), 
			Id = 0,
			TexturePath = $"{basePath}_Corner_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(0.0f, 0.0f), 
			Size = new Vector2(32.0f, 64.0f), 
			Id = 1,
			TexturePath = $"{basePath}_H_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(16.0f, 0.0f), 
			Size = new Vector2(16.0f, 64.0f), 
			Id = 2,
			TexturePath = $"{basePath}_Corner_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(0.0f, 0.0f), 
			Size = new Vector2(16.0f, 32.0f), 
			Id = 3,
			TexturePath = $"{basePath}_V_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(0.0f, 0.0f), 
			Size = new Vector2(32.0f, 32.0f), 
			Id = 4,
			TexturePath = $"{basePath}_HV_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(16.0f, 0.0f), 
			Size = new Vector2(16.0f, 32.0f), 
			Id = 5,
			TexturePath = $"{basePath}_V_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(0.0f, 64.0f), 
			Size = new Vector2(16.0f, 32.0f), 
			Id = 6,
			TexturePath = $"{basePath}_Corner_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(0.0f, 64.0f), 
			Size = new Vector2(32.0f, 32.0f), 
			Id = 7,
			TexturePath = $"{basePath}_H_hr1.tex",
		}, new Part {
			TextureCoordinates = new Vector2(16.0f, 64.0f), 
			Size = new Vector2(16.0f, 32.0f), 
			Id = 8,
			TexturePath = $"{basePath}_Corner_hr1.tex",
		});
	}
}