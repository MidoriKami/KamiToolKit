namespace KamiToolKit.Nodes;

public sealed unsafe class VerticalLineNode : HorizontalLineNode {
    public VerticalLineNode() {
        RotationDegrees = 90.0f;
    }

    public override float Height {
        get => InternalResNode->GetWidth();
        set => InternalResNode->SetWidth((ushort) value);
    }

    public override float Width { 
        get => InternalResNode->GetHeight();
        set => InternalResNode->SetHeight((ushort) value);
    }
}
