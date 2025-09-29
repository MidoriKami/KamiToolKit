using System;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;


namespace KamiToolKit.Nodes;

public unsafe class TextMultiLineInputNode : TextInputNode {

    private float originalHeight;

    public TextMultiLineInputNode() : base()
    {
        TextLimitsNode.AlignmentType = AlignmentType.BottomRight;

        CurrentTextNode.TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.MultiLine;
        CurrentTextNode.LineSpacing = 14;

        Flags = TextInputFlags.EnableIme | TextInputFlags.AllowUpperCase | TextInputFlags.AllowLowerCase |
                TextInputFlags.EnableDictionary | TextInputFlags.AllowNumberInput | TextInputFlags.AllowSymbolInput |
                TextInputFlags.MultiLine;

        Component->InputSanitizationFlags = (AllowedEntities)639;
        Component->ComponentTextData.Flags2 = (TextInputFlags2)11;
        Component->ComponentTextData.MaxLine = 64;
        Component->ComponentTextData.MaxByte = 65535;

        CollisionNode.AddEvent(AddonEventType.InputReceived, InputComplete);
    }

    public override SeString SeString {
        get => base.SeString;
        set {
            base.SeString = value;
            UpdateHeightForContent();
        }
    }

    public override string String {
        get => base.String;
        set {
            base.String = value;
            UpdateHeightForContent();
        }
    }

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

    protected override void OnCursorChanged(AtkTextInput.AtkTextInputEventInterface* listener, TextSelectionInfo* numEvents) {
        base.OnCursorChanged(listener, numEvents);
        UpdateHeightForContent();
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
