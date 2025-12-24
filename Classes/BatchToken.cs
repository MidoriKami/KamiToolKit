using System;
using KamiToolKit.Premade.Widgets;

namespace KamiToolKit.Classes;

internal readonly struct BatchToken : IDisposable {
    private readonly ColorPickerWidget owner;
    public BatchToken(ColorPickerWidget owner) => this.owner = owner;
    public void Dispose() => owner.EndBatchUpdate();
}
