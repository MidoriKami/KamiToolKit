using System;
using System.Collections.Generic;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="TextButtonNode"/> that has multiple label states.
/// </summary>
public class MultiStateButtonNode<T> : TextButtonNode, IMultiStateButtonNode<T> where T : notnull {

    /// <inheritdoc />
    public Action<T>? OnStateChanged { get; set; }

    /// <inheritdoc />
    public required List<T> States {
        get;
        set {
            field = value;
            UpdateDisplay();
        }
    }

    /// <inheritdoc />
    public T SelectedState {
        get => States[SelectedIndex];
        set => SelectedIndex = States.IndexOf(value);
    }

    /// <summary>
    /// Constructs a new <see cref="MultiStateButtonNode{T}"/>.
    /// </summary>
    public MultiStateButtonNode()
        => OnClick = CycleState;

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

    private string GetStateText(T state) {
        if (state is Enum enumState) {
            return enumState.Description;
        }

        return state.ToString() ?? "Unable to Parse Type";
    }

    private int SelectedIndex {
        get;
        set {
            field = value;
            UpdateDisplay();
        }
    }
}
