using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUldManagerExtensions {
	
	/// <summary>
	/// Gets a pointer to the specified node id
	/// Automatically adds base id, so instead of specifying "100,000,021", you only need to provide "21"
	/// </summary>
	/// <param name="manager">AtkUldManager to search</param>
	/// <param name="nodeId">Node ID to search for</param>
	/// <returns>Pointer to desired node, or null</returns>
	public static AtkResNode* GetCustomNodeById(ref this AtkUldManager manager, uint nodeId)
		=> nodeId >= NodeBase.NodeIdBase ? 
			   manager.SearchNodeById(nodeId) : 
			   manager.SearchNodeById(nodeId + NodeBase.NodeIdBase);
}