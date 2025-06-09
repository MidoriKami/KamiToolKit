using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

	public required string InternalName { get; init; } = "NameNotSet";

	public required string Title { get; set; } = "TitleNotSet";

	public string Subtitle { get; set; } = string.Empty;

	public required NativeController NativeController { get; init; }

	public int OpenWindowSoundEffectId { get; set; } = 32;

	public TitleMenuOptions TitleMenuOptions { get; set; } = new() {
		Enable = true,
		ShowClose = true,
		ShowScale = true,
	};

	public WindowOptions WindowOptions { get; set; } = new() {
		DisableClamping = true,
	};

	public required Vector2 Size { get; set; }

	public Vector2 ContentStartPosition => WindowNode.ContentStartPosition;

	public Vector2 ContentSize => WindowNode.ContentSize;

	private Vector2? InternalPosition { get; set; }

	public Vector2 Position {
		get => GetPosition();
		set => InternalPosition = value;
	}

	public bool IsOpen => InternalAddon is not null && InternalAddon->IsVisible;

	public int AddonId => InternalAddon is null ? 0 : InternalAddon->Id;

	private void SetInitialState() {
		WindowNode.SetTitle(Title, Subtitle);
		
		InternalAddon->OpenSoundEffectId = (short) OpenWindowSoundEffectId;

		InternalAddon->SetSize((ushort) Size.X, (ushort) Size.Y);
		WindowNode.Size = Size;
		
		var screenSize = new Vector2(AtkStage.Instance()->ScreenSize.Width, AtkStage.Instance()->ScreenSize.Height);
		var defaultPosition = screenSize / 2.0f - Size / 2.0f;
		
		InternalAddon->SetPosition((short)defaultPosition.X, (short)defaultPosition.Y);
		
		UpdateFlags();
		UpdatePosition();
	}

	private void UpdateFlags() {
		// Note, some flags are default on, need to invert enable to clear them
		
		UpdateFlag(ref InternalAddon->Flags1A3, 0x20, WindowOptions.DisableClamping);
		UpdateFlag(ref InternalAddon->Flags1A3, 0x1, TitleMenuOptions.Enable);
		UpdateFlag(ref InternalAddon->Flags1A1, 0x4, !TitleMenuOptions.ShowClose);
		UpdateFlag(ref InternalAddon->Flags1C8, 0x800, !TitleMenuOptions.ShowScale);
	}

	public Vector2 GetPosition()
		=> new(InternalAddon->X, InternalAddon->Y);

	private void UpdatePosition() {
		if (InternalPosition is { } position) {
			InternalAddon->SetPosition((short) position.X, (short) position.Y);
			InternalPosition = null;
		}
	}

	private void SetFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T> {
		flagsField |= T.One << BitOperations.Log2((uint) flag);
	}

	private void ClearFlag<T>(ref T flagsField, int flag) where T : struct, IBinaryInteger<T>
		=> flagsField &= ~(T.One << BitOperations.Log2((uint) flag));

	private void UpdateFlag<T>(ref T flagsField, int flag, bool enable) where T : struct, IBinaryInteger<T> {
		if (enable) {
			SetFlag(ref flagsField, flag);
		}
		else {
			ClearFlag(ref flagsField, flag);
		}
	}
}

public class TitleMenuOptions {

	/// <summary>
	/// Enables right-clicking on the window header to open the window context menu
	/// </summary>
	public bool Enable { get; set; }
	
	/// <summary>
	/// Enable showing a close button in the context menu
	/// </summary>
	public bool ShowClose { get; set; }
	
	/// <summary>
	/// Enable showing the scale selector in the window context menu
	/// </summary>
	public bool ShowScale { get; set; }
}

public class WindowOptions {
	
	/// <summary>
	/// Setting to <em>True</em> allows the window to be moved past the edge of the window.
	/// </summary>
	public bool DisableClamping { get; set; }
}