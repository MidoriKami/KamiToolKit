using System;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class TextMultiLineInputNode : TextInputNode {

    private float originalHeight;

    public TextMultiLineInputNode() : base() {
        TextLimitsNode.AlignmentType = AlignmentType.BottomRight;

        CurrentTextNode.TextFlags |= TextFlags.MultiLine;
        CurrentTextNode.LineSpacing = 14;

        Flags |= TextInputFlags.MultiLine;

        OnInputReceived += _ => UpdateHeightForContent();

        CollisionNode.AddEvent(AddonEventType.InputReceived, InputComplete);

        Component->InputSanitizationFlags = (AllowedEntities)639;
        Component->ComponentTextData.Flags2 = (TextInputFlags2)11;
        Component->ComponentTextData.MaxLine = 255;
        Component->ComponentTextData.MaxByte = 65535;
    }

    public uint MaxLines {
        get => Component->ComponentTextData.MaxLine;
        set => Component->ComponentTextData.MaxLine = value;
    }

    public uint MaxBytes {
        get => Component->ComponentTextData.MaxByte;
        set => Component->ComponentTextData.MaxByte = value;
    }

    public override SeString SeString {
        get => base.SeString;
        set {
            base.SeString = value;
            PlaceholderTextNode.IsVisible = PlaceholderString is not null && value.ToString().IsNullOrEmpty();
            UpdateHeightForContent();
        }
    }

    public override string String {
        get => base.String;
        set {
            base.String = value;
            PlaceholderTextNode.IsVisible = PlaceholderString is not null && value.IsNullOrEmpty() && !FocusNode.IsVisible;
            UpdateHeightForContent();
        }
    }

    private void UpdateHeightForContent() {
        var text = String;
        var lineCount = Math.Max(1, text.Split('\r', '\n').Length);
        var lineHeight = CurrentTextNode.LineSpacing;
        var contentHeight = Math.Max(originalHeight, (lineCount * lineHeight) + 20);
        Height = contentHeight;
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
    }
}
