using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives;

namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Interface representing a node that can be attached to a parent node or addon.
/// </summary>
public unsafe interface IAttachableNode {

    /// <summary>
    /// Attaches this node to the target node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(NodeBase? targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target layout node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(ILayoutNode? targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to target addon's root node using targetPosition as the relative positioning.
    /// </summary>
    void AttachNode(NativeAddon? targetAddon, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native addon using targetPosition as the relative positioning.
    /// </summary>
    void AttachNode(AtkUnitBase* targetAddon, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native res node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkResNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native image node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkImageNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native text node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkTextNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native nine-grid node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkNineGridNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native counter node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkCounterNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native collision node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkCollisionNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native clipping mask node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkClippingMaskNode* targetNode, NodePosition targetPosition = NodePosition.AsLastChild);

    /// <summary>
    /// Attaches this node to the target native component node using targetPosition to determine where to insert the node relatively.
    /// </summary>
    void AttachNode(AtkComponentNode* targetNode, NodePosition targetPosition = NodePosition.AfterAllSiblings);
}
