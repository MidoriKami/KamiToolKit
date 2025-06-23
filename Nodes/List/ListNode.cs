using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes;

public abstract class ListNode : ComponentNode<AtkComponentBase, AtkUldComponentDataBase>;

/// Note, automatically inserts buttons to fill the set height, please ensure option count is greater than button count.
public abstract unsafe class ListNode<T> : ListNode {

	public readonly NineGridNode BackgroundNode;
	public readonly ResNode ContainerNode;
	public List<ListButtonNode> Nodes = [];
	public readonly ScrollBarNode ScrollBarNode;

	public T? SelectedOption { get; set; } 

	public List<T>? Options {
		get; set { 
			field = value;
			RebuildNodeList();
		}
	}

	protected ListNode() {
		SetInternalComponentType(ComponentType.Base);

		BackgroundNode = new SimpleNineGridNode {
			TexturePath = "ui/uld/ListB.tex",
			TextureCoordinates = new Vector2(0.0f, 0.0f),
			TextureSize = new Vector2(32.0f, 32.0f),
			TopOffset = 10, BottomOffset = 12, LeftOffset = 10, RightOffset = 10,
			IsVisible = true,
		};
		
		BackgroundNode.AttachNode(this);

		ContainerNode = new ResNode {
			NodeFlags = NodeFlags.Clip,
			IsVisible = true,
		};
		
		ContainerNode.AttachNode(this);

		ScrollBarNode = new ScrollBarNode {
			Position = new Vector2(0.0f, 9.0f),
			Size = new Vector2(8.0f, 0.0f),
			IsVisible = true,
			OnValueChanged = OnScrollUpdate,
		};
		
		ScrollBarNode.AttachNode(this);
		
		BuildTimelines();
		
		ContainerNode.SetEventFlags();
		ContainerNode.AddEvent(AddonEventType.MouseWheel, OnMouseWheel);
	}

	protected float NodeHeight { get; set; } = 22.0f;
	
	public override float Height {
		get => base.Height;
		set {
			var adjustedSize = GetNodeCount(value) * NodeHeight + 24.0f;
			
			base.Height = adjustedSize;
			BackgroundNode.Height = adjustedSize;
			ContainerNode.Height = adjustedSize;
			ScrollBarNode.Height = adjustedSize - 23.0f;

			if (Options is not null && Options.Count != 0) {
				RebuildNodeList();
			}
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			BackgroundNode.Width = value;
			ContainerNode.Width = value - 25.0f;
			foreach (var buttonNode in Nodes) {
				buttonNode.Width = value - 25.0f;
			}
			ScrollBarNode.X = value - 17.0f;
		}
	}
	
	private void OnScrollUpdate(int scrollPosition) {
		var index = scrollPosition / 22.0f;
		
		CurrentStartIndex = (int) index;
		UpdateNodes();
	}
	
	private void OnMouseWheel(AddonEventData data) {
		CurrentStartIndex -= data.GetMouseData().WheelDirection;
		UpdateNodes();
		ScrollBarNode.ScrollPosition = (int) ( CurrentStartIndex * NodeHeight + 9.0f );
				
		data.SetHandled();
	}
	
	public int CurrentStartIndex { get; set; }

	private void RebuildNodeList() {
		var buttonCount = GetNodeCount(Height);

		foreach (var button in Nodes) {
			button.Dispose();
		}
		Nodes.Clear();

		foreach (var index in Enumerable.Range(0, buttonCount)) {
			var newButton = new ListButtonNode {
				NodeId = (uint) index,
				Size = new Vector2(Width - 25.0f, NodeHeight),
				Position = new Vector2(8.0f, NodeHeight * index + 9.0f),
				IsVisible = true,
				Label = $"Button {index}",
				OnClick = () => OnOptionClick(index),
			};
			
			Nodes.Add(newButton);
			newButton.AttachNode(ContainerNode);
		}

		if (Options is not null) {
			ScrollBarNode.UpdateScrollParams((int) ScrollBarNode.Height, (int) ( Options.Count * NodeHeight + 24.0f ) );
		}

		UpdateNodes();
	}

	protected virtual void OnOptionClick(int nodeId) {
		if (Options is null) return;
		
		SelectedOption = Options[nodeId + CurrentStartIndex];
		OnOptionSelected?.Invoke(Options[nodeId + CurrentStartIndex]);
		
		UpdateSelected();
	}

	private void UpdateSelected() {
		if (Options is null) return;
		
		foreach (var index in Enumerable.Range(0, Nodes.Count)) {
			var option = Options[index + CurrentStartIndex];

			Nodes[index].Selected = SelectedOption?.Equals(option) ?? false;
			Nodes[index].Label = GetLabelForOption(option);
		}
	}
	
	protected abstract string GetLabelForOption(T option);
	
	protected void UpdateNodes() {
		if (Options is null) return;
		var maxStartIndex = Options.Count - Nodes.Count;
		
		var max = Math.Max(0, maxStartIndex);
		CurrentStartIndex = Math.Clamp(CurrentStartIndex, 0, max);
		UpdateSelected();
	}

	public void SelectDefaultOption() {
		if (Options is not null) {
			SelectedOption = Options.First();
		}
	}

	public Action<T>? OnOptionSelected { get; set; }

	public void Show() {
		IsVisible = true;
		DrawFlags = 0x200000;
	}

	public void Hide() {
		IsVisible = false;
		DrawFlags = 0x100;
	}

	public void Toggle(bool newState) {
		if (newState) {
			Show();
		}
		else {
			Hide();
		}
	}
	
	protected int GetNodeCount(float height)
		=> (int) ( ( height - 18.0f ) / NodeHeight );
	
	private void BuildTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 29)
			.AddLabel(1, 17, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(10, 18, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
			.AddLabel(20, 7, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
			.EndFrameSet()
			.Build()
		);
	}
}