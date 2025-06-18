using System;
using System.Collections.Generic;

namespace KamiToolKit.Nodes;

public class TextDropDownNode : DropDownNode<TextListNode> {

	public TextDropDownNode() {
		OptionListNode.OnOptionSelected += OptionSelectedHandler;
	}

	public Action<string>? OnOptionSelected { get; set; }
	
	private void OptionSelectedHandler(string option) {
		OnOptionSelected?.Invoke(option);
		LabelNode.Text = option;
		Toggle();
	}

	public required List<string>? Options {
		get => OptionListNode.Options;
		set {
			OptionListNode.Options = value;
			OptionListNode.SelectDefaultOption();
			LabelNode.Text = OptionListNode.SelectedOption ?? "ERROR: Invalid Default Option";
		}
	}
}