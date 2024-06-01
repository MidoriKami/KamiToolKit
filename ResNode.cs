using System;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public unsafe class ResNode : NodeBase<AtkResNode>, IDisposable {
    protected override sealed AtkResNode* InternalNode { get; set; }
    public override NodeType NodeType => NodeType.Res;

    public ResNode() {
        InternalNode = IMemorySpace.GetUISpace()->Create<AtkResNode>();

        if (InternalNode is null) {
            throw new Exception("Unable to create memory for AtkResNode");
        }
    }

    public void Dispose() {
        IMemorySpace.Free(InternalNode);
    }
}