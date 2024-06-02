using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;

namespace KamiToolKit;

[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "This class is a library utility, it is meant to provide the functionality to other assemblies")]
public abstract unsafe class NodeBase<T> : IDisposable where T : unmanaged, ICreatable {
    protected T* InternalNode { get; }
    
    private AtkResNode* InternalResNode => (AtkResNode*) InternalNode;

    private IAddonLifecycle? addonLifecycleService;

    protected NodeBase(NodeType nodeType) {
        InternalNode = IMemorySpace.GetUISpace()->Create<T>();
        InternalResNode->Type = nodeType;

        if (InternalNode is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }
    }
    
    public void Dispose() {
        UnAttachNode();
        
        InternalResNode->Destroy(false);
        IMemorySpace.Free(InternalNode);
    }
    
    public float X {
        get => InternalResNode->GetX();
        set => InternalResNode->SetX(value);
    }

    public float Y {
        get => InternalResNode->GetY();
        set => InternalResNode->SetY(value);
    }

    public Vector2 Position {
        get => new(X, Y);
        set => InternalResNode->SetPositionFloat(value.X, value.Y);
    }

    public float ScreenX {
        get => InternalResNode->ScreenX;
        set => InternalResNode->ScreenX = value;
    }

    public float ScreenY {
        get => InternalResNode->ScreenY;
        set => InternalResNode->ScreenY = value;
    }

    public Vector2 ScreenPosition {
        get => new(ScreenX, ScreenY);
        set {
            ScreenX = value.X;
            ScreenY = value.Y;
        }
    }
    
    public float Width {
        get => InternalResNode->GetWidth();
        set => InternalResNode->SetWidth((ushort)value);
    }

    public float Height {
        get => InternalResNode->Height;
        set => InternalResNode->SetHeight((ushort) value);
    }
    
    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    private float ScaleX {
        get => InternalResNode->GetScaleX();
        set => InternalResNode->SetScaleX(value);
    }

    private float ScaleY {
        get => InternalResNode->GetScaleY();
        set => InternalResNode->SetScaleY(value);
    }
    
    public Vector2 Scale {
        get => new(ScaleX, ScaleY);
        set => InternalResNode->SetScale(value.X, value.Y);
    }

    public float Rotation {
        get => InternalResNode->Rotation;
        set => InternalResNode->Rotation = value;
    }

    private float OriginX {
        get => InternalResNode->OriginX;
        set => InternalResNode->OriginX = value;
    }

    private float OriginY {
        get => InternalResNode->OriginY;
        set => InternalResNode->OriginY = value;
    }

    private Vector2 Origin {
        get => new(OriginX, OriginY);
        set {
            OriginX = value.X;
            OriginY = value.Y;
        }
    }

    public bool IsVisible {
        get => InternalResNode->IsVisible;
        set => InternalResNode->ToggleVisibility(value);
    }

    public NodeFlags NodeFlags {
        get => InternalResNode->NodeFlags;
        set => InternalResNode->NodeFlags = value;
    }

    public void AddFlags(NodeFlags flags)
        => NodeFlags |= flags;

    public void RemoveFlags(NodeFlags flags)
        => NodeFlags &= ~flags;

    public Vector4 Color {
        get => InternalResNode->Color.ToVector4();
        set => InternalResNode->Color = value.ToByteColor();
    }

    public float Alpha {
        get => InternalResNode->Color.A;
        set => InternalResNode->Color.A = (byte)(value * 255.0f);
    }

    public Vector3 AddColor {
        get => new(InternalResNode->AddRed, InternalResNode->AddGreen, InternalResNode->AddBlue);
        set {
            InternalResNode->AddRed = (short)value.X;
            InternalResNode->AddGreen = (short)value.Y;
            InternalResNode->AddBlue = (short)value.Z;
        }
    }

    public Vector3 MultiplyColor {
        get => new(InternalResNode->MultiplyRed, InternalResNode->MultiplyGreen, InternalResNode->MultiplyBlue);
        set {
            InternalResNode->MultiplyRed = (byte)value.X;
            InternalResNode->MultiplyGreen = (byte)value.Y;
            InternalResNode->MultiplyBlue = (byte)value.Z;
        }
    }

    public uint ChildCount => InternalResNode->ChildCount;

    private AtkResNode* ParentNode {
        get => InternalResNode->ParentNode;
        set => InternalResNode->ParentNode = value;
    }
    
    private AtkResNode* PrevSiblingNode {
        get => InternalResNode->PrevSiblingNode;
        set => InternalResNode->PrevSiblingNode = value;
    }
    
    private AtkResNode* NextSiblingNode {
        get => InternalResNode->NextSiblingNode;
        set => InternalResNode->NextSiblingNode = value;
    }
    
    private AtkResNode* ChildNode {
        get => InternalResNode->ChildNode;
        set => InternalResNode->ChildNode = value;
    }
    
    public uint NodeID {
        get => InternalResNode->NodeId;
        set => InternalResNode->NodeId = value;
    }

    // Attaches a self-removing node to an addon, when the addon finalizes, or the plugin unloads, attached nodes will automatically be removed.
    public void AttachNodeSafe(IAddonLifecycle addonLifecycle, void* addon, AtkResNode* targetNode, NodePosition position) {
        var atkUnitBase = (AtkUnitBase*) addon;

        addonLifecycleService ??= addonLifecycle;
        
        addonLifecycle.RegisterListener(AddonEvent.PreFinalize, atkUnitBase->NameString, AutomaticNodeCleanup);
        AttachNode(atkUnitBase, targetNode, position);
    }

