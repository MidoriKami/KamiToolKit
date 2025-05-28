using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Classes;

public static unsafe class ColorHelper {
	public static Vector4 GetColor(uint colorId) {
		var colorEntry = DalamudInterface.Instance.DataManager.GetExcelSheet<UIColor>().GetRow(colorId);

		return CurrentTheme() switch {
			0 => ConvertToVector4(colorEntry.Dark),
			1 => ConvertToVector4(colorEntry.Light),
			2 => ConvertToVector4(colorEntry.ClassicFF),
			3 => ConvertToVector4(colorEntry.ClearBlue),
			_ => Vector4.One,
		};
	}

	private static int CurrentTheme()
		=> AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType;
	
	private static Vector4 ConvertToVector4(uint color) {
		var r = (byte)(color >> 24);
		var g = (byte)(color >> 16);
		var b = (byte)(color >> 8);
		var a = (byte)color;

		return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
	}
}