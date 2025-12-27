using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;


/// <summary>
///     Needs More Work.
/// </summary>
internal unsafe class TextMultiLineInputNodeScrollable : TextInputNode {

    private int startLineIndex;

    private bool isProgrammaticTextSet;
    
    private ReadOnlySeString fullText;
    private ReadOnlySeString lastDisplayedText;

    public TextMultiLineInputNodeScrollable() {
        TextLimitsNode.AlignmentType = AlignmentType.BottomRight;

        CurrentTextNode.TextFlags |= TextFlags.MultiLine;
        CurrentTextNode.LineSpacing = 14;

        Flags |= TextInputFlags.MultiLine;

        CollisionNode.AddEvent(AtkEventType.InputReceived, InputComplete);
        CollisionNode.AddEvent(AtkEventType.MouseWheel, OnMouseScrolled);

        Component->InputSanitizationFlags = AllowedEntities.UppercaseLetters | AllowedEntities.LowercaseLetters | AllowedEntities.Numbers | 
                                            AllowedEntities.SpecialCharacters | AllowedEntities.CharacterList | AllowedEntities.OtherCharacters |
                                            AllowedEntities.Payloads | AllowedEntities.Unknown9;

        Component->ComponentTextData.Flags2 = TextInputFlags2.MultiLine | TextInputFlags2.AllowSymbolInput | TextInputFlags2.AllowNumberInput;

        Component->ComponentTextData.MaxLine = byte.MaxValue;
        Component->ComponentTextData.MaxByte = ushort.MaxValue;
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
        get => fullText.ToString();
        set {
            isProgrammaticTextSet = true;
            fullText = value;
            UpdateCurrentTextDisplay();
            isProgrammaticTextSet = false;
        }
    }

    public override ReadOnlySeString SeString {
        get => fullText;
        set {
            isProgrammaticTextSet = true;
            fullText = value;
            UpdateCurrentTextDisplay();
            isProgrammaticTextSet = false;
        }
    }
    
    public override Action<ReadOnlySeString>? OnInputReceived {
        get => base.OnInputReceived;
        set {
            base.OnInputReceived = currentComponentText => {
                if (isProgrammaticTextSet) return;

                ApplyDisplayChangesToFullText(currentComponentText.ToString());
                lastDisplayedText = currentComponentText;
                UpdateLineCountDisplay();
            };

            base.OnInputReceived += value;
        }
    }
    
    private void OnMouseScrolled(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        var lines = fullText.ToString().Split(['\r', '\n'], StringSplitOptions.None);
        var lineHeight = CurrentTextNode.LineSpacing;
        var maxVisibleLines = (int)(Height / lineHeight);

        var oldStartLineIndex = startLineIndex;

        if (atkEventData->IsScrollUp)
            startLineIndex = Math.Max(0, startLineIndex - 1);

        else if (atkEventData->IsScrollDown)
            startLineIndex = Math.Min(Math.Max(0, lines.Length - maxVisibleLines), startLineIndex + 1);

        if (oldStartLineIndex != startLineIndex) {
            UpdateCurrentTextDisplay();
        }

        atkEvent->SetEventIsHandled();
    }

    private void ApplyDisplayChangesToFullText(string newDisplayedText) {
        var lines = fullText.ToString().Split(['\r', '\n'], StringSplitOptions.None).ToList();
        var oldDisplayLines = lastDisplayedText.ToString().Split(['\r', '\n'], StringSplitOptions.None);
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
        var lines = fullText.ToString().Split(['\r', '\n'], StringSplitOptions.None);
        var lineHeight = CurrentTextNode.LineSpacing;
        var totalLines = lines.Length;
        var maxVisibleLines = (int)(Height / lineHeight);

        if (maxVisibleLines <= 0) return;

        startLineIndex = Math.Clamp(startLineIndex, 0, Math.Max(0, totalLines - maxVisibleLines));

        var currentEndLine = Math.Min(startLineIndex + maxVisibleLines, totalLines);
        var limitText = $"{startLineIndex + 1}-{currentEndLine}/{totalLines}";

        TextLimitsNode.SeString = limitText;
    }

    private void UpdateCurrentTextDisplay() {
        var lines = fullText.ToString().Split(['\r', '\n'], StringSplitOptions.None);
        var lineHeight = CurrentTextNode.LineSpacing;
        var maxVisibleLines = (int)(Height / lineHeight);

        if (maxVisibleLines <= 0) return;

        startLineIndex = Math.Clamp(startLineIndex, 0, Math.Max(0, lines.Length - maxVisibleLines));

        var displayText = startLineIndex > 0 && startLineIndex < lines.Length
            ? string.Join("\r", lines.Skip(startLineIndex).Take(maxVisibleLines))
            : fullText.ToString();

        lastDisplayedText = displayText;
        var capturedProgrammaticFlag = isProgrammaticTextSet;

        isProgrammaticTextSet = capturedProgrammaticFlag;
        Component->SetText(displayText);
        UpdateLineCountDisplay();
    }

    private void InputComplete() {
        if (UIInputData.Instance()->IsKeyPressed(SeVirtualKey.RETURN)) {
            var textInputComponent = Node->GetAsAtkComponentTextInput();
            var cursorPos = textInputComponent->CursorPos;

            using (var utf8String = new Utf8String()) {
                utf8String.SetString("\r");
                textInputComponent->WriteString(&utf8String);
            }

            textInputComponent->CursorPos = cursorPos + 1;
            textInputComponent->SelectionStart = cursorPos + 1;
            textInputComponent->SelectionEnd = cursorPos + 1;
        }

        OnInputComplete?.Invoke(Component->EvaluatedString.AsSpan());
    }
}
