﻿using System;
using Lumina.Excel;

namespace KamiToolKit.Nodes;

public class LuminaDropDownNode<T> : DropDownNode<LuminaListNode<T>, T> where T : struct, IExcelRow<T> {

    public LuminaDropDownNode() {
        OptionListNode.OnOptionSelected += OptionSelectedHandler;
    }

    public Action<T>? OnOptionSelected { get; set; }

    public LuminaListNode<T>.GetLabel? LabelFunction {
        get => OptionListNode.LabelFunction;
        set {
            OptionListNode.LabelFunction = value;
            ResolveOptions();
        }
    }

    public LuminaListNode<T>.ShouldShow? FilterFunction {
        get => OptionListNode.FilterFunction;
        set {
            OptionListNode.FilterFunction = value;
            ResolveOptions();
        }
    }

    private void OptionSelectedHandler(T option) {
        OnOptionSelected?.Invoke(option);
        UpdateLabel(option);
        Toggle();
    }

    private void ResolveOptions() {
        if (LabelFunction is null) return;
        if (FilterFunction is null) return;

        OptionListNode.SelectDefaultOption();
        LabelNode.String = LabelFunction.Invoke(OptionListNode.SelectedOption);
    }

    protected override void UpdateLabel(T option) {
        LabelNode.String = LabelFunction?.Invoke(option) ?? "错误：未提供标签生成函数";
    }
}
