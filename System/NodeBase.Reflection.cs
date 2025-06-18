using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KamiToolKit.Classes;

namespace KamiToolKit.System;

public abstract partial class NodeBase {
	private void VisitChildren(Action<NodeBase> visitAction) {
		try {
			foreach (var child in GetChildren(this)) {
				visitAction(child);
			}
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}

	private IEnumerable<NodeBase> GetChildren(NodeBase parent) {
		foreach (var member in GetMemberInfo(parent).Where(IsNodeMember)) {
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

	private static bool IsNodeMember(MemberInfo member) => member.MemberType switch {
		MemberTypes.Field => (member as FieldInfo)?.FieldType.IsAssignableTo(typeof(NodeBase)) ?? false,
		MemberTypes.Property => (member as PropertyInfo)?.PropertyType.IsAssignableTo(typeof(NodeBase)) ?? false,
		_ => false,
	};

	private static IEnumerable<MemberInfo> GetMemberInfo(NodeBase node)
		=> node.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(member => member.MemberType is MemberTypes.Field or MemberTypes.Property);

	private static NodeBase? GetNode(MemberInfo member, NodeBase node) => member.MemberType switch {
		MemberTypes.Field => (member as FieldInfo)?.GetValue(node) as NodeBase,
		MemberTypes.Property => (member as PropertyInfo)?.GetValue(node) as NodeBase,
		_ => null,
	};

	private static IEnumerable<MemberInfo> GetEnumerableMemberInfo(NodeBase node) 
		=> GetMemberInfo(node).Where(IsMemberEnumerable).Where(IsBaseNodeEnumerable);
	
	private static bool IsMemberEnumerable(MemberInfo member) => member.MemberType switch {
		MemberTypes.Field => (member as FieldInfo)?.FieldType.GetInterfaces().Contains(typeof(IEnumerable)) ?? false,
		MemberTypes.Property => (member as PropertyInfo)?.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) ?? false,
		_ => false,
	};
	
	private static bool IsBaseNodeEnumerable(MemberInfo member) => member.MemberType switch {
		MemberTypes.Field => (member as FieldInfo)?.FieldType.GetGenericArguments().FirstOrDefault()?.IsAssignableTo(typeof(NodeBase)) ?? false,
		MemberTypes.Property => (member as PropertyInfo)?.PropertyType.GetGenericArguments().FirstOrDefault()?.IsAssignableTo(typeof(NodeBase)) ?? false,
		_ => false,
	};
	
	private static IEnumerable<NodeBase>? GetEnumerable(MemberInfo member, NodeBase node) => member.MemberType switch {
		MemberTypes.Field => (member as FieldInfo)?.GetValue(node) as IEnumerable<NodeBase>,
		MemberTypes.Property => (member as PropertyInfo)?.GetValue(node) as IEnumerable<NodeBase>,
		_ => null,
	};
}