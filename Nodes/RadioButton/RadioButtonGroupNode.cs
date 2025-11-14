using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public unsafe class RadioButtonGroupNode : SimpleComponentNode {

    private List<RadioButtonNode> radioButtons = [];

    public RadioButtonGroupNode() {
        BuildTimelines();
    }

    public ReadOnlySeString? SelectedOption {
        get => radioButtons.FirstOrDefault(button => button.IsSelected)?.SeString;
        set {
            if (value == null)
                return;

            foreach (var radioButton in radioButtons) {
                radioButton.IsChecked = radioButton.SeString == value;
                radioButton.IsSelected = radioButton.SeString == value;
            }

            RecalculateLayout();
        }
    }

    public float VerticalPadding { get; set; } = 2.0f;

    public void AddButton(ReadOnlySeString label, Action callback) {
        var newRadioButton = new RadioButtonNode {
            Height = 16.0f, 
            SeString = label, 
            Callback = callback,
        };

        newRadioButton.AddEvent(AtkEventType.ButtonClick, () => ClickHandler(newRadioButton));

        radioButtons.Add(newRadioButton);
        newRadioButton.AttachNode(this);

        if (radioButtons.Count is 1) {
            newRadioButton.IsChecked = true;
            newRadioButton.IsSelected = true;
        }

        RecalculateLayout();
    }

    public void RemoveButton(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.SeString == label);
        if (button is null) return;

        button.Dispose();
        radioButtons.Remove(button);
        RecalculateLayout();
    }

    public void Clear() {
        foreach (var node in radioButtons) {
            node.Dispose();
        }

        radioButtons.Clear();
    }

    private void RecalculateLayout() {
        var yPosition = 0.0f;

        foreach (var index in Enumerable.Range(0, radioButtons.Count)) {
            var button = radioButtons[index];

            button.Y = yPosition;
            yPosition += button.Height + VerticalPadding;
        }

        Height = yPosition;
    }

    private void ClickHandler(RadioButtonNode selectedButton) {
        foreach (var radioButton in radioButtons) {
            radioButton.IsChecked = false;
            radioButton.IsSelected = false;
        }

        selectedButton.IsChecked = true;
        selectedButton.IsSelected = true;
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 19)
            .AddLabel(1, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );
    }

    private class RadioButtonNode : ComponentNode<AtkComponentRadioButton, AtkUldComponentDataRadioButton> {
        public readonly TextNode LabelNode;
        public readonly ImageNode SelectedImageNode;

        public readonly ImageNode UnselectedImageNode;

        public RadioButtonNode() {
            SetInternalComponentType(ComponentType.RadioButton);

            UnselectedImageNode = new SimpleImageNode {
                NodeId = 4,
                TexturePath = "ui/uld/RadioButtonA.tex",
                TextureCoordinates = new Vector2(0.0f, 0.0f),
                TextureSize = new Vector2(16.0f, 16.0f),
                Size = new Vector2(16.0f, 16.0f),
                NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
                WrapMode = WrapMode.Tile,
            };
            UnselectedImageNode.AttachNode(this);

            SelectedImageNode = new SimpleImageNode {
                NodeId = 3,
                TexturePath = "ui/uld/RadioButtonA.tex",
                TextureCoordinates = new Vector2(16.0f, 0.0f),
                TextureSize = new Vector2(16.0f, 16.0f),
                Size = new Vector2(16.0f, 16.0f),
                NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
                WrapMode = WrapMode.Tile,
            };
            SelectedImageNode.AttachNode(this);

            LabelNode = new TextNode {
                NodeId = 2,
                Position = new Vector2(20.0f, 0.0f),
                Size = new Vector2(98.0f, 16.0f),
                FontSize = 14,
                TextColor = ColorHelper.GetColor(8),
                TextOutlineColor = ColorHelper.GetColor(7),
                AlignmentType = AlignmentType.Left,
                NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            };
            LabelNode.AttachNode(this);

            BuildTimelines();

            Data->Nodes[0] = LabelNode.NodeId;
            Data->Nodes[1] = UnselectedImageNode.NodeId;
            Data->Nodes[2] = 0;
            Data->Nodes[3] = 0;

            AddEvent(AtkEventType.ButtonClick, ClickHandler);

            InitializeComponentEvents();
        }

        public Action? Callback { get; set; }

        public ReadOnlySeString SeString {
            get => LabelNode.SeString;
            set {
                LabelNode.SeString = value;
                Width = LabelNode.Width + LabelNode.Position.X;
            }
        }

        public string String {
            get => LabelNode.String;
            set => LabelNode.String = value;
        }

        public bool IsChecked {
            get => Component->IsChecked;
            set => Component->SetChecked(value);
        }

        public bool IsSelected {
            get => Component->IsSelected;
            set {
                Component->IsSelected = value;
                SelectedImageNode.IsVisible = value;
            }
        }

        private void ClickHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
            Callback?.Invoke();
        }

        private void BuildTimelines() {
            AddTimeline(new TimelineBuilder()
                .BeginFrameSet(1, 9)
                .AddFrame(1, new Vector2(24, 62))
                .EndFrameSet()
                .BeginFrameSet(10, 19)
                .AddFrame(10, new Vector2(24, 44))
                .EndFrameSet()
                .Build()
            );

            CollisionNode.AddTimeline(new TimelineBuilder()
                .BeginFrameSet(1, 159)
                .AddEmptyFrame(1)
                .EndFrameSet()
                .Build()
            );

            UnselectedImageNode.AddTimeline(new TimelineBuilder()
                .BeginFrameSet(1, 9)
                .AddFrame(1, alpha: 255)
                .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(10, 19)
                .AddFrame(10, alpha: 255)
                .AddFrame(12, alpha: 255)
                .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(20, 29)
                .AddFrame(20, alpha: 255)
                .AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(30, 39)
                .AddFrame(30, alpha: 102)
                .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
                .EndFrameSet()
                .BeginFrameSet(40, 49)
                .AddFrame(40, alpha: 255)
                .AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(50, 59)
                .AddFrame(50, alpha: 255)
                .AddFrame(52, alpha: 255)
                .AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(60, 69)
                .AddFrame(60, alpha: 255)
                .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(70, 79)
                .AddFrame(70, alpha: 255)
                .AddFrame(72, alpha: 255)
                .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(72, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(80, 89)
                .AddFrame(80, alpha: 255)
                .AddFrame(80, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(90, 99)
                .AddFrame(90, alpha: 102)
                .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
                .EndFrameSet()
                .BeginFrameSet(100, 109)
                .AddFrame(100, alpha: 255)
                .AddFrame(100, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(110, 119)
                .AddFrame(110, alpha: 255)
                .AddFrame(112, alpha: 255)
                .AddFrame(110, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(120, 129)
                .AddFrame(120, alpha: 255)
                .AddFrame(120, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(130, 139)
                .AddFrame(130, alpha: 255)
                .AddFrame(130, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(140, 149)
                .AddFrame(140, alpha: 255)
                .AddFrame(140, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(150, 159)
                .AddFrame(150, alpha: 255)
                .AddFrame(150, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .Build()
            );

            SelectedImageNode.AddTimeline(new TimelineBuilder()
                .BeginFrameSet(60, 69)
                .AddFrame(60, alpha: 255)
                .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(70, 79)
                .AddFrame(70, alpha: 255)
                .AddFrame(72, alpha: 255)
                .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(72, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(80, 89)
                .AddFrame(80, alpha: 255)
                .AddFrame(80, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(90, 99)
                .AddFrame(90, alpha: 102)
                .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
                .EndFrameSet()
                .BeginFrameSet(100, 109)
                .AddFrame(100, alpha: 255)
                .AddFrame(100, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(110, 119)
                .AddFrame(110, alpha: 255)
                .AddFrame(112, alpha: 255)
                .AddFrame(110, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(120, 129)
                .AddFrame(120, alpha: 0)
                .AddFrame(122, alpha: 255)
                .AddFrame(120, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(122, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(130, 139)
                .AddFrame(130, alpha: 255)
                .AddFrame(132, alpha: 0)
                .AddFrame(130, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(132, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(140, 149)
                .AddFrame(140, alpha: 0)
                .AddFrame(142, alpha: 255)
                .AddFrame(140, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(142, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(150, 159)
                .AddFrame(150, alpha: 255)
                .AddFrame(152, alpha: 0)
                .AddFrame(150, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .AddFrame(152, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .Build()
            );

            LabelNode.AddTimeline(new TimelineBuilder()
                .BeginFrameSet(1, 9)
                .AddFrame(1, alpha: 255)
                .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(10, 19)
                .AddFrame(10, alpha: 255)
                .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(20, 29)
                .AddFrame(20, alpha: 255)
                .AddFrame(20, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(30, 39)
                .AddFrame(30, alpha: 102)
                .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
                .EndFrameSet()
                .BeginFrameSet(40, 49)
                .AddFrame(40, alpha: 255)
                .AddFrame(40, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(50, 59)
                .AddFrame(50, alpha: 255)
                .AddFrame(50, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(60, 69)
                .AddFrame(60, alpha: 255)
                .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(70, 79)
                .AddFrame(70, alpha: 255)
                .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(80, 89)
                .AddFrame(80, alpha: 255)
                .AddFrame(80, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(90, 99)
                .AddFrame(90, alpha: 102)
                .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
                .EndFrameSet()
                .BeginFrameSet(100, 109)
                .AddFrame(100, alpha: 255)
                .AddFrame(100, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(110, 119)
                .AddFrame(110, alpha: 255)
                .AddFrame(110, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(120, 129)
                .AddFrame(120, alpha: 255)
                .AddFrame(120, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(130, 139)
                .AddFrame(130, alpha: 255)
                .AddFrame(130, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(140, 149)
                .AddFrame(140, alpha: 255)
                .AddFrame(140, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .BeginFrameSet(150, 159)
                .AddFrame(150, alpha: 255)
                .AddFrame(150, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
                .EndFrameSet()
                .Build()
            );
        }
    }
}
