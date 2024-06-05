using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;

public unsafe class CounterNode : NodeBase<AtkCounterNode> {
    public CounterNode() : base(NodeType.Counter) {
        var asset = NativeMemoryHelper.UiAlloc<AtkUldAsset>();
        asset->Id = 1;
        asset->AtkTexture.Ctor();
        asset->AtkTexture.LoadTexture("ui/uld/Money_Number_hr1.tex");

        var part = NativeMemoryHelper.UiAlloc<AtkUldPart>();
        part->UldAsset = asset;
        part->U = 0;
        part->V= 0;
        part->Height = 22;
        part->Width = 22;

        var partsList = NativeMemoryHelper.UiAlloc<AtkUldPartsList>();
        partsList->Parts = part;
        partsList->Id = 1;
        partsList->PartCount = 1;
        
        InternalNode->PartsList = partsList;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            InternalNode->PartsList->Parts->UldAsset->AtkTexture.ReleaseTexture();
            
            NativeMemoryHelper.UiFree(InternalNode->PartsList->Parts->UldAsset);
            NativeMemoryHelper.UiFree(InternalNode->PartsList->Parts);
            NativeMemoryHelper.UiFree(InternalNode->PartsList);
            
            base.Dispose(disposing);
        }
    }

    public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = value;
    }

    public AtkUldPartsList* PartsList {
        get => InternalNode->PartsList;
        set => InternalNode->PartsList = value;
    }

    public uint NumberWidth {
        get => InternalNode->NumberWidth;
        set => InternalNode->NumberWidth = (byte)value;
    }

    public uint CommaWidth {
        get => InternalNode->CommaWidth;
        set => InternalNode->CommaWidth = (byte)value;
    }

    public uint SpaceWidth {
        get => InternalNode->SpaceWidth;
        set => InternalNode->SpaceWidth = (byte) value;
    }

    /// <remarks>Might not be a enum at all.</remarks>
    public TextAlignment TextAlignment {
        get => (TextAlignment) InternalNode->TextAlign;
        set => InternalNode->TextAlign = (ushort) value;
    }

    public float CounterWidth {
        get => InternalNode->CounterWidth;
        set => InternalNode->CounterWidth = value;
    }

    public int Number {
        get => int.Parse(InternalNode->NodeText.ToString());
        set => InternalNode->SetNumber(value);
    }

    public string Text {
        get => InternalNode->NodeText.ToString();
        set => InternalNode->SetText($"{int.Parse(value):n0}");
    }
}

public enum TextAlignment {
    Unknown = 5,
}