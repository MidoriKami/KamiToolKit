using System;
using KamiToolKit.Premade.Widgets;

namespace KamiToolKit.Classes;

internal readonly struct BatchToken(ColorPickerWidget owner) : IDisposable {
    public void Dispose() => owner.EndBatchUpdate();
}
