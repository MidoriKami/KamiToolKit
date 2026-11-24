namespace KamiToolKit.Core.Interfaces.Nodes;

/// <summary>
/// Interface representing a SliderNode.
/// </summary>
public interface ISliderNode<T> : IBoundedRange<T>, IValueChangeable<T>, INode;
