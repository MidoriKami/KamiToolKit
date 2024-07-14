using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes;

public unsafe class TextNode() : NodeBase<AtkTextNode>(NodeType.Text) {
    public Vector4 TextColor {
        get => InternalNode->TextColor.ToVector4();
        set => InternalNode->TextColor = value.ToByteColor();
    }

    public Vector4 TextOutlineColor {
        get => InternalNode->EdgeColor.ToVector4();
        set => InternalNode->EdgeColor = value.ToByteColor();
    }

    public Vector4 BackgroundColor {
        get => InternalNode->BackgroundColor.ToVector4();
        set => InternalNode->BackgroundColor = value.ToByteColor();
    }

    public uint SelectStart {
        get => InternalNode->SelectStart;
        set => InternalNode->SelectStart = value;
    }

    public uint SelectEnd {
        get => InternalNode->SelectEnd;
        set => InternalNode->SelectEnd = value;
    }

    public AlignmentType AlignmentType {
        get => InternalNode->AlignmentType;
        set => InternalNode->AlignmentType = value;
    }

    public FontType FontType {
        get => InternalNode->FontType;
        set => InternalNode->FontType = value;
    }

    public TextFlags TextFlags {
        get => (TextFlags) InternalNode->TextFlags;
        set => InternalNode->TextFlags = (byte) value;
    }

    public TextFlags2 TextFlags2 {
        get => (TextFlags2) InternalNode->TextFlags2;
        set => InternalNode->TextFlags2 = (byte) value;
    }

    public uint FontSize {
        get => InternalNode->FontSize;
        set => InternalNode->FontSize = (byte) value;
    }

    public uint LineSpacing {
        get => InternalNode->LineSpacing;
        set => InternalNode->LineSpacing = (byte) value;
    }
    
    public uint CharSpacing {
        get => InternalNode->CharSpacing;
        set => InternalNode->CharSpacing = (byte) value;
    }

    public uint TextId {
        get => InternalNode->TextId;
        set => InternalNode->TextId = value;
    }

    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => InternalNode->SetNumber(number, showCommas, showPlusSign, (byte) digits, zeroPad);

    private nint stringMemoryBuffer = nint.Zero;
    
    /// <summary>
    /// If you want the node to resize automatically, use TextFlags.AutoAdjustNodeSize <b><em>before</em></b> setting the String property.
    /// </summary>
    public SeString Text {
        get => MemoryHelper.ReadSeStringNullTerminated((nint) InternalNode->GetText());
        set {
            var encodedString = value.Encode();
            var stringLength = encodedString.Length;

            Marshal.FreeHGlobal(stringMemoryBuffer);
            stringMemoryBuffer = Marshal.AllocHGlobal(stringLength + 1);
            Marshal.Copy(encodedString, 0, stringMemoryBuffer, stringLength);
            Marshal.WriteByte(stringMemoryBuffer, stringLength, 0);
            InternalNode->SetText((byte*)stringMemoryBuffer);
        }
    }
}