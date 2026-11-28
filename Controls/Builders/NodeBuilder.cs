using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.UI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.Components;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Text.ReadOnly;


namespace KamiToolKit.Controls.Builders;

/// <summary>
/// Generic builder class for constructing UI node trees backed by a specific <see cref="ILayoutNode"/> type.
/// Inherits from <see cref="NodeBuilder"/> so it can be passed to <c>Action{NodeBuilder}</c> callbacks.
/// </summary>
/// <typeparam name="T">The type of layout node this builder creates.</typeparam>
public class NodeBuilder<T> : NodeBuilder where T : ILayoutNode, new() {

    /// <summary>
    /// The owned layout node, created by the constructor.
    /// </summary>
    private readonly T layoutNode;

    /// <summary>
    /// Gets the layout node that children are added to.
    /// </summary>
    protected override ILayoutNode LayoutNode => layoutNode;

    /// <summary>
    /// Constructor for a NodeBuilder that builds into a new layout node of type <typeparamref name="T"/>.
    /// </summary>
    public NodeBuilder() {
        layoutNode = new T();
        if (layoutNode is VerticalListNode verticalList) {
            verticalList.StretchWidth = true;
        }
    }

    /// <summary>
    /// Constructor for a NodeBuilder that adds children directly to an existing target node.
    /// </summary>
    /// <param name="targetLayoutNode">The node that children will be added to instead of creating a new one.</param>
    internal NodeBuilder(T targetLayoutNode) {
        layoutNode = targetLayoutNode;
    }

    /// <summary>
    /// Builds the resulting node of type <typeparamref name="T"/>.
    /// </summary>
    public new T Build() => layoutNode;
}

