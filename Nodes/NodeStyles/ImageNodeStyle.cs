using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.NodeStyles;

public class ImageNodeStyle : NodeBaseStyle {
    public WrapMode WrapMode;
    public ImageNodeFlags ImageNodeFlags;

    public ImageStyleDisable ImageStyleDisable = ImageStyleDisable.None;
        
    public override bool DrawSettings() {
        var configChanged = base.DrawSettings();
        
        if (!ImageStyleDisable.HasFlag(ImageStyleDisable.WrapMode))
            configChanged |= ComboHelper.EnumCombo("Wrap Mode", ref WrapMode);
        
        if (!ImageStyleDisable.HasFlag(ImageStyleDisable.ImageFlags))
            configChanged |= ComboHelper.EnumCombo("Image Node Flags", ref ImageNodeFlags);
        
        return configChanged;
    }
}

[Flags]
public enum ImageStyleDisable {
    None = 0,
    WrapMode = 1 << 1,
    ImageFlags = 1 << 2,
}