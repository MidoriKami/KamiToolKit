using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public unsafe class CounterNode() : NodeBase<AtkCounterNode>(NodeType.Counter) {

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
        set => InternalNode->SetText(value);
    }
}

public enum TextAlignment {
    
}