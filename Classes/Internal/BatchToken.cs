using System;
using KamiToolKit.Nodes;

namespace KamiToolKit.Classes.Internal;

internal readonly struct BatchToken(ColorPickerNode owner) : IDisposable {
    public void Dispose() => owner.EndBatchUpdate();
}
