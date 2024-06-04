using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace KamiToolKit;

public abstract unsafe partial class NodeBase<T> where T : unmanaged, ICreatable {
    public Vector4 Margin { get; set; }
    
    public Vector4 Padding { get; set; }
}