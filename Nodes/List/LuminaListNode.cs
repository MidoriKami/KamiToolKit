using System;
using System.Linq;
using KamiToolKit.Classes;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

public class LuminaListNode<T> : ListNode<T> where T : struct, IExcelRow<T> {

	private Func<T, string>? InternalLabelResolver { get; set; }
	
	public required Func<T, string>? LabelFunction {
		get => InternalLabelResolver;
		set {
			InternalLabelResolver = value;
			ResolveOptions();
		}
	}

	private Func<T, bool>? InternalFilterFunction { get; set; }

	public required Func<T, bool>? FilterFunction {
		get => InternalFilterFunction;
		set {
			InternalFilterFunction = value;
			ResolveOptions();
		}
	}

	private void ResolveOptions() {
		if (LabelFunction is null) return;
		if (FilterFunction is null) return;
		
		Options = DalamudInterface.Instance.DataManager.GetExcelSheet<T>()
			.Where(FilterFunction)
			.ToList();
	}

	protected override string GetLabelForOption(T option)
		=> LabelFunction?.Invoke(option) ?? "ERROR: Label Function Not Found";
}