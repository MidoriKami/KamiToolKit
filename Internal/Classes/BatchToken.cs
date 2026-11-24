using System;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Internal.Classes;

internal readonly struct BatchToken(ColorPickerNode owner) : IDisposable {
    public void Dispose() => owner.EndBatchUpdate();
}
