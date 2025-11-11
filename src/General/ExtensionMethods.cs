using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;

namespace Raele.GodotUtils;

public static class ExtensionMethods
{
	/// <summary>
	/// Gets all ancestors of the given node in the scene tree. The immediate parent is returned first, and the tree
	/// root is returned last.
	/// </summary>
	/// <param name="node">The node whose ancestors are to be retrieved.</param>
	/// <param name="includeSelf">Whether to include the given node itself in the returned collection.</param>
	/// <returns>An enumerable collection of ancestor nodes, starting with the immediate parent and ending with the root
	/// of the scene tree.</returns>
	public static IEnumerable<Node> GetAncestors(this Node node, bool includeSelf = false)
	{
		if (includeSelf)
		{
			yield return node;
		}
		for (Node? current = node.GetParent(); current != null; current = current.GetParent())
		{
			yield return current;
		}
	}

	public static T? GetAncestorOrDefault<T>(this Node node) where T : Node
		=> node.GetAncestors().FirstOrDefault(n => n is T) as T;

	/// <summary>
	/// Tries to get the first ancestor of the specified type T from the given node.
	/// </summary>
	/// <typeparam name="T">The type of the ancestor node to find.</typeparam>
	/// <param name="node">The node whose ancestors are to be searched.</param>
	/// <param name="ancestor">The first ancestor of type T if found; otherwise, null.</param>
	/// <returns>True if an ancestor of type T is found; otherwise, false.</returns>
	public static bool TryGetAncestor<T>(this Node node, [NotNullWhen(true)] out T? ancestor) where T : Node
	{
		ancestor = node.GetAncestorOrDefault<T>();
		return ancestor != null;
	}

	public static T RequireAncestor<T>(this Node node) where T : Node
	{
		T? ancestor = node.GetAncestorOrDefault<T>();
		if (ancestor == null)
		{
			throw new NoNullAllowedException($"Failed to find expected ancestor. Node \"{node.Name}\" ({node.GetType().Name}) must be a descendant of a {typeof(T).Name} node.");
		}
		return ancestor;
	}

	public static T? GetChildOrDefault<T>(this Node node) where T : Node
		=> node.GetChildren().FirstOrDefault(c => c is T) as T;

	public static bool TryGetChild<T>(this Node node, [NotNullWhen(true)] out T? child) where T : Node
	{
		child = node.GetChildOrDefault<T>();
		return child != null;
	}

	public static T RequireChild<T>(this Node node) where T : Node
	{
		T? child = node.GetChildOrDefault<T>();
		if (child == null)
		{
			throw new NoNullAllowedException($"Failed to find expected child. Node \"{node.Name}\" ({node.GetType().Name}) must have a child of type {typeof(T).Name}.");
		}
		return child;
	}
}
