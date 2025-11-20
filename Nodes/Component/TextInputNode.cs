using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.Parts;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public unsafe class TextInputNode : ComponentNode<AtkComponentTextInput, AtkUldComponentDataTextInput> {

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode CurrentTextNode;
    public readonly CursorNode CursorNode;
    public readonly NineGridNode FocusNode;
    public readonly TextInputSelectionListNode SelectionListNode;
    public readonly TextNode TextLimitsNode;
    public readonly TextNode PlaceholderTextNode;

    private AtkComponentInputBase.CallbackDelegate? pinnedCallbackFunction;

    public Action? OnFocused;
    public Action? OnUnfocused;

    public TextInputNode() {
        SetInternalComponentType(ComponentType.TextInput);

        BackgroundNode = new SimpleNineGridNode {
            NodeId = 19,
            TexturePath = "ui/uld/TextInputA.tex",
            TextureCoordinates = new Vector2(24.0f, 0.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
            Offsets = new Vector4(10.0f),
            Size = new Vector2(152.0f, 28.0f),
        };
        BackgroundNode.AttachNode(this);

        FocusNode = new SimpleNineGridNode {
            NodeId = 18,
            TexturePath = "ui/uld/TextInputA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
            Offsets = new Vector4(10.0f),
            Size = new Vector2(152.0f, 28.0f),
        };
        FocusNode.AttachNode(this);

        TextLimitsNode = new TextNode {
            NodeId = 17,
            Position = new Vector2(-24.0f, 6.0f),
            Size = new Vector2(170.0f, 19.0f),
            FontType = FontType.MiedingerMed,
            FontSize = 14,
            AlignmentType = (AlignmentType)21,
        };
        TextLimitsNode.AttachNode(this);

        CurrentTextNode = new TextNode {
            NodeId = 16,
            Position = new Vector2(10.0f, 6.0f),
            Size = new Vector2(132.0f, 18.0f),
            AlignmentType = AlignmentType.TopLeft,
            TextFlags = TextFlags.AutoAdjustNodeSize,
            TextColor = ColorHelper.GetColor(1),
        };
        CurrentTextNode.AttachNode(this);

        SelectionListNode = new TextInputSelectionListNode {
            NodeId = 4, 
            Position = new Vector2(0.0f, 22.0f), 
            Size = new Vector2(186.0f, 208.0f), 
        };
        SelectionListNode.AttachNode(this);

        CursorNode = new CursorNode {
            NodeId = 2,
            Position = new Vector2(10.0f, 2.0f),
            Size = new Vector2(4.0f, 24.0f),
            OriginY = 4.0f,
        };
        CursorNode.AttachNode(this);

        PlaceholderTextNode = new TextNode {
            Position = new Vector2(10.0f, 6.0f),
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
                TextInputFlags.EnableDictionary | TextInputFlags.AllowNumberInput | TextInputFlags.AllowSymbolInput;

        EnableCompletion = false;
        Component->EnableTabCallback = true;

        LoadTimelines();

        pinnedCallbackFunction = OnCallback;
        Component->Callback = (delegate* unmanaged<AtkUnitBase*, InputCallbackType, CStringPointer, CStringPointer, int, InputCallbackResult>) Marshal.GetFunctionPointerForDelegate(pinnedCallbackFunction);

        InitializeComponentEvents();

        CollisionNode.AddEvent(AtkEventType.FocusStart, () => {
            PlaceholderTextNode.IsVisible = false;
            OnFocused?.Invoke();

            if (AutoSelectAll && Component->EvaluatedString.Length > 0) {
                DalamudInterface.Instance.Framework.RunOnTick(() => {
                    // Approach #1: Invoke the same function that is called when you press Control + A

                    var keyModifiers = new Experimental.KeyModifiers {
                        IsControlDown = true,
                    };

                    Experimental.Instance.ProcessKeyShortcutFunction?.Invoke(AtkStage.Instance()->AtkInputManager->TextInput, SeVirtualKey.A, &keyModifiers);  
                    
                    // Approach #2: Invoke the SetTextSelection function, this will crash as-is, needs additional data to be set to work correctly.

                    // var selectionInfo = stackalloc AtkTextInput.TextSelectionInfo[1];
                    // selectionInfo->SelectionStart = 0;
                    // selectionInfo->SelectionEnd = (ushort) Component->EvaluatedString.Length;
                    // selectionInfo->StringLength = (ushort) Component->EvaluatedString.Length;
                    //
                    // Component->UpdateTextSelection(selectionInfo);
                    
                    // Approach #3: Set the selects directly, I don't think this will work, as the selection seems to need to be set in multiple systems to work.
                    
                    // ref var textInput = ref AtkStage.Instance()->AtkInputManager->TextInput;
                    //
                    // textInput->SelectionStart = 0;
                    // textInput->SelectionEnd = (short)Component->EvaluatedString.Length;
                }, delayTicks: 1);
            }
        });
        
        CollisionNode.AddEvent(AtkEventType.FocusStop, () => {
            OnUnfocused?.Invoke();
            if (!PlaceholderString.IsNullOrEmpty() && String.IsNullOrEmpty()) {
                PlaceholderTextNode.IsVisible = true;
                PlaceholderTextNode.String = PlaceholderString;
            }
        });
    }

    public bool IsFocused => AtkStage.Instance()->AtkInputManager->FocusedNode == CollisionNode.Node;

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

    public bool EnableCompletion {
        get => Component->EnableCompletion;
        set => Component->EnableCompletion = value;
    }

    public bool EnableFocusSounds {
        get => Component->EnableFocusSounds;
        set => Component->EnableFocusSounds = value;
    }

    public virtual ReadOnlySeString SeString {
        get => Component->EvaluatedString.AsSpan();
        set => Component->SetText(value);
    }

    public virtual string String {
        get => SeString.ToString();
        set => Component->SetText(value);
    }

    public string? PlaceholderString {
        get;
        set {
            field = value;
            PlaceholderTextNode.String = value ?? string.Empty;
        }
    }

    public bool IsError {
        get => FocusNode.MultiplyColor == new Vector3(1.0f, 0.6f, 0.6f);
        set => FocusNode.MultiplyColor = value ? new Vector3(1.0f, 0.6f, 0.6f) : Vector3.One;
    }

    public bool AutoSelectAll { get; set; } = true;

    public void ClearFocus() {
        if (IsFocused) {
            AtkStage.Instance()->AtkInputManager->SetFocus(null, ParentAddon, 0);
        }
    }

    public virtual Action<ReadOnlySeString>? OnInputReceived { get; set; }
    public virtual Action<ReadOnlySeString>? OnInputComplete { get; set; }
    public Action? OnFocusLost { get; set; }
    public Action? OnEscapeEntered { get; set; }
    public Action? OnTabEntered { get; set; }

    private InputCallbackResult OnCallback(AtkUnitBase* addon, InputCallbackType type, CStringPointer rawString, CStringPointer evaluatedString, int eventKind) {
        switch (type) {
            case InputCallbackType.Enter:
                OnInputComplete?.Invoke(Component->EvaluatedString.AsSpan());
                ClearFocus();
                break;

            case InputCallbackType.TextChanged:
                OnInputReceived?.Invoke(Component->EvaluatedString.AsSpan());
                break;

            case InputCallbackType.Escape:
                OnEscapeEntered?.Invoke();
                break;

            case InputCallbackType.FocusLost:
                OnFocusLost?.Invoke();
                break;

            case InputCallbackType.Tab:
                OnTabEntered?.Invoke();
                break;
        }

        return InputCallbackResult.None;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

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
}
