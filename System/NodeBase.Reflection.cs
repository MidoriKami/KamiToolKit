using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract partial class NodeBase {
    private void VisitChildren(Action<NodeBase?> visitAction) {
        try {
            foreach (var child in GetChildren(this)) {
                DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
                    visitAction(child);
                });
            }
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    private IEnumerable<NodeBase?> GetChildren(NodeBase parent) {
        foreach (var member in GetNodeMembers(parent)) {
            if (GetNode(member, parent) is { } node)
                yield return node;
        }

        foreach (var enumerableMember in GetEnumerableMemberInfo(parent)) {
            if (GetEnumerable(enumerableMember, parent) is { } enumerable) {
                foreach (var child in enumerable) {
                    yield return child;
                }
            }
        }
    }

    private static bool IsNodeMember(MemberInfo member)
        => typeof(NodeBase).IsAssignableFrom(GetMemberType(member));

    private static IEnumerable<MemberInfo> GetNodeMembers(NodeBase parent)
        => GetMemberInfo(parent).Where(IsNodeMember).Where(IsMemberSingleIndexable);

    private static IEnumerable<MemberInfo> GetMemberInfo(NodeBase node)
        => node.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(member => member.MemberType is MemberTypes.Field or MemberTypes.Property);

    private static NodeBase? GetNode(MemberInfo member, NodeBase node) => member.MemberType switch {
        MemberTypes.Field => (member as FieldInfo)?.GetValue(node) as NodeBase,
        MemberTypes.Property => (member as PropertyInfo)?.GetValue(node) as NodeBase,
        _ => null,
    };

    private static IEnumerable<MemberInfo> GetEnumerableMemberInfo(NodeBase node)
        => GetMemberInfo(node).Where(IsMemberEnumerable).Where(IsBaseNodeEnumerable).Where(IsMemberSingleIndexable);

    private static bool IsMemberEnumerable(MemberInfo member)
        => GetMemberType(member)?.GetInterfaces().Contains(typeof(IEnumerable)) ?? false;

    private static bool IsBaseNodeEnumerable(MemberInfo member)
        => typeof(NodeBase).IsAssignableFrom(GetMemberType(member)?.GetGenericArguments().FirstOrDefault());

    private static bool IsMemberSingleIndexable(MemberInfo member) {
        if (member is not PropertyInfo property) return true;

        return property.GetIndexParameters().Length is 0;
    }

    private static IEnumerable<NodeBase?>? GetEnumerable(MemberInfo member, NodeBase node) => member.MemberType switch {
        MemberTypes.Field => (member as FieldInfo)?.GetValue(node) as IEnumerable<NodeBase>,
        MemberTypes.Property => (member as PropertyInfo)?.GetValue(node) as IEnumerable<NodeBase>,
        _ => null,
    };

    private static Type? GetMemberType(MemberInfo member) => member.MemberType switch {
        MemberTypes.Field => (member as FieldInfo)?.FieldType,
        MemberTypes.Property => (member as PropertyInfo)?.PropertyType,
        _ => null,
    };
}
