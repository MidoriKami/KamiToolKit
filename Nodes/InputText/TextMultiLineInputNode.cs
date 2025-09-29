using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;


namespace KamiToolKit.Nodes;

public unsafe class TextMultiLineInputNode : ComponentNode<AtkComponentTextInput, AtkUldComponentDataTextInput> {

    public delegate void TextInputVirtualFuncDelegate(AtkTextInput.AtkTextInputEventInterface* listener, TextSelectionInfo* numEvents);

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode CurrentTextNode;
    public readonly CursorNode CursorNode;
    public readonly NineGridNode FocusNode;
    public readonly TextInputSelectionListNode SelectionListNode;
    public readonly TextNode TextLimitsNode;
    public readonly TextNode PlaceholderTextNode;

    public Action? OnFocused;

    public Action? OnUnfocused;

    private delegate* unmanaged<AtkTextInput.AtkTextInputEventInterface*, TextSelectionInfo*, void> originalFunction;
    private TextInputVirtualFuncDelegate? pinnedFunction;

    private AtkTextInputEventInterfaceVirtualTable* virtualTable;

    private float originalHeight;

    public TextMultiLineInputNode()
    {
        SetInternalComponentType(ComponentType.TextInput);

        BackgroundNode = new SimpleNineGridNode
        {
            NodeId = 19,
            TexturePath = "ui/uld/TextInputA.tex",
            TextureCoordinates = new Vector2(24.0f, 0.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            Offsets = new Vector4(10.0f),
            Size = new Vector2(152.0f, 28.0f),
        };
        BackgroundNode.AttachNode(this);

        FocusNode = new SimpleNineGridNode
        {
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

        TextLimitsNode = new TextNode
        {
            NodeId = 17,
            Position = new Vector2(-24.0f, 6.0f),
            Size = new Vector2(170.0f, 19.0f),
            FontType = FontType.MiedingerMed,
            FontSize = 14,
            AlignmentType = AlignmentType.BottomRight,
            NodeFlags = NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        TextLimitsNode.AttachNode(this);

        CurrentTextNode = new TextNode
        {
            NodeId = 16,
            Position = new Vector2(10.0f, 6.0f),
            Size = new Vector2(132.0f, 18.0f),
            AlignmentType = AlignmentType.TopLeft,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.MultiLine,
            TextColor = ColorHelper.GetColor(1),
            LineSpacing = 14,
        };
        CurrentTextNode.AttachNode(this);

        SelectionListNode = new TextInputSelectionListNode
        {
            NodeId = 4,
            Position = new Vector2(0.0f, 22.0f),
            Size = new Vector2(186.0f, 208.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        SelectionListNode.AttachNode(this);

        CursorNode = new CursorNode
        {
            NodeId = 2,
            Position = new Vector2(10.0f, 2.0f),
            Size = new Vector2(4.0f, 24.0f),
            OriginY = 4.0f,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        CursorNode.AttachNode(this);

        PlaceholderTextNode = new TextNode
        {
            Position = new Vector2(10.0f, 6.0f),
            IsVisible = true,
            TextColor = ColorHelper.GetColor(3),
        };
        PlaceholderTextNode.AttachNode(this);

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

        Data->CandidateColor = new ByteColor { R = 66 };
        Data->IMEColor = new ByteColor { R = 67 };
        Data->FocusColor = KnownColor.Black.Vector().ToByteColor();

        Flags = TextInputFlags.EnableIme | TextInputFlags.AllowUpperCase | TextInputFlags.AllowLowerCase |
                TextInputFlags.EnableDictionary | TextInputFlags.AllowNumberInput | TextInputFlags.AllowSymbolInput |
                TextInputFlags.MultiLine;

        LoadTimelines();

        SetupVirtualTable();

        InitializeComponentEvents();

        CollisionNode.AddEvent(AddonEventType.InputReceived, InputComplete);

        CollisionNode.AddEvent(AddonEventType.FocusStart, _ =>
        {
            PlaceholderTextNode.IsVisible = false;
            OnFocused?.Invoke();
        });

        CollisionNode.AddEvent(AddonEventType.FocusStop, _ =>
        {
            OnUnfocused?.Invoke();
            if (!PlaceholderString.IsNullOrEmpty() && String.IsNullOrEmpty())
            {
                PlaceholderTextNode.IsVisible = true;
                PlaceholderTextNode.String = PlaceholderString;
            }
        });
        
        Component->InputSanitizationFlags = (AllowedEntities)639;
        Component->ComponentTextData.Flags2 = (TextInputFlags2)11;
        Component->ComponentTextData.MaxLine = 64;
        Component->ComponentTextData.MaxByte = 65535;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            NativeMemoryHelper.Free(virtualTable, 0x8 * 10);

            base.Dispose(disposing);
        }
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

    public TextInputFlags Flags {
        get => (TextInputFlags) ((byte)Data->Flags1 | (byte)Data->Flags2 << 8);
        set {
            Data->Flags1 = (TextInputFlags1)((ushort)value & 0xFF);
            Data->Flags2 = (TextInputFlags2)((ushort)value >> 8);
        }
    }

    public SeString SeString {
        get => SeString.Parse(Component->UnkText1);
        set {
            Component->SetText(value.ToString());
            PlaceholderTextNode.IsVisible = PlaceholderString is not null && value.ToString().IsNullOrEmpty();
            UpdateHeightForContent();
        }
    }

    public string String {
        get => Component->UnkText1.ToString();
        set {
            Component->SetText(value);
            PlaceholderTextNode.IsVisible = PlaceholderString is not null && value.IsNullOrEmpty() && !FocusNode.IsVisible;
            UpdateHeightForContent();
        }
    }

    public string? PlaceholderString {
        get;
        set {
            field = value;
            PlaceholderTextNode.String = value ?? string.Empty;
        }
    }

    public bool AutoSelectAll { get; set; }

    public uint MaxLines {
        get => Component->ComponentTextData.MaxLine;
        set => Component->ComponentTextData.MaxLine = value;
    }

    public uint MaxBytes {
        get => Component->ComponentTextData.MaxByte;
        set => Component->ComponentTextData.MaxByte = value;
    }


    private void UpdateHeightForContent() {
        var text = String;
        var lineCount = Math.Max(1, text.Split('\r', '\n').Length);
        var lineHeight = CurrentTextNode.LineSpacing;
        var contentHeight = Math.Max(originalHeight, (lineCount * lineHeight) + 20);
        Height = contentHeight;
    }

    private void SetupVirtualTable() {
        var eventInterface = (AtkTextInputEventInterface*)&Component->AtkTextInputEventInterface;

        virtualTable = (AtkTextInputEventInterfaceVirtualTable*)NativeMemoryHelper.Malloc(0x8 * 10);
        NativeMemory.Copy(eventInterface->VirtualTable, virtualTable, 0x8 * 10);

        eventInterface->VirtualTable = virtualTable;

        pinnedFunction = OnCursorChanged;

        originalFunction = virtualTable->UpdateCursor;
        virtualTable->UpdateCursor = (delegate* unmanaged<AtkTextInput.AtkTextInputEventInterface*, TextSelectionInfo*, void>)Marshal.GetFunctionPointerForDelegate(pinnedFunction);
    }

    private void OnCursorChanged(AtkTextInput.AtkTextInputEventInterface* listener, TextSelectionInfo* numEvents) {
        var applySelectAll = !FocusNode.IsVisible && AutoSelectAll;

        if (applySelectAll) {
            numEvents->SelectionStart = 0;
            numEvents->SelectionEnd = numEvents->StringLength;
        }

        originalFunction(listener, numEvents);

        if (applySelectAll) {
            Marshal.WriteInt16((nint)AtkStage.Instance()->AtkInputManager->TextInput, 222, 0);
            Marshal.WriteInt16((nint)AtkStage.Instance()->AtkInputManager->TextInput, 224, (short)numEvents->StringLength);
        }

        try {
            OnInputReceived?.Invoke(SeString.Parse(Component->UnkText1));
            UpdateHeightForContent();
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    private void InputComplete(AddonEventData data) {
        if (UIInputData.Instance()->IsKeyPressed(SeVirtualKey.RETURN)) {
            var textInputComponent = InternalComponentNode->GetAsAtkComponentTextInput();
            var cursorPos = textInputComponent->CursorPos;

            using (var utf8String = new Utf8String()) {
                utf8String.SetString("\r");
                textInputComponent->WriteString(&utf8String);
            }

            DalamudInterface.Instance.Framework.RunOnTick(() => {
                textInputComponent->CursorPos = cursorPos + 1;
                textInputComponent->SelectionStart = cursorPos + 1;
                textInputComponent->SelectionEnd = cursorPos + 1;
            });
        }

        OnInputComplete?.Invoke(SeString.Parse(Component->UnkText1));
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        if (originalHeight <= 0f)
            originalHeight = Height;
        BackgroundNode.Size = Size;
        FocusNode.Size = Size;
        PlaceholderTextNode.Size = Size;
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
        [FieldOffset(8)] public delegate* unmanaged<AtkTextInput.AtkTextInputEventInterface*, TextSelectionInfo*, void> UpdateCursor;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public struct TextSelectionInfo {
        [FieldOffset(0x0)] public bool CharacterAdded;
        [FieldOffset(0x2)] public ushort SelectionStart;
        [FieldOffset(0x4)] public ushort SelectionEnd;
        [FieldOffset(0x6)] public ushort StringLength;
    }
}
