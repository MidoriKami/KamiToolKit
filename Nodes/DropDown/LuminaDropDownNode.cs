using System;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

public class LuminaDropDownNode<T> : DropDownNode<LuminaListNode<T>, T> where T : struct, IExcelRow<T> {

	public LuminaDropDownNode() {
		OptionListNode.OnOptionSelected += OptionSelectedHandler;
	}

	public Action<T>? OnOptionSelected { get; set; }
	
	private void OptionSelectedHandler(T option) {
		OnOptionSelected?.Invoke(option);
		LabelNode.Text = LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Set";
		Toggle();
	}

	public Func<T, string>? LabelFunction {
		get => OptionListNode.LabelFunction;
		set {
			OptionListNode.LabelFunction = value;
			ResolveOptions();
		}
	}

	public Func<T, bool>? FilterFunction {
		get => OptionListNode.FilterFunction;
		set {
			OptionListNode.FilterFunction = value;
			ResolveOptions();
		}
	}

	private void ResolveOptions() {
		if (LabelFunction is null) return;
		if (FilterFunction is null) return;
		
		OptionListNode.SelectDefaultOption();
		LabelNode.Text = LabelFunction.Invoke(OptionListNode.SelectedOption);
	}
}