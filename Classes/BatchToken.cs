using System;
using KamiToolKit.Premade.Node.Color;

namespace KamiToolKit.Classes;

internal readonly struct BatchToken(ColorPickerWidget owner) : IDisposable {
    public void Dispose() => owner.EndBatchUpdate();
}
