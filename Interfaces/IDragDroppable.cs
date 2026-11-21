using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing various DragDrop actions.
/// </summary>
public interface IDragDroppable {

    /// <summary>
    /// Action that is triggered when a DragDrop is beginning
    /// </summary>
    Action<DragDropNode>? OnBegin { get; set; }

    /// <summary>
    /// Action that is triggered when a DragDrop has finished
    /// </summary>
    Action<DragDropNode>? OnEnd { get; set; }

    /// <summary>
    /// Action that is triggered when a compatible DragDrop is dropped onto this node
    /// </summary>
    Action<DragDropNode, DragDropPayload>? OnPayloadAccepted { get; set; }

    /// <summary>
    /// Action that is triggered when the item in this drag drop is being dropped onto the world
    /// </summary>
    Action<DragDropNode>? OnDiscard { get; set; }

    /// <summary>
    /// Action that is triggered when the item is clicked
    /// </summary>
    Action<DragDropNode>? OnClicked { get; set; }

    /// <summary>
    /// Action that is triggered when the item is being moused over
    /// </summary>
    Action<DragDropNode>? OnRollOver { get; set; }

    /// <summary>
    /// Action that is triggered when the item is no longer being moused over
    /// </summary>
    Action<DragDropNode>? OnRollOut { get; set; }

    /// <summary>
    /// Gets or sets whether this is draggable.
    /// </summary>
    bool IsDraggable { get; set; }

    /// <summary>
    /// Gets or sets whether this drag drop responds to clicks.
    /// </summary>
    bool IsClickable { get; set; }

    /// <summary>
    /// Gets or sets the accepted payload types for this node.
    /// </summary>
    DragDropType AcceptedType { get; set; }

    /// <summary>
    /// Gets or sets the DragDrop Payload.
    /// </summary>
    DragDropPayload Payload { get; set; }

    /// <summary>
    /// Gets or sets the sound effect suppression values for the DragDrop.
    /// </summary>
    AtkDragDropInterface.SoundEffectSuppression SoundEffectSuppression { get; set; }

    /// <summary>
    /// Clear the payload data.
    /// </summary>
    void Clear();
}
