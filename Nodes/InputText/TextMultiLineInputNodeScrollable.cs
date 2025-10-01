using System;
using System.Linq;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class TextMultiLineInputNodeScrollable : TextInputNode {

    private float originalHeight;
    private int startLineIndex;

    private bool isInternallyUpdatingDisplay;
    private bool isProgrammaticTextSet;
    
    private SeString fullText = string.Empty;
    private SeString lastDisplayedText = string.Empty;

    public TextMultiLineInputNodeScrollable() {
        TextLimitsNode.AlignmentType = AlignmentType.BottomRight;

        CurrentTextNode.TextFlags |= TextFlags.MultiLine;
        CurrentTextNode.LineSpacing = 14;

        Flags |= TextInputFlags.MultiLine;

        OnInputReceived += _ => {
            if (isProgrammaticTextSet || isInternallyUpdatingDisplay) return;

            var currentComponentText = Component->UnkText1.ToString();
            ApplyDisplayChangesToFullText(currentComponentText);
            lastDisplayedText = currentComponentText;
            UpdateLineCountDisplay();
        };

        CollisionNode.AddEvent(AddonEventType.InputReceived, InputComplete);

        AddEvent(AddonEventType.MouseWheel, evt => {
            var mouse = evt.GetMouseData();

            var lines = fullText.TextValue.Split(['\r', '\n'], StringSplitOptions.None);
            var lineHeight = CurrentTextNode.LineSpacing;
            var maxVisibleLines = (int)(originalHeight / lineHeight);

            var oldStartLineIndex = startLineIndex;

            if (mouse.WheelDirection > 0)
                startLineIndex = Math.Max(0, startLineIndex - 1);
            else if (mouse.WheelDirection < 0)
                startLineIndex = Math.Min(Math.Max(0, lines.Length - maxVisibleLines), startLineIndex + 1);

            if (oldStartLineIndex != startLineIndex) {
                UpdateCurrentTextDisplay();
                evt.SetHandled();
            }
        });

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

    public override string String {
        get => fullText.TextValue;
        set {
            isProgrammaticTextSet = true;
            fullText = value;
            UpdateCurrentTextDisplay();
            isProgrammaticTextSet = false;
        }
    }

    public override SeString SeString {
        get => fullText;
        set {
            isProgrammaticTextSet = true;
            fullText = value;
            UpdateCurrentTextDisplay();
            isProgrammaticTextSet = false;
        }
    }

    private void ApplyDisplayChangesToFullText(string newDisplayedText) {
        var lines = fullText.TextValue.Split(['\r', '\n'], StringSplitOptions.None).ToList();
        var oldDisplayLines = lastDisplayedText.TextValue.Split(['\r', '\n'], StringSplitOptions.None);
        var newDisplayLines = newDisplayedText.Split(['\r', '\n'], StringSplitOptions.None);

        if (startLineIndex < lines.Count) {
            var removeCount = Math.Min(oldDisplayLines.Length, lines.Count - startLineIndex);
            lines.RemoveRange(startLineIndex, removeCount);

            lines.InsertRange(startLineIndex, newDisplayLines);
        }
        else {
            lines.AddRange(newDisplayLines);
        }

        for (var i = lines.Count - 1; i >= 0; i--) {
            if (string.IsNullOrEmpty(lines[i]))
                lines.RemoveAt(i);
            else
                break;
        }

        if (lines.Count == 0)
            lines.Add(string.Empty);

        fullText = string.Join("\r", lines);
        lastDisplayedText = newDisplayedText;
    }

    private void UpdateLineCountDisplay() {
        var lines = fullText.TextValue.Split(['\r', '\n'], StringSplitOptions.None);
        var lineHeight = CurrentTextNode.LineSpacing;
        var totalLines = lines.Length;
        var maxVisibleLines = (int)(originalHeight / lineHeight);

        if (maxVisibleLines <= 0) return;

        startLineIndex = Math.Clamp(startLineIndex, 0, Math.Max(0, totalLines - maxVisibleLines));

        var currentEndLine = Math.Min(startLineIndex + maxVisibleLines, totalLines);
        var limitText = $"{startLineIndex + 1}-{currentEndLine}/{totalLines}";

        TextLimitsNode.SeString = limitText;
    }

    private void UpdateCurrentTextDisplay() {
        if (isInternallyUpdatingDisplay) return;

        var lines = fullText.TextValue.Split(['\r', '\n'], StringSplitOptions.None);
        var lineHeight = CurrentTextNode.LineSpacing;
        var maxVisibleLines = (int)(originalHeight / lineHeight);

        if (maxVisibleLines <= 0) return;

        startLineIndex = Math.Clamp(startLineIndex, 0, Math.Max(0, lines.Length - maxVisibleLines));

        var displayText = startLineIndex > 0 && startLineIndex < lines.Length
            ? string.Join("\r", lines.Skip(startLineIndex).Take(maxVisibleLines))
            : fullText.TextValue;

        lastDisplayedText = displayText;
        var capturedProgrammaticFlag = isProgrammaticTextSet;

        isInternallyUpdatingDisplay = true;
        isProgrammaticTextSet = capturedProgrammaticFlag;
        Component->SetText(displayText);
        UpdateLineCountDisplay();
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
