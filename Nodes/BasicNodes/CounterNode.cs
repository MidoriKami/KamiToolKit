using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

/// <summary>
/// A counter node for displaying numbers
/// </summary>
public unsafe class CounterNode : NodeBase<AtkCounterNode> {
    protected readonly PartsList PartsList;
    
    public CounterNode() : base(NodeType.Counter) {
        PartsList = new PartsList();
        PartsList.Add( new Part() );
        
        InternalNode->PartsList = PartsList.InternalPartsList;

        NumberWidth = 10;
        CommaWidth = 8;
        SpaceWidth = 6;
        TextAlignment = TextAlignment.Unknown;
        CounterWidth = 32;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();
            
            base.Dispose(disposing);
        }
    }

    public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = value;
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