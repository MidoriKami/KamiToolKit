using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract partial class NodeBase {

    private void VisitChildren(Action<NodeBase?> visitAction) {
        try {
            var callingType = GetType();
            NativeController.TryAddRuntimeType(callingType);
            
            if (NativeController.ChildMembers.TryGetValue(callingType, out var members)) {
                foreach (var memberInfo in members) {
                    if (GetNode(memberInfo, this) is { } node) {
                        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
                            visitAction(node);
                        });
                    }
                }
            }

            if (NativeController.EnumerableMembers.TryGetValue(callingType, out var enumerableMembers)) {
                foreach (var node in enumerableMembers.SelectMany(member => GetEnumerable(member, this) ?? [])) {
                    if (node is not null) {
                        DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
                            visitAction(node);
                        });
                    }
                }
            }
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    private static NodeBase? GetNode(MemberInfo member, NodeBase node) => member.MemberType switch {
        MemberTypes.Field => (member as FieldInfo)?.GetValue(node) as NodeBase,
        MemberTypes.Property => (member as PropertyInfo)?.GetValue(node) as NodeBase,
        _ => null,
    };

    private static IEnumerable<NodeBase?>? GetEnumerable(MemberInfo member, NodeBase node) => member.MemberType switch {
        MemberTypes.Field => (member as FieldInfo)?.GetValue(node) as IEnumerable<NodeBase>,
        MemberTypes.Property => (member as PropertyInfo)?.GetValue(node) as IEnumerable<NodeBase>,
        _ => null,
    };
}
