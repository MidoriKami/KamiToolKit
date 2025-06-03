using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUnitBaseExtensions {
	
	/// <summary>
	/// Gets a pointer to the specified node id
	/// Automatically adds base id, so instead of specifying "100,000,021", you only need to provide "21"
	/// </summary>
	/// <remarks>
	///	This will only search for nodes that are directly attached to the addon,
	/// if your desired node is attached to another component you'll need to use AtkUldManager.GetCustomNodeById on the parent component
	/// </remarks>
	/// <param name="addon">Addon's whose node list will be searched.</param>
	/// <param name="nodeId">Node ID to search for</param>
	/// <returns>Pointer to desired node, or null</returns>
	public static AtkResNode* GetCustomNodeById(ref this AtkUnitBase addon, uint nodeId)
		=> nodeId >= NodeBase.NodeIdBase ? 
			   addon.GetNodeById(nodeId) : 
			   addon.GetNodeById(nodeId + NodeBase.NodeIdBase);
}