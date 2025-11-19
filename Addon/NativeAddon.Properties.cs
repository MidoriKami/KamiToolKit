using System;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

    private void SetInitialState() {
        WindowNode.SetTitle(Title.ToString(), Subtitle.ToString());

        InternalAddon->OpenSoundEffectId = (short)OpenWindowSoundEffectId;

        var addonConfig = LoadAddonConfig();
        if (addonConfig.Position != Vector2.Zero) {
            Position = addonConfig.Position;
            InternalAddon->SetPosition((short)Position.X, (short)Position.Y);
        }
        else {
            var screenSize = new Vector2(AtkStage.Instance()->ScreenSize.Width, AtkStage.Instance()->ScreenSize.Height);
            var defaultPosition = screenSize / 2.0f - Size / 2.0f;
            InternalAddon->SetPosition((short)defaultPosition.X, (short)defaultPosition.Y);
        }

        if (addonConfig.Scale is not 1.0f) {
            var newScale = Math.Clamp(addonConfig.Scale, 0.25f, 6.0f);
            
            Scale = newScale;
            InternalAddon->SetScale(Scale, true);
        }

        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);
        WindowNode.Size = Size;

        UpdateFlags();
        UpdatePosition();
    }

    private void UpdateFlags() {
        // Note, some flags are default on, need to invert enable to clear them

        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x20, WindowOptions.DisableClamping);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x40, WindowOptions.EnableClickThrough);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A3, 0x1, TitleMenuOptions.Enable);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1A1, 0x4, !TitleMenuOptions.ShowClose);
        FlagHelper.UpdateFlag(ref InternalAddon->Flags1C8, 0x800, !TitleMenuOptions.ShowScale);
    }

    public Vector2 GetPosition()
        => new(InternalAddon->X, InternalAddon->Y);

    private void UpdatePosition() {
        if (InternalPosition is { } position) {
            InternalAddon->SetPosition((short)position.X, (short)position.Y);
            InternalPosition = null;
        }
    }

    private readonly JsonSerializerOptions serializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };

    private AddonConfig LoadAddonConfig() {
        var directory = DalamudInterface.Instance.PluginInterface.ConfigDirectory;
        var file = new FileInfo(Path.Combine(directory.FullName, $"{InternalName}.addon.json"));
        if (!file.Exists) {
            file.Create().Close();
            return new AddonConfig();
        }

        AddonConfig? addonConfig;
        
        try {
            var data = File.ReadAllText(file.FullName);
            addonConfig = JsonSerializer.Deserialize<AddonConfig>(data, serializerOptions);
            addonConfig ??= new AddonConfig();
        }
        catch (Exception e) {
            DalamudInterface.Instance.Log.Error(e, "Exception while deserializing AddonConfig, creating new config.");
            addonConfig = new AddonConfig();
        }
        
        return addonConfig;
    }

    private void SaveAddonConfig() {
        var directory = DalamudInterface.Instance.PluginInterface.ConfigDirectory;
        var file = new FileInfo(Path.Combine(directory.FullName, $"{InternalName}.addon.json"));

        var configData = new AddonConfig {
            Position = Position,
            Scale = InternalAddon->Scale / AtkUnitBase.GetGlobalUIScale(),
        };
        
        var data = JsonSerializer.Serialize(configData, serializerOptions);
        
        FilesystemUtil.WriteAllTextSafe(file.FullName, data);
    }

    public void SetWindowSize(Vector2 windowSize) {
        if (InternalAddon is null) return;

        Size = windowSize;
        InternalAddon->SetSize((ushort)Size.X, (ushort)Size.Y);
        WindowNode.Size = Size;
    }

    public void SetWindowSize(float width, float height)
        => SetWindowSize(new Vector2(width, height));

    public required string InternalName {
        get => field;
        init {
            var noSpaces = value.Replace(" ", "");
            field = noSpaces.Length > 31 ? noSpaces[..31] : noSpaces;
        }
    } = "NameNotSet";

    public required ReadOnlySeString Title { get; set; } = "TitleNotSet";

    public ReadOnlySeString Subtitle { get; set; }

    public required NativeController NativeController { get; init; }

    public int OpenWindowSoundEffectId { get; set; } = 23;

    public TitleMenuOptions TitleMenuOptions { get; init; } = new();

    public WindowOptions WindowOptions { get; init; } = new();

    public Vector2 Size { get; set; } = new(400.0f, 400.0f);

    public Vector2 ContentStartPosition => WindowNode.ContentStartPosition + ContentPadding;

    public Vector2 ContentSize => WindowNode.ContentSize - ContentPadding * 2.0f - new Vector2(0.0f, 4.0f);

    public Vector2 ContentPadding => new(8.0f, 0.0f);

    private Vector2? InternalPosition { get; set; }

    public float Scale { get; set; }

    public int DepthLayer { get; set; } = 4;

    public Vector2 Position {
        get => GetPosition();
        set => InternalPosition = value;
    }

    public bool IsOpen => InternalAddon is not null && InternalAddon->IsVisible;

    public int AddonId => InternalAddon is null ? 0 : InternalAddon->Id;

    public bool RememberClosePosition { get; set; } = true;

    public static implicit operator AtkUnitBase*(NativeAddon addon) => addon.InternalAddon;
}

public class TitleMenuOptions {

    /// <summary>
    ///     Enables right-clicking on the window header to open the window context menu
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    ///     Enable showing a close button in the context menu
    /// </summary>
    public bool ShowClose { get; set; } = true;

    /// <summary>
    ///     Enable showing the scale selector in the window context menu
    /// </summary>
    public bool ShowScale { get; set; } = true;
}

public class WindowOptions {

    /// <summary>
    ///     Setting to <em>True</em> allows the window to be moved past the edge of the window.
    /// </summary>
    public bool DisableClamping { get; set; } = true;

    public bool EnableClickThrough { get; set; } = false;

    /// <summary>
    ///     Setting to <em>False</em> will cause this window to not close when the game tries to close all open windows.
    /// </summary>
    /// <example>
    ///     When pressing <em>Escape</em> with no window focused, will close all open windows. 
    ///     When moving between zones, all open windows will be closed.
    /// </example>
    public bool RespectCloseAll { get; set; } = true;
}
