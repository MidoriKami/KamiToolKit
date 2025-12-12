namespace KamiToolKit.Nodes;

public sealed unsafe class VerticalLineNode : HorizontalLineNode {
    public VerticalLineNode() {
        RotationDegrees = 90.0f;
    }

    public override float Height {
        get => ResNode->GetWidth();
        set => ResNode->SetWidth((ushort) value);
    }

    public override float Width { 
        get => ResNode->GetHeight();
        set => ResNode->SetHeight((ushort) value);
    }
}
