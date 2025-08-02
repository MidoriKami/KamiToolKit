using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

    private Vector2 clickStartPosition = Vector2.Zero;
    private NodeEditMode currentEditMode = 0;

    private ViewportEventListener? editEventListener;

    private bool isCursorSet;

    private bool isMoving;
    private bool isResizing;

    private NodeEditOverlayNode? overlayNode;

    public Action? OnResizeComplete { get; set; }
    public Action? OnMoveComplete { get; set; }
    public Action? OnEditComplete { get; set; }

    public bool EnableMoving {
        get;
        set {
            field = value;
            if (value) {
                EnableEditMode(NodeEditMode.Move);
            }
            else {
                DisableEditMode(NodeEditMode.Move);
            }
        }
    }

    public bool EnableResizing {
        get;
        set {
            field = value;
            if (value) {
                EnableEditMode(NodeEditMode.Resize);
            }
            else {
                DisableEditMode(NodeEditMode.Resize);
            }
        }
    }

    public void EnableEditMode(NodeEditMode mode) {

        currentEditMode |= mode;

        if (overlayNode is null) {
            overlayNode = new NodeEditOverlayNode {
                Position = new Vector2(-16.0f, -16.0f), Size = Size + new Vector2(32.0f, 32.0f), IsVisible = true,
            };
            overlayNode.AttachNode(this);
        }

        overlayNode.ShowParts = currentEditMode.HasFlag(NodeEditMode.Resize);

        if (editEventListener is null) {
            editEventListener = new ViewportEventListener(OnEditEvent);
            editEventListener.AddEvent(AtkEventType.MouseMove, overlayNode.InternalResNode);
            editEventListener.AddEvent(AtkEventType.MouseDown, overlayNode.InternalResNode);
        }
    }

    public void DisableEditMode(NodeEditMode mode) {

        currentEditMode &= ~mode;

        if (currentEditMode.HasFlag(NodeEditMode.Resize) || currentEditMode.HasFlag(NodeEditMode.Move)) return;

        if (editEventListener is not null) {
            editEventListener.RemoveEvent(AtkEventType.MouseMove);
            editEventListener.RemoveEvent(AtkEventType.MouseDown);
            editEventListener.Dispose();
            editEventListener = null;
        }

        if (overlayNode is not null) {
            overlayNode.DetachNode();
            overlayNode.Dispose();
            overlayNode = null;
        }

    }

    private void OnEditEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (overlayNode is null) return;
        if (editEventListener is null) return;

        ref var mouseData = ref atkEventData->MouseData;
        var mousePosition = new Vector2(mouseData.PosX, mouseData.PosY);
        var mouseDelta = mousePosition - clickStartPosition;

        switch (eventType) {
            // Move Logic
            case AtkEventType.MouseMove when isMoving: {
                Position += mouseDelta;
                clickStartPosition = mousePosition;

                atkEvent->SetEventIsHandled(true);
            }
                break;

            // Update hover state when not resizing, as we latch that for the behavior
            case AtkEventType.MouseMove when !isResizing: {
                overlayNode.UpdateHover(atkEventData);
            }
                break;

            // Resize Logic
            case AtkEventType.MouseMove when isResizing: {
                Position += overlayNode.GetPositionDelta(mouseDelta);
                Size += overlayNode.GetSizeDelta(mouseDelta);

                overlayNode.Size = Size + new Vector2(32.0f, 32.0f);

                clickStartPosition = mousePosition;

                atkEvent->SetEventIsHandled(true);
            }
                break;

            // Begin Resize Event
            case AtkEventType.MouseDown when !isResizing && overlayNode.AnyHovered() && currentEditMode.HasFlag(NodeEditMode.Resize): {
                editEventListener.AddEvent(AtkEventType.MouseUp, overlayNode.InternalResNode);

                isResizing = true;
                clickStartPosition = mousePosition;

                atkEvent->SetEventIsHandled(true);
            }
                break;

            // End Resize Event
            case AtkEventType.MouseUp when isResizing: {
                OnResizeComplete?.Invoke();
                OnEditComplete?.Invoke();

                isResizing = false;
                editEventListener.RemoveEvent(AtkEventType.MouseUp);
            }
                break;

            // Begin Move Event
            case AtkEventType.MouseDown when !overlayNode.AnyHovered() && overlayNode.CheckCollision(atkEventData) && !isMoving && currentEditMode.HasFlag(NodeEditMode.Move): {
                editEventListener.AddEvent(AtkEventType.MouseUp, overlayNode.InternalResNode);

                isMoving = true;
                clickStartPosition = mousePosition;

                atkEvent->SetEventIsHandled(true);
            }
                break;

            // End Move Event
            case AtkEventType.MouseUp when isMoving: {
                OnMoveComplete?.Invoke();
                OnEditComplete?.Invoke();

                isMoving = false;
                editEventListener.RemoveEvent(AtkEventType.MouseUp);
            }
                break;
        }

        if (isCursorSet) {
            ResetCursor();
            isCursorSet = false;
        }

        if (currentEditMode.HasFlag(NodeEditMode.Move)) {
            if (isMoving) {
                SetCursor(AddonCursorType.Grab);
                isCursorSet = true;
            }
            else if (CheckCollision(atkEventData)) {
                SetCursor(AddonCursorType.Hand);
                isCursorSet = true;
            }
        }

        if (overlayNode.AnyHovered() && currentEditMode.HasFlag(NodeEditMode.Resize)) {
            overlayNode.SetCursor();
            isCursorSet = true;
        }
    }
}
