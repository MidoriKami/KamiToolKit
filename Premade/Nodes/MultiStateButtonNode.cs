using System;
using System.Collections.Generic;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Nodes;

/// <summary>
/// A TextButton that has a configurable set of states
/// </summary>
public class MultiStateButtonNode<T> : TextButtonNode where T : notnull {
    public Action<T>? OnStateChanged { get; set; }

    public MultiStateButtonNode()
        => OnClick = CycleState;

    public required List<T> States {
        get;
        set {
            field = value;
            UpdateDisplay();
        }
    }

    private int SelectedIndex {
        get;
        set {
            field = value;
            UpdateDisplay();
        }
    }
    
    public T SelectedState {
        get => States[SelectedIndex];
        set => SelectedIndex = States.IndexOf(value);
    }

    private void CycleState() {
        if (States.Count is 0) return;

        SelectedIndex = (SelectedIndex + 1) % States.Count;
        OnStateChanged?.Invoke(SelectedState);
    }

    private void UpdateDisplay() {
        if (SelectedIndex < 0) return;
        if (SelectedIndex > States.Count - 1) return;

        String = GetStateText(States[SelectedIndex]);
    }

    protected virtual string GetStateText(T state) {
        if (state is Enum enumState) {
            return enumState.Description;
        }
        
        return state.ToString() ?? "Unable to Parse Type";
    }
}