    private void AutomaticNodeCleanup(AddonEvent type, AddonArgs args) {
        UnAttachNode();
        addonLifecycleService!.UnregisterListener(AutomaticNodeCleanup);
        Dispose();
    }

    private void AttachNode(void* atkUnitBase, AtkResNode* targetNode, NodePosition position)
        => AttachNode((AtkUnitBase*) atkUnitBase, targetNode, position);
    
    private void AttachNode(AtkUnitBase* parent, AtkResNode* targetNode, NodePosition position) {
        switch (position) {
            case NodePosition.BeforeTarget:
                EmplaceBefore(targetNode);
                break;

            case NodePosition.AfterTarget:
                EmplaceAfter(targetNode);
                break;

            case NodePosition.BeforeAllSiblings:
                EmplaceBeforeSiblings(targetNode);
                break;

            case NodePosition.AfterAllSiblings:
                EmplaceAfterSiblings(targetNode);
                break;
            
            case NodePosition.AsLastChild:
                EmplaceAsLastChild(targetNode);
                break;
            
            case NodePosition.AsFirstChild:
                EmplaceAsFirstChild(targetNode);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
        
        parent->UpdateCollisionNodeList(false);
        parent->UldManager.UpdateDrawNodeList();
    }
    
     private void EmplaceBefore(AtkResNode* targetNode) {
        InternalResNode->ParentNode = targetNode->ParentNode;

        // Target node is the head of the nodelist, we will be the new head.
        if (targetNode->NextSiblingNode is null) {
            targetNode->ParentNode->ChildNode = InternalResNode;
        }

        // We have a node that will be before us
        if (targetNode->NextSiblingNode is not null) {
            targetNode->NextSiblingNode->PrevSiblingNode = InternalResNode;
            InternalResNode->NextSiblingNode = targetNode->NextSiblingNode;
        }

        targetNode->NextSiblingNode = InternalResNode;
        InternalResNode->PrevSiblingNode = targetNode;
        
        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceAfter(AtkResNode* targetNode) {
        InternalResNode->ParentNode = targetNode->ParentNode;

        // We have a node that will be after us
        if (targetNode->PrevSiblingNode is not null) {
            targetNode->PrevSiblingNode->NextSiblingNode = InternalResNode;
            InternalResNode->PrevSiblingNode = targetNode->PrevSiblingNode;
        }

        targetNode->PrevSiblingNode = InternalResNode;
        InternalResNode->NextSiblingNode = targetNode;
        
        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceBeforeSiblings(AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->NextSiblingNode;
        }

        if (previous is not null) {
            EmplaceBefore(previous);
        }
        
        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceAfterSiblings(AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->PrevSiblingNode;
        }

        if (previous is not null) {
            EmplaceAfter(previous);
        }

        targetNode->ParentNode->ChildCount++;
    }

    private void EmplaceAsLastChild(AtkResNode* targetNode) {
        // If the child list is empty
        if (targetNode->ChildNode is null)
        {
            targetNode->ChildNode = InternalResNode;
            InternalResNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
        // Else Add to the List
        else
        {
            var currentNode = targetNode->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
        
            InternalResNode->ParentNode = targetNode;
            InternalResNode->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = InternalResNode;
            targetNode->ChildCount++;
        }
    }
    
    private void EmplaceAsFirstChild(AtkResNode* targetNode) {
        // If the child list is empty
        if (targetNode->ChildNode is null && targetNode->ChildCount is 0)
        {
            targetNode->ChildNode = InternalResNode;
            InternalResNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
        // Else Add to the List as the First Child
        else {
            targetNode->ChildNode->NextSiblingNode = InternalResNode;
            InternalResNode->PrevSiblingNode = targetNode->ChildNode;
            targetNode->ChildNode = InternalResNode;
            InternalResNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
    }

    public void UnAttachNode() {
        if (InternalResNode is null) return;
        if (InternalResNode->ParentNode is null) return;
        
        // If we were the main child of the containing node, assign it to the next element in line.
        if (InternalResNode->ParentNode->ChildNode == InternalResNode) {
            // And we have a node after us, our parents child should be the next node in line.
            if (InternalResNode->PrevSiblingNode != null) {
                InternalResNode->ParentNode->ChildNode = InternalResNode->PrevSiblingNode;
            }
            // else our parent is no longer pointing to any children.
            else {
                InternalResNode->ParentNode->ChildNode = null;
            }
        }
        
        // If we have a node before us
        if (InternalResNode->NextSiblingNode != null) {
            // and a node after us, link the one before to the one after
            if (InternalResNode->PrevSiblingNode != null) {
                InternalResNode->NextSiblingNode->PrevSiblingNode = InternalResNode->PrevSiblingNode;
            }
            // else unlink it from us
            else {
                InternalResNode->NextSiblingNode->PrevSiblingNode = null;
            }
        }
        
        // If we have a node after us
        if (InternalResNode->PrevSiblingNode != null) {
            // and a node before us, link the one after to the one before
            if (InternalResNode->NextSiblingNode != null) {
                InternalResNode->PrevSiblingNode->NextSiblingNode = InternalResNode->NextSiblingNode;
            }
            // else unlink it from us
            else {
                InternalResNode->PrevSiblingNode->NextSiblingNode = null;
            }
        }
    }
}

public enum NodePosition {
    BeforeTarget,
    AfterTarget,
    BeforeAllSiblings,
    AfterAllSiblings,
    AsLastChild,
    AsFirstChild
}