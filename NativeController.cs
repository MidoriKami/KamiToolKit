using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using Serilog.Events;

namespace KamiToolKit;

/// <summary>
///     Controller for custom native nodes, this class is required to attach custom nodes to native ui, this service will
///     also keep track of the allocated nodes to prevent memory leaks.
/// </summary>
public unsafe class NativeController : IDisposable {

    internal static readonly ConcurrentDictionary<Type, List<MemberInfo>> ChildMembers = [];
    internal static readonly ConcurrentDictionary<Type, List<MemberInfo>> EnumerableMembers = [];
    
    public NativeController(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Inject(this);

        // Inject non-Experimental Properties
        pluginInterface.Inject(DalamudInterface.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(DalamudInterface.Instance);

        // Inject Experimental Properties
        pluginInterface.Inject(Experimental.Instance);
        DalamudInterface.Instance.GameInteropProvider.InitializeFromAttributes(Experimental.Instance);

        Experimental.Instance.EnableHooks();

        GenerateKamiToolKitTypeMaps();
        GenerateCallingAssemblyTypeMap(Assembly.GetCallingAssembly());

        // Force enable Verbose so that users are able to get advanced logging information on request.
        DalamudInterface.Instance.Log.MinimumLogLevel = LogEventLevel.Verbose;
    }

    public void Dispose()
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
            NodeBase.DisposeNodes();
            NativeAddon.DisposeAddons();

            Experimental.Instance.DisposeHooks();
            OverlayNodeController.Dispose();
        });

    public void AttachNode(NodeBase customNode, NodeBase targetNode, NodePosition? position = null)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => AttachToNodeBase(customNode, targetNode, position));

    public void AttachNode(NodeBase customNode, AtkResNode* targetNode, NodePosition? position = null)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => AttachToAtkResNode(customNode, targetNode, position));

    public void AttachNode(NodeBase customNode, AtkComponentNode* targetNode, NodePosition position = NodePosition.AfterAllSiblings)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => AttachToAtkComponentNode(customNode, targetNode, position));

    public void AttachNode(NodeBase customNode, NativeAddon targetAddon, NodePosition? position = null)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => AttachToNativeAddon(customNode, targetAddon, position));

    public void DetachNode(NodeBase? customNode, Action? disposeAction = null)
        => DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => DetachNodeTask(customNode, disposeAction));

    public void DisposeNode<T>(ref T? customNode) where T : NodeBase {
        var node = Interlocked.Exchange(ref customNode, null);
        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => DisposeNodeTask(node));
    }

    private void AttachToNodeBase(NodeBase customNode, NodeBase targetNode, NodePosition? position) {
        Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}] to another Custom Node [{targetNode.GetType()}]");
        var addon = GetAddonForNode(targetNode.InternalResNode);

        switch (targetNode) {

            // Don't attach directly to ComponentNode, attach to its managed RootNode
            case ComponentNode componentNode:
                customNode.AttachNode(componentNode, position ?? NodePosition.AfterAllSiblings);
                customNode.EnableEvents(addon);
                return;

            default:
                customNode.AttachNode(targetNode, position ?? NodePosition.AsLastChild);
                customNode.EnableEvents(addon);
                return;
        }
    }

    private void AttachToAtkResNode(NodeBase customNode, AtkResNode* targetNode, NodePosition? position) {
        Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint)customNode.InternalResNode:X}] to a native AtkResNode");
        var addon = GetAddonForNode(targetNode);

        customNode.RegisterAutoDetach(addon);
        customNode.AttachNode(targetNode, position ?? NodePosition.AsLastChild);
        customNode.EnableEvents(addon);
    }

    private static void AttachToNativeAddon(NodeBase customNode, NativeAddon targetAddon, NodePosition? position) {
        Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint)customNode.InternalResNode:X}] to a Custom Addon [{targetAddon.GetType()}]");

        customNode.AttachNode(targetAddon, position ?? NodePosition.AsLastChild);
        customNode.EnableEvents(targetAddon.InternalAddon);
    }

    private void AttachToAtkComponentNode(NodeBase customNode, AtkComponentNode* targetNode, NodePosition position) {
        if (targetNode->GetNodeType() is not NodeType.Component) {
            Log.Error("TargetNode type was expected to be Component but was not. Aborting attach.");
            return;
        }

        Log.Verbose($"[NativeController] Attaching [{customNode.GetType()}:{(nint)customNode.InternalResNode:X}] to a native AtkComponentNode");

        var addon = GetAddonForNode((AtkResNode*)targetNode);
        if (addon is not null) {
            Log.Verbose($"[NativeController] Tried to get Addon from native AtkComponentNode, found: {addon->NameString}");

            customNode.RegisterAutoDetach(addon);
            customNode.AttachNode(targetNode, position);
            customNode.EnableEvents(addon);
        }
        else {
            Log.Error($"[NativeController] Attempted to attach [{customNode.GetType()}:{(nint)customNode.InternalResNode:X}] to a native AtkComponentNode, but could not find parent addon. Aborting.");
        }
    }

    private static void DetachNodeTask(NodeBase? customNode, Action? disposeAction) {
        if (customNode is not null) {
            Log.Verbose($"[NativeController] Detaching [{customNode.GetType()}:{(nint)customNode.InternalResNode:X}] from all sources.");
        }

        customNode?.DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);
        customNode?.UnregisterAutoDetach();
        customNode?.DisableEvents();
        customNode?.DetachNode();
        disposeAction?.Invoke();
    }

    private void DisposeNodeTask(NodeBase? customNode) {
        if (customNode is not null) {
            Log.Verbose($"[NativeController] Disposing [{customNode.GetType()}:{(nint)customNode.InternalResNode:X}] from all sources.");
        }

        customNode?.DisableEditMode(NodeEditMode.Move | NodeEditMode.Resize);
        customNode?.UnregisterAutoDetach();
        customNode?.DisableEvents();
        customNode?.DetachNode();
        customNode?.Dispose();
    }

    private AtkUnitBase* GetAddonForNode(AtkResNode* node)
        => RaptureAtkUnitManager.Instance()->GetAddonByNode(node);


    private void GenerateKamiToolKitTypeMaps() {
        var stopwatch = Stopwatch.StartNew();
        
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var nodeBaseTypes = types.Where(type => typeof(NodeBase).IsAssignableFrom(type));

        foreach (var type in nodeBaseTypes) {
            Log.Verbose($"Generating TypeMap for: {type}");
            
            var members = GetMembers(type);
            if (members.Count is not 0) {
                ChildMembers.TryAdd(type, members);
            }

            var enumerableMembers = GetEnumerables(type);
            if (enumerableMembers.Count is not 0) {
                EnumerableMembers.TryAdd(type, enumerableMembers);
            }
        }
        
        stopwatch.LogTime("KTK TYPEMAP");
    }

    private static void GenerateCallingAssemblyTypeMap(Assembly callingAssembly) {
        var stopwatch = Stopwatch.StartNew();
        
        var types = callingAssembly.GetTypes();
        var nodeBaseTypes = types.Where(type => typeof(NodeBase).IsAssignableFrom(type));

        foreach (var type in nodeBaseTypes) {
            Log.Verbose($"Generating TypeMap for: {type}");
            
            var members = GetMembers(type);
            if (members.Count is not 0) {
                ChildMembers.TryAdd(type, members);
            }

            var enumerableMembers = GetEnumerables(type);
            if (enumerableMembers.Count is not 0) {
                EnumerableMembers.TryAdd(type, enumerableMembers);
            }
        }
        
        stopwatch.LogTime("CALLER ASSEMBLY");
    }
    
    private static List<MemberInfo> GetMembers(Type type) {
        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var targetMembers = members.Where(member => member.MemberType is MemberTypes.Field or MemberTypes.Property);
        var assignableMembers = targetMembers.Where(member => typeof(NodeBase).IsAssignableFrom(GetMemberType(member)));
        var indexableMembers = assignableMembers.Where(IsMemberSingleIndexable);
        var finalList = indexableMembers.ToList();
        
        return finalList;
    }

    private static List<MemberInfo> GetEnumerables(Type type) {
        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var targetMembers = members.Where(member => member.MemberType is MemberTypes.Field or MemberTypes.Property);
        var enumerableMembers = targetMembers.Where(IsEnumerableOrArray);
        var indexableMembers = enumerableMembers.Where(IsMemberSingleIndexable);
        var finalList = indexableMembers.ToList();
        
        return finalList;
    }

    private static Type? GetMemberType(MemberInfo member) => member.MemberType switch {
        MemberTypes.Field => (member as FieldInfo)?.FieldType,
        MemberTypes.Property => (member as PropertyInfo)?.PropertyType,
        _ => null,
    };
    
    private static bool IsMemberSingleIndexable(MemberInfo member) {
        if (member is not PropertyInfo property) return true;

        return property.GetIndexParameters().Length is 0;
    }

    private static bool IsEnumerableOrArray(MemberInfo member) {
        var memberType = GetMemberType(member);

        if (typeof(NodeBase).IsAssignableFrom(memberType?.GetGenericArguments().FirstOrDefault())) return true;
        if (typeof(NodeBase).IsAssignableFrom(memberType?.GetElementType())) return true;
        
        return false;
    }

    private static readonly HashSet<Type> ParsedRuntimeTypes = [];

    internal static void TryAddRuntimeType(Type type) {
        if (!type.IsGenericType) return;
        if (!ParsedRuntimeTypes.Add(type)) return;

        Log.Debug($"Generating Runtime Type Mapping for: {type}");
        var stopwatch = Stopwatch.StartNew();

        if (!ChildMembers.ContainsKey(type)) {
            var members = GetMembers(type);
            if (members.Count is not 0) {
                ChildMembers.TryAdd(type, members);
            }
        }

        if (!EnumerableMembers.ContainsKey(type)) {
            var enumerableMembers = GetEnumerables(type);
            if (enumerableMembers.Count is not 0) {
                EnumerableMembers.TryAdd(type, enumerableMembers);
            }
        }
        
        stopwatch.LogTime("RUNTIMETYPE");
    }
}