/// <summary>
/// Abstract base class for constructing UI node trees.
/// All builder methods are implemented here; derived classes provide the concrete layout node.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public abstract class NodeBuilder {

    /// <summary>
    /// Gets the layout node that children are added to.
    /// </summary>
    protected abstract ILayoutNode LayoutNode { get; }

    /// <summary>
    /// Builds the resulting layout node.
    /// </summary>
    public virtual ILayoutNode Build() => LayoutNode;

    /// <summary>
    /// Adds a predefined node to this builder.
    /// </summary>
    public NodeBuilder AddNode(NodeBase node) {
        LayoutNode.AddNode(node);
        return this;
    }

    /// <summary>
    /// Chains building via externally provided action.
    /// </summary>
    public NodeBuilder BuildAction(Action<NodeBuilder> buildAction) {
        buildAction(this);
        return this;
    }

    /// <summary>
    /// Adds a spacer with the specified size.
    /// </summary>
    public NodeBuilder Spacer(float size)
        => AddNode(new ResNode { Size = new Vector2(size, size) });

    /// <summary>
    /// Adds a HorizontalLineNode.
    /// </summary>
    public NodeBuilder Separator()
        => AddNode(new HorizontalLineNode { Height = 4.0f });

    /// <summary>
    /// Adds a <see cref="CategoryTextNode"/> with label text.
    /// </summary>
    public NodeBuilder CategoryLabel(ReadOnlySeString label, out CategoryTextNode node, Action<ICategoryTextNode>? configure = null) {
        node = new CategoryTextNode {
            Size = new Vector2(100.0f, 16.0f),
            String = label,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="CategoryTextNode"/> with label text.
    /// </summary>
    public NodeBuilder CategoryLabel(ReadOnlySeString label, Action<ICategoryTextNode>? configure = null)
        => CategoryLabel(label, out _, configure);

    /// <summary>
    /// Adds a <see cref="TextNode"/> with label text.
    /// </summary>
    public NodeBuilder Text(ReadOnlySeString label, out TextNode node, Action<ITextNode>? configure = null) {
        node = new TextNode {
            Size = new Vector2(100.0f, 24.0f),
            String = label,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextNode"/> with label text.
    /// </summary>
    public NodeBuilder Text(ReadOnlySeString label, Action<ITextNode>? configure = null)
        => Text(label, out _, configure);

    /// <summary>
    /// Adds a centered <see cref="TextNode"/> with label text.
    /// </summary>
    public NodeBuilder TextCentered(ReadOnlySeString label, out TextNode node, Action<ITextNode>? configure = null)
        => Text(label, out node, text => {
            text.AlignmentType = AlignmentType.Center;
            configure?.Invoke(text);
        });

    /// <summary>
    /// Adds a centered <see cref="TextNode"/> with label text.
    /// </summary>
    public NodeBuilder TextCentered(ReadOnlySeString label, Action<ITextNode>? configure = null)
        => TextCentered(label, out _, configure);

    /// <summary>
    /// Adds a centered <see cref="TextNode"/> with label text and a specified font size.
    /// </summary>
    public NodeBuilder TextCentered(ReadOnlySeString label, uint fontSize, out TextNode node, Action<ITextNode>? configure = null)
        => Text(label, out node, text => {
            text.AlignmentType = AlignmentType.Center;
            text.FontSize = fontSize;
            configure?.Invoke(text);
        });

    /// <summary>
    /// Adds a centered <see cref="TextNode"/> with label text and a specified font size.
    /// </summary>
    public NodeBuilder TextCentered(ReadOnlySeString label, uint fontSize, Action<ITextNode>? configure = null)
        => TextCentered(label, fontSize, out _, configure);

    /// <summary>
    /// Adds a <see cref="TextNode"/> with label text.
    /// With params to wordwrap.
    /// </summary>
    public NodeBuilder TextWrapped(ReadOnlySeString label, int lineCount, out TextNode node, Action<ITextNode>? configure = null) {
        node = new TextNode {
            Size = new Vector2(100.0f, 16.0f * lineCount),
            TextFlags = TextFlags.WordWrap | TextFlags.MultiLine,
            LineSpacing = 14,
            String = label,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextNode"/> with label text.
    /// With params to wordwrap.
    /// </summary>
    public NodeBuilder TextWrapped(ReadOnlySeString label, int lineCount, Action<ITextNode>? configure = null)
        => TextWrapped(label, lineCount, out _, configure);

    /// <summary>
    /// Adds a <see cref="TextNode"/> that will fill the current content area.
    /// </summary>
    public NodeBuilder TextFilled(ReadOnlySeString label, out TextNode node, Action<ITextNode>? configure = null) {
        node = new TextNode {
            Size = new Vector2(100.0f, 24.0f),
            String = label,
            LayoutFill = LayoutFill.Both,
            AlignmentType = AlignmentType.Center,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextNode"/> that will fill the current content area.
    /// </summary>
    public NodeBuilder TextFilled(ReadOnlySeString label, Action<ITextNode>? configure = null)
        => Text(label, out _, configure);

    /// <summary>
    /// Adds a warning-styled <see cref="TextNode"/> that will fill the current content area.
    /// </summary>
    public NodeBuilder TextWarningFilled(ReadOnlySeString label, out TextNode node, Action<ITextNode>? configure = null)
        => TextFilled(label, out node, text => {
            text.FontSize = 14;
            text.TextColor = ColorHelper.GetColor(32);
            configure?.Invoke(text);
        });

    /// <summary>
    /// Adds a warning-styled <see cref="TextNode"/> that will fill the current content area.
    /// </summary>
    public NodeBuilder TextWarningFilled(ReadOnlySeString label, Action<ITextNode>? configure = null)
        => TextWarningFilled(label, out _, configure);

    /// <summary>
    /// Adds a <see cref="CheckboxNode"/>.
    /// </summary>
    public NodeBuilder Checkbox(ReadOnlySeString label, bool value, Action<bool> onChecked, out CheckboxNode node, Action<ICheckboxNode>? configure = null) {
        node = new CheckboxNode {
            Size = new Vector2(100.0f, 24.0f),
            String = label,
            IsChecked = value,
            OnChecked = onChecked,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="CheckboxNode"/>.
    /// </summary>
    public NodeBuilder Checkbox(ReadOnlySeString label, bool value, Action<bool> onChecked, Action<ICheckboxNode>? configure = null)
        => Checkbox(label, value, onChecked, out _, configure);

    /// <summary>
    /// Adds a <see cref="TextButtonNode"/>.
    /// </summary>
    public NodeBuilder TextButton(ReadOnlySeString label, Action onClicked, out TextButtonNode node, Action<ITextButtonNode>? configure = null) {
        node = new TextButtonNode {
            Size = new Vector2(100.0f, 24.0f),
            String = label,
            OnClick = onClicked,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextButtonNode"/>.
    /// </summary>
    public NodeBuilder TextButton(ReadOnlySeString label, Action onClicked, Action<ITextButtonNode>? configure = null)
        => TextButton(label, onClicked, out _, configure);

    /// <summary>
    /// Adds a <see cref="CircleButtonNode"/>.
    /// </summary>
    public NodeBuilder CircleButton(CircleButtonIcon icon, Action onClicked, out CircleButtonNode node, Action<ICircleButtonNode>? configure = null) {
        node = new CircleButtonNode {
            Size = new Vector2(32.0f, 32.0f),
            Icon = icon,
            OnClick = onClicked,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="CircleButtonNode"/>.
    /// </summary>
    public NodeBuilder CircleButton(CircleButtonIcon icon, Action onClicked, Action<ICircleButtonNode>? configure = null)
        => CircleButton(icon, onClicked, out _, configure);

    /// <summary>
    /// Adds a <see cref="IconButtonNode"/>.
    /// </summary>
    public NodeBuilder IconButton(uint iconId, Action onClicked, out IconButtonNode node, Action<IIconButtonNode>? configure = null) {
        node = new IconButtonNode {
            Size = new Vector2(32.0f, 32.0f),
            IconId = iconId,
            OnClick = onClicked,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="IconButtonNode"/>.
    /// </summary>
    public NodeBuilder IconButton(uint iconId, Action onClicked, Action<IIconButtonNode>? configure = null)
        => IconButton(iconId, onClicked, out _, configure);

    /// <summary>
    /// Adds a <see cref="ImGuiIconButtonNode"/>.
    /// </summary>
    public NodeBuilder ImGuiButton(string filePath, Action onClicked, out ImGuiIconButtonNode node, Action<IImGuiIconButtonNode>? configure = null) {
        node = new ImGuiIconButtonNode {
            Size = new Vector2(32.0f, 32.0f),
            TexturePath = filePath,
            OnClick = onClicked,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="ImGuiIconButtonNode"/>.
    /// </summary>
    public NodeBuilder ImGuiButton(string filePath, Action onClicked, Action<IImGuiIconButtonNode>? configure = null)
        => ImGuiButton(filePath, onClicked, out _, configure);

    /// <summary>
    /// Adds a <see cref="ImGuiIconButtonNode"/>.
    /// </summary>
    public NodeBuilder ImGuiButton(IDalamudTextureWrap texture, Action onClicked, out ImGuiIconButtonNode node, Action<IImGuiIconButtonNode>? configure = null) {
        node = new ImGuiIconButtonNode {
            Size = new Vector2(32.0f, 32.0f),
            OnClick = onClicked,
        };

        node.LoadTexture(texture);

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="ImGuiIconButtonNode"/>.
    /// </summary>
    public NodeBuilder ImGuiButton(IDalamudTextureWrap texture, Action onClicked, Action<IImGuiIconButtonNode>? configure = null)
        => ImGuiButton(texture, onClicked, out _, configure);

    /// <summary>
    /// Adds a <see cref="TextureButtonNode"/>.
    /// </summary>
    public NodeBuilder TextureButton(string texturePath, Vector2 textureCoordinates, Vector2 partSize, Action onClicked, out TextureButtonNode node, Action<ITextureButtonNode>? configure = null) {
        node = new TextureButtonNode {
            Size = new Vector2(32.0f, 32.0f),
            TexturePath = texturePath,
            TextureCoordinates = textureCoordinates,
            TextureSize = partSize,
            OnClick = onClicked,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextureButtonNode"/>.
    /// </summary>
    public NodeBuilder TextureButton(string texturePath, Vector2 textureCoordinates, Vector2 partSize, Action onClicked, Action<ITextureButtonNode>? configure = null)
        => TextureButton(texturePath, textureCoordinates, partSize, onClicked, out _, configure);

    /// <summary>
    /// Adds a <see cref="DropDownNode{TV}"/>.
    /// </summary>
    public NodeBuilder Combo<TU>(List<TU> options, TU initialValue, Action<TU> onOptionSelected, Func<TU, ReadOnlySeString> labelDefinition, out DropDownNode<TU> node, Action<IDropDownNode<TU>>? configure = null) {
        node = new DropDownNode<TU> {
            Size = new Vector2(100.0f, 24.0f),
            GetLabelFunction = labelDefinition,
            OnOptionSelected = onOptionSelected,
            MaxListOptions = options.Count > 15 ? 15 : 5,
            Options = options,
            SelectedOption = initialValue,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="DropDownNode{T}"/>.
    /// </summary>
    public NodeBuilder Combo<TU>(List<TU> options, TU initialValue, Action<TU> onOptionSelected, Func<TU, ReadOnlySeString> labelDefinition, Action<IDropDownNode<TU>>? configure = null)
        => Combo(options, initialValue, onOptionSelected, labelDefinition, out _, configure);

    /// <summary>
    /// Adds a <see cref="DropDownNode{T}"/> predefined as an enum DropDownNode.
    /// </summary>
    public NodeBuilder Combo<TU>(TU initialValue, Action<TU> onOptionSelected, out DropDownNode<TU> node, Action<IDropDownNode<TU>>? configure = null) where TU : struct, Enum
        => Combo([.. Enum.GetValues<TU>()], initialValue, onOptionSelected, value => value.ToString(), out node, configure);

    /// <summary>
    /// Adds a <see cref="DropDownNode{T}"/> predefined as an enum DropDownNode.
    /// </summary>
    public NodeBuilder Combo<TU>(TU initialValue, Action<TU> onOptionSelected, Action<IDropDownNode<TU>>? configure = null) where TU : struct, Enum
        => Combo(initialValue, onOptionSelected, out _, configure);

    /// <summary>
    /// Adds a <see cref="DropDownNode{T}"/> predefined as a string DropDownNode.
    /// </summary>
    public NodeBuilder Combo(List<ReadOnlySeString> options, ReadOnlySeString initialValue, Action<ReadOnlySeString> onOptionSelected, out DropDownNode<ReadOnlySeString> builtNode, Action<IDropDownNode<ReadOnlySeString>>? configure = null)
        => Combo(options, initialValue, onOptionSelected, value => value, out builtNode, configure);

    /// <summary>
    /// Adds a <see cref="DropDownNode{T}"/> predefined as a string DropDownNode.
    /// </summary>
    public NodeBuilder Combo(List<ReadOnlySeString> options, ReadOnlySeString initialValue, Action<ReadOnlySeString> onOptionSelected, Func<ReadOnlySeString, ReadOnlySeString> labelDefinition, Action<IDropDownNode<ReadOnlySeString>>? configure = null)
        => Combo(options, initialValue, onOptionSelected, labelDefinition, out _, configure);

    /// <summary>
    /// Adds a <see cref="NumericInputNode"/>.
    /// </summary>
    public NodeBuilder InputInt(int value, int min, int max, Action<int> onValueUpdate, out NumericInputNode node, Action<INumericInputNode>? configure = null) {
        node = new NumericInputNode {
            Size = new Vector2(100.0f, 24.0f),
            Range = min..max,
            Value = value,
            OnValueUpdate = onValueUpdate,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="NumericInputNode"/>.
    /// </summary>
    public NodeBuilder InputInt(int value, int min, int max, Action<int> onValueUpdate, Action<INumericInputNode>? configure = null)
        =>  InputInt(value, min, max, onValueUpdate, out _, configure);

    /// <summary>
    /// Adds a <see cref="SliderNode"/>.
    /// </summary>
    public NodeBuilder IntSlider(int value, int min, int max, Action<int> onValueUpdate, out SliderNode node, Action<ISliderNode<int>>? configure = null) {
        node = new SliderNode {
            Size = new Vector2(100.0f, 24.0f),
            Min = min,
            Max = max,
            Value = value,
            OnValueUpdate = onValueUpdate,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="SliderNode"/>.
    /// </summary>
    public NodeBuilder IntSlider(int value, int min, int max, Action<int> onValueUpdate, Action<ISliderNode<int>>? configure = null)
        =>  IntSlider(value, min, max, onValueUpdate, out _, configure);

    /// <summary>
    /// Adds a <see cref="FloatSliderNode"/>.
    /// </summary>
    public NodeBuilder FloatSlider(float value, float min, float max, Action<float> onValueUpdate, out FloatSliderNode node, Action<ISliderNode<float>>? configure = null) {
        node = new FloatSliderNode {
            Size = new Vector2(100.0f, 24.0f),
            Min = min,
            Max = max,
            Value = value,
            OnValueUpdate = onValueUpdate,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="FloatSliderNode"/>.
    /// </summary>
    public NodeBuilder FloatSlider(float value, float min, float max, Action<float> onValueUpdate, Action<ISliderNode<float>>? configure = null)
        =>  FloatSlider(value, min, max, onValueUpdate, out _, configure);

    /// <summary>
    /// Adds a <see cref="TextInputNode"/>.
    /// </summary>
    public NodeBuilder InputText(Action<ReadOnlySeString> onInputReceived, out TextInputNode node, ReadOnlySeString? initialValue = null, Action<ITextInputNode>? configure = null) {
        node = new TextInputNode {
            Size = new Vector2(100.0f, 28.0f),
            String = initialValue ?? string.Empty,
            OnInputReceived = onInputReceived,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextInputNode"/>.
    /// </summary>
    public NodeBuilder InputText(Action<ReadOnlySeString> onInputReceived, ReadOnlySeString? initialValue = null, Action<ITextInputNode>? configure = null)
        => InputText(onInputReceived, out _, initialValue, configure);

    /// <summary>
    /// Adds a <see cref="TextInputNode"/> with defaults for search.
    /// </summary>
    public NodeBuilder Search(Action<ReadOnlySeString> onInputReceived, out SearchInputNode node, ReadOnlySeString? initialValue = null, Action<ITextInputNode>? configure = null) {
        node = new SearchInputNode {
            Size = new Vector2(100.0f, 28.0f),
            String = initialValue ?? string.Empty,
            OnInputReceived = onInputReceived,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TextInputNode"/> with defaults for search.
    /// </summary>
    public NodeBuilder Search(Action<ReadOnlySeString> onInputReceived, ReadOnlySeString? initialValue = null, Action<ITextInputNode>? configure = null)
        => Search(onInputReceived, out _, initialValue, configure);

    /// <summary>
    /// Adds a virtualized list via <see cref="ListNode{T,TU}"/>.
    /// </summary>
    public NodeBuilder List<TU, TV>(List<TU> options, Action<TU> onOptionSelected, out ListNode<TU, TV> node, Action<IListNode<TU, TV>>? configure = null) where TV : ListItemNode<TU>, IListItemNode, new() {
        node = new ListNode<TU, TV> {
            LayoutFill = LayoutFill.Vertical,
            Options = options,
            OnOptionSelected = onOptionSelected,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a virtualized list via <see cref="ListNode{T,TU}"/>.
    /// </summary>
    public NodeBuilder List<TU, TV>(List<TU> options, Action<TU?> onItemSelected, Action<IListNode<TU, TV>>? configure = null) where TV : ListItemNode<TU>, IListItemNode, new()
        => List(options, onItemSelected, out _, configure);

    /// <summary>
    /// Adds a virtualized list via <see cref="ListNode{T,TU}"/>.
    /// </summary>
    /// <remarks>
    /// This version is without a <see cref="ListNode{T,TU}.OnOptionSelected"/> callback, so it assumes you want the options to not be selectable.
    /// </remarks>
    public NodeBuilder List<TU, TV>(List<TU> options, out ListNode<TU, TV> node, Action<IListNode<TU, TV>>? configure = null) where TV : ListItemNode<TU>, IListItemNode, new() {
        node = new ListNode<TU, TV> {
            LayoutFill = LayoutFill.Vertical,
            Options = options,
            EnableSelection = false,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a virtualized list via <see cref="ListNode{T,TU}"/>.
    /// </summary>
    /// <remarks>
    /// This version is without a <see cref="ListNode{T,TU}.OnOptionSelected"/> callback, so it assumes you want the options to not be selectable.
    /// </remarks>
    public NodeBuilder List<TU, TV>(List<TU> options, Action<IListNode<TU, TV>>? configure = null) where TV : ListItemNode<TU>, IListItemNode, new()
        => List(options, out _, configure);

    /// <summary>
    /// Adds a virtualized list via <see cref="ListNode{T,TU}"/>.
    /// </summary>
    /// <remarks>
    /// This overload provides a callback for when the node itself reports that its state has changed vis ItemStateChanged(...);
    /// </remarks>
    public NodeBuilder List<TU, TV>(List<TU> options, Action<TV, TU>? onItemStateChanged, out ListNode<TU, TV> node, Action<IListNode<TU, TV>>? configure = null) where TV : ListItemNode<TU>, IListItemNode, new() {
        node = new ListNode<TU, TV> {
            LayoutFill = LayoutFill.Vertical,
            Options = options,
            OnItemStateChanged = onItemStateChanged,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a virtualized list via <see cref="ListNode{T,TU}"/>.
    /// </summary>
    /// <remarks>
    /// This overload provides a callback for when the node itself reports that its state has changed vis ItemStateChanged(...);
    /// </remarks>
    public NodeBuilder List<TU, TV>(List<TU> options, Action<TV, TU>? onItemStateChanged, Action<IListNode<TU, TV>>? configure = null) where TV : ListItemNode<TU>, IListItemNode, new()
        => List(options, onItemStateChanged, out _, configure);

    /// <summary>
    /// Adds a <see cref="ScrollingNode{T}"/> with a backed VerticalListNode.
    /// </summary>
    /// <remarks>
    /// Warning, if you need to display a lot of elements use a <see cref="ListNode{T,TU}"/> instead.
    /// </remarks>
    public NodeBuilder Scrolling(Action<NodeBuilder> buildInner, out ScrollingNode<VerticalListNode> node, Action<IScrollingNode<VerticalListNode>>? configure = null) {
        var innerBuilder = new NodeBuilder<VerticalListNode>();
        buildInner(innerBuilder);

        var verticalListNode = innerBuilder.Build();
        verticalListNode.FitContents = true;

        node = new ScrollingNode<VerticalListNode>(verticalListNode) {
            AutoHideScrollBar = true,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="ScrollingNode{T}"/> with a backed VerticalListNode.
    /// </summary>
    /// <remarks>
    /// Warning, if you need to display a lot of elements use a <see cref="ListNode{T,TU}"/> instead.
    /// </remarks>
    public NodeBuilder Scrolling(Action<NodeBuilder> buildInner, Action<IScrollingNode<VerticalListNode>>? configure = null)
        => Scrolling(buildInner, out _, configure);

    /// <summary>
    /// Adds a <see cref="RatioSplitNode"/> for creating segmented areas.
    /// </summary>
    public NodeBuilder RatioSplit(float[] ratios, out RatioSplitNode node, Action<IRatioSplitNode>? configure = null, params Action<NodeBuilder>[] sectionBuildActions) {
        node = new RatioSplitNode {
            Size = new Vector2(100.0f, 24.0f),
            Ratios = ratios,
            ItemSpacing = 2.0f,
        };

        foreach (var buildAction in sectionBuildActions) {
            var innerBuilder = new NodeBuilder<VerticalListNode>();
            buildAction(innerBuilder);
            node.AddNode(innerBuilder.Build());
        }

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="RatioSplitNode"/> for creating segmented areas.
    /// </summary>
    public NodeBuilder RatioSplit(float[] ratios, Action<IRatioSplitNode>? configure = null, params Action<NodeBuilder>[] sectionBuildActions)
        => RatioSplit(ratios, out _, configure, sectionBuildActions);

    /// <summary>
    /// Adds a <see cref="RatioSplitNode"/> for creating segmented areas.
    /// </summary>
    public NodeBuilder RatioSplit(float[] ratios, params Action<NodeBuilder>[] sectionBuildActions)
        => RatioSplit(ratios, out _, null, sectionBuildActions);

    /// <summary>
    /// Adds a two-string display node for displaying a label and an updatable value.
    /// </summary>
    public NodeBuilder DataRow(ReadOnlySeString label, ReadOnlySeString initialValue, out TextNode valueNode, float[]? ratios = null, Action<IRatioSplitNode>? configureSplit = null) {
        TextNode localValueNode = null!;

        RatioSplit(ratios ?? [0.5f, 0.5f],
            configureRatio => {
                configureRatio.ShowSeparators = false;
                configureSplit?.Invoke(configureRatio);
            },
            leftSplit => leftSplit.Text(label, textNode => {
                textNode.AddTextFlags(TextFlags.Ellipsis);
            }),
            rightSplit => rightSplit.Text(initialValue, out localValueNode, textNode => {
                textNode.AddTextFlags(TextFlags.Ellipsis);
            }));

        valueNode = localValueNode;
        return this;
    }

    /// <summary>
    /// Adds a standardized data row with a left label and a right value node.
    /// </summary>
    /// <remarks>
    /// This overload does not include an out TextNode for updating the value text, this probably isn't what you wanted, unless the data doesn't change.
    /// </remarks>
    public NodeBuilder DataRow(ReadOnlySeString label, ReadOnlySeString initialValue, float[]? ratios = null, Action<IRatioSplitNode>? configureSplit = null)
        => DataRow(label, initialValue, out _, ratios, configureSplit);

    /// <summary>
    /// Adds a standardized split row with a left text label and a custom right-hand side component that is built with NodeBuilder.
    /// </summary>
    public NodeBuilder Row(ReadOnlySeString label, Action<NodeBuilder> buildRight, float[]? ratios = null, Action<IRatioSplitNode>? configureSplit = null) {
        RatioSplit(ratios ?? [0.5f, 0.5f],
            configureRatio => {
                configureRatio.ShowSeparators = false;
                configureSplit?.Invoke(configureRatio);
            },
            leftSplit => leftSplit.Text(label, configureLabel => {
                configureLabel.AddTextFlags(TextFlags.Ellipsis);
            }),
            buildRight);

        return this;
    }

    /// <summary>
    /// Adds a <see cref="TabBarNode"/>.
    /// </summary>
    /// <remarks>
    /// Builds a standard tab bar, this does not include content nodes, if you need content areas as well use TabbedContent.
    /// </remarks>
    public NodeBuilder TabBar(Action<ITabBarNode> configure, out TabBarNode node) {
        node = new TabBarNode {
            Size = new Vector2(100.0f, 24.0f),
        };

        configure.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TabBarNode"/> with specified tab initialization actions.
    /// </summary>
    /// <remarks>
    /// Builds a standard tab bar, this does not include content nodes, if you need content areas as well use TabbedContent.
    /// </remarks>
    public NodeBuilder TabBar(Action<ITabBarNode> configure)
        => TabBar(configure, out _);

    /// <summary>
    /// Adds a <see cref="TabbedContentNode"/>, a tab bar with associated content areas.
    /// </summary>
    public NodeBuilder TabbedContent(Action<TabbedContentBuilder> buildTabs, out TabbedContentNode node, Action<ITabbedContentNode>? configure = null, Action? tabChanged = null) {
        node = new TabbedContentNode {
            Size = new Vector2(100.0f, 200.0f),
            OnTabChanged = tabChanged,
        };

        var contentBuilder = new TabbedContentBuilder(node);
        buildTabs(contentBuilder);

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="TabbedContentNode"/>, a tab bar with associated content areas.
    /// </summary>
    public NodeBuilder TabbedContent(Action<TabbedContentBuilder> buildTabs, Action<ITabbedContentNode>? configure = null, Action? tabChanged = null)
        => TabbedContent(buildTabs, out _, configure, tabChanged);

    /// <summary>
    /// Adds a layout node that will fit all its children to itself.
    /// </summary>
    /// <remarks>
    /// Intended to be used for cases where you want multiple nodes that may or may not be visible at any given time.
    /// </remarks>
    public NodeBuilder FittedContent(Action<NodeBuilder> buildInner, out FittedContentNode node, Action<ILayoutListNode>? configure = null) {
        var innerBuilder = new NodeBuilder<FittedContentNode>();
        buildInner(innerBuilder);

        node = innerBuilder.Build();
        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a layout node that will fit all its children to itself.
    /// </summary>
    /// <remarks>
    /// Intended to be used for cases where you want multiple nodes that may or may not be visible at any given time.
    /// </remarks>
    public NodeBuilder FittedContent(Action<NodeBuilder> buildInner, Action<ILayoutListNode>? configure = null)
        => FittedContent(buildInner, out _, configure);

    /// <summary>
    /// Adds a nested <see cref="VerticalListNode"/> container.
    /// </summary>
    public NodeBuilder VerticalList(Action<NodeBuilder> buildInner, out VerticalListNode node, Action<IVerticalListNode>? configure = null) {
        var innerBuilder = new NodeBuilder<VerticalListNode>();
        buildInner(innerBuilder);

        node = innerBuilder.Build();

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a nested <see cref="VerticalListNode"/> container.
    /// </summary>
    public NodeBuilder VerticalList(Action<NodeBuilder> buildInner, Action<IVerticalListNode>? configure = null)
        => VerticalList(buildInner, out _, configure);

    /// <summary>
    /// Adds a nested <see cref="HorizontalListNode"/> container.
    /// </summary>
    public NodeBuilder HorizontalList(Action<NodeBuilder> buildInner, out HorizontalListNode node, Action<IHorizontalListNode>? configure = null) {
        var innerBuilder = new NodeBuilder<HorizontalListNode>();
        buildInner(innerBuilder);

        node = innerBuilder.Build();
        node.FitHeight = true;

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a nested <see cref="HorizontalListNode"/> container.
    /// </summary>
    public NodeBuilder HorizontalList(Action<NodeBuilder> buildInner, Action<IHorizontalListNode>? configure = null)
        => HorizontalList(buildInner, out _, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color.
    /// </summary>
    public NodeBuilder ColorEdit4(ReadOnlySeString label, Vector4 value, Vector4? defaultValue, Action<Vector4> onColorConfirmed, out ColorEditNode node, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null) {
        node = new ColorEditNode {
            Size = new Vector2(100.0f, 24.0f),
            String = label,
            CurrentColor = value,
            DefaultColor = defaultValue,
            OnColorConfirmed = onColorConfirmed,
            OnColorPreviewed = onColorPreviewed,
            OnColorCancelled = onColorCancelled,
        };

        configure?.Invoke(node);

        return AddNode(node);
    }

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color.
    /// </summary>
    public NodeBuilder ColorEdit4(ReadOnlySeString label, Vector4 value, Vector4? defaultValue, Action<Vector4> onColorConfirmed, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(label, value, defaultValue, onColorConfirmed, out _, onColorPreviewed, onColorCancelled, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color with a KnownColor default.
    /// </summary>
    public NodeBuilder ColorEdit4(ReadOnlySeString label, Vector4 value, KnownColor defaultColor, Action<Vector4> onColorConfirmed, out ColorEditNode node, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(label, value, defaultColor.Vector(), onColorConfirmed, out node, onColorPreviewed, onColorCancelled, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color with a KnownColor default.
    /// </summary>
    public NodeBuilder ColorEdit4(ReadOnlySeString label, Vector4 value, KnownColor defaultColor, Action<Vector4> onColorConfirmed, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(label, value, defaultColor, onColorConfirmed, out _, onColorPreviewed, onColorCancelled, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color without a label.
    /// </summary>
    public NodeBuilder ColorEdit4(Vector4 value, Vector4? defaultValue, Action<Vector4> onColorConfirmed, out ColorEditNode node, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(string.Empty, value, defaultValue, onColorConfirmed, out node, onColorPreviewed, onColorCancelled, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color without a label.
    /// </summary>
    public NodeBuilder ColorEdit4(Vector4 value, Vector4? defaultValue, Action<Vector4> onColorConfirmed, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(value, defaultValue, onColorConfirmed, out _, onColorPreviewed, onColorCancelled, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color with a KnownColor default, without a label.
    /// </summary>
    public NodeBuilder ColorEdit4(Vector4 value, KnownColor defaultColor, Action<Vector4> onColorConfirmed, out ColorEditNode node, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(value, defaultColor.Vector(), onColorConfirmed, out node, onColorPreviewed, onColorCancelled, configure);

    /// <summary>
    /// Adds a <see cref="ColorEditNode"/> for picking a Vector4 color with a KnownColor default, without a label.
    /// </summary>
    public NodeBuilder ColorEdit4(Vector4 value, KnownColor defaultColor, Action<Vector4> onColorConfirmed, Action<Vector4>? onColorPreviewed = null, Action? onColorCancelled = null, Action<IColorEditNode>? configure = null)
        => ColorEdit4(value, defaultColor, onColorConfirmed, out _, onColorPreviewed, onColorCancelled, configure);
}
