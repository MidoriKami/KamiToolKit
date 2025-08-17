using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes;

public unsafe class TextInputNode : ComponentNode<AtkComponentTextInput, AtkUldComponentDataTextInput> {

    public delegate void TextInputVirtualFuncDelegate(AtkTextInput.AtkTextInputEventInterface* listener, ushort* numEvents);

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode CurrentTextNode;
    public readonly CursorNode CursorNode;
    public readonly NineGridNode FocusNode;
    public readonly TextInputSelectionListNode SelectionListNode;
    public readonly TextNode TextLimitsNode;

    public Action? OnFocused;

    public Action? OnUnfocused;

    private delegate* unmanaged<AtkTextInput.AtkTextInputEventInterface*, ushort*, void> originalFunction;
    private TextInputVirtualFuncDelegate? pinnedFunction;

    private AtkTextInputEventInterfaceVirtualTable* virtualTable;

    public TextInputNode() {
        SetInternalComponentType(ComponentType.TextInput);

        BackgroundNode = new SimpleNineGridNode {
            NodeId = 19,
            TexturePath = "ui/uld/TextInputA.tex",
            TextureCoordinates = new Vector2(24.0f, 0.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            Offsets = new Vector4(10.0f),
            Size = new Vector2(152.0f, 28.0f),
        };

        BackgroundNode.AttachNode(this);

        FocusNode = new SimpleNineGridNode {
            NodeId = 18,
            TexturePath = "ui/uld/TextInputA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            Offsets = new Vector4(10.0f),
            Size = new Vector2(152.0f, 28.0f),
            IsVisible = true,
        };

        FocusNode.AttachNode(this);

        TextLimitsNode = new TextNode {
            NodeId = 17,
            Position = new Vector2(-24.0f, 6.0f),
            Size = new Vector2(170.0f, 19.0f),
            FontType = FontType.MiedingerMed,
            FontSize = 14,
            AlignmentType = (AlignmentType)21,
            NodeFlags = NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        TextLimitsNode.AttachNode(this);

        CurrentTextNode = new TextNode {
            NodeId = 16,
            Position = new Vector2(10.0f, 6.0f),
            Size = new Vector2(132.0f, 18.0f),
            AlignmentType = AlignmentType.TopLeft,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TextFlags = TextFlags.AutoAdjustNodeSize,
            TextColor = ColorHelper.GetColor(1),
        };

        CurrentTextNode.AttachNode(this);

        SelectionListNode = new TextInputSelectionListNode {
            NodeId = 4, 
            Position = new Vector2(0.0f, 22.0f), 
            Size = new Vector2(186.0f, 208.0f), 
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        SelectionListNode.AttachNode(this);

        CursorNode = new CursorNode {
            NodeId = 2,
            Position = new Vector2(10.0f, 2.0f),
            Size = new Vector2(4.0f, 24.0f),
            OriginY = 4.0f,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        CursorNode.AttachNode(this);

        Data->Nodes[0] = CurrentTextNode.NodeId;
        Data->Nodes[1] = BackgroundNode.NodeId;
        Data->Nodes[2] = CursorNode.NodeId;
        Data->Nodes[3] = SelectionListNode.NodeId;
        Data->Nodes[4] = SelectionListNode.Buttons[8].NodeId;
        Data->Nodes[5] = SelectionListNode.Buttons[7].NodeId;
        Data->Nodes[6] = SelectionListNode.Buttons[6].NodeId;
        Data->Nodes[7] = SelectionListNode.Buttons[5].NodeId;
        Data->Nodes[8] = SelectionListNode.Buttons[4].NodeId;
        Data->Nodes[9] = SelectionListNode.Buttons[3].NodeId;
        Data->Nodes[10] = SelectionListNode.Buttons[2].NodeId;
        Data->Nodes[11] = SelectionListNode.Buttons[1].NodeId;
        Data->Nodes[12] = SelectionListNode.Buttons[0].NodeId;
        Data->Nodes[13] = SelectionListNode.LabelNode.NodeId;
        Data->Nodes[14] = SelectionListNode.BackgroundNode.NodeId;
        Data->Nodes[15] = TextLimitsNode.NodeId;

        Data->CandidateColor = new ByteColor {
            R = 66,
        };
        Data->IMEColor = new ByteColor {
            R = 67,
        };
        Data->FocusColor = KnownColor.Black.Vector().ToByteColor();

        // Flags1 = TextInputFlags1.EnableIME | TextInputFlags1.AllowUpperCase | TextInputFlags1.AllowLowerCase | TextInputFlags1.EnableDictionary;
        // Flags2 = TextInputFlags2.AllowNumberInput | TextInputFlags2.AllowSymbolInput;

        Flags1 = (TextInputFlags1)212;
        Flags2 = (TextInputFlags2)3;

        LoadTimelines();

        SetupVirtualTable();

        InitializeComponentEvents();

        CollisionNode.AddEvent(AddonEventType.InputReceived, InputComplete);
        CollisionNode.AddEvent(AddonEventType.FocusStart, _ => OnFocused?.Invoke());
        CollisionNode.AddEvent(AddonEventType.FocusStop, _ => OnUnfocused?.Invoke());
    }

    public Action<SeString>? OnInputReceived { get; set; }

    public Action<SeString>? OnInputComplete { get; set; }

    public int MaxCharacters {
        get => (int)Component->ComponentTextData.MaxChar;
        set => Component->ComponentTextData.MaxChar = (uint)value;
    }

    public bool ShowLimitText {
        get => TextLimitsNode.IsVisible;
        set => TextLimitsNode.IsVisible = value;
    }

    public TextInputFlags1 Flags1 {
        get => Data->Flags1;
        set => Data->Flags1 = value;
    }

    public TextInputFlags2 Flags2 {
        get => Data->Flags2;
        set => Data->Flags2 = value;
    }

    public SeString String {
        get => SeString.Parse(Component->UnkText1);
        set => Component->SetText(value.ToString());
    }

    private void FocusStart(AddonEventData obj)
        => OnFocused?.Invoke();

    private void SetupVirtualTable() {

        // Note: This virtual table only has 5 entries, but we will make it have 10 in-case square enix adds another entry
        var eventInterface = (AtkTextInputEventInterface*)&Component->AtkTextInputEventInterface;

        virtualTable = (AtkTextInputEventInterfaceVirtualTable*)NativeMemoryHelper.Malloc(0x8 * 10);
        NativeMemory.Copy(eventInterface->VirtualTable, virtualTable, 0x8 * 10);

        eventInterface->VirtualTable = virtualTable;

        pinnedFunction = OnInputChanged;

        originalFunction = virtualTable->OnInputReceived;
        virtualTable->OnInputReceived = (delegate* unmanaged<AtkTextInput.AtkTextInputEventInterface*, ushort*, void>)Marshal.GetFunctionPointerForDelegate(pinnedFunction);
    }

    private void OnInputChanged(AtkTextInput.AtkTextInputEventInterface* listener, ushort* numEvents) {
        originalFunction(listener, numEvents);

        try {
            OnInputReceived?.Invoke(SeString.Parse(Component->UnkText1));
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            NativeMemoryHelper.Free(virtualTable, 0x8 * 10);

            base.Dispose(disposing);
        }
    }

    private void InputComplete(AddonEventData data)
        => OnInputComplete?.Invoke(SeString.Parse(Component->UnkText1));

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;
        FocusNode.Size = Size;
        TextLimitsNode.Size = new Vector2(Width + 18.0f, Height - 9.0f);
        CurrentTextNode.Size = new Vector2(Width - 20.0f, Height - 10.0f);
    }

    private void LoadTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 29)
            .AddLabelPair(1, 9, 17)
            .AddLabelPair(10, 19, 18)
            .AddLabelPair(20, 29, 7)
            .EndFrameSet()
            .Build());

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 9, 1, alpha: 255)
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 255)
            .AddFrame(12, alpha: 255)
            .EndFrameSet()
            .AddFrameSetWithFrame(20, 29, 20, alpha: 127)
            .Build());

        FocusNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 0)
            .AddFrame(12, alpha: 255)
            .EndFrameSet()
            .Build());

        TextLimitsNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(1, 9, 1, alpha: 102)
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 102)
            .AddFrame(12, alpha: 127)
            .EndFrameSet()
            .AddFrameSetWithFrame(20, 29, 20, alpha: 76)
            .Build());

        CursorNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 15)
            .AddLabel(1, 101, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(15, 0, AtkTimelineJumpBehavior.LoopForever, 101)
            .EndFrameSet()
            .Build());
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public struct AtkTextInputEventInterface {
        [FieldOffset(0)] public AtkTextInputEventInterfaceVirtualTable* VirtualTable;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8 * 5)]
    public struct AtkTextInputEventInterfaceVirtualTable {
        [FieldOffset(8)] public delegate* unmanaged<AtkTextInput.AtkTextInputEventInterface*, ushort*, void> OnInputReceived;
    }
}
