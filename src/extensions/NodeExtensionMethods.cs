using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;

namespace Raele.GodotUtils;

public static class ExtensionMethods
{
	extension(Node self)
	{
		/// <summary>
		/// Gets all ancestors of the given node in the scene tree. The immediate parent is returned first, and the tree
		/// root is returned last.
		/// </summary>
		/// <param name="node">The node whose ancestors are to be retrieved.</param>
		/// <param name="includeSelf">Whether to include the given node itself in the returned collection.</param>
		/// <returns>An enumerable collection of ancestor nodes, starting with the immediate parent and ending with the root
		/// of the scene tree.</returns>
		public IEnumerable<Node> GetAncestors(bool includeSelf = false)
		{
			if (includeSelf)
			{
				yield return self;
			}
			for (Node? current = self.GetParent(); current != null; current = current.GetParent())
			{
				yield return current;
			}
		}

		public T? GetAncestorOrDefault<T>() where T : Node
			=> self.GetAncestors().OfType<T>().FirstOrDefault();

		/// <summary>
		/// Tries to get the first ancestor of the specified type T from the given node.
		/// </summary>
		/// <typeparam name="T">The type of the ancestor node to find.</typeparam>
		/// <param name="node">The node whose ancestors are to be searched.</param>
		/// <param name="ancestor">The first ancestor of type T if found; otherwise, null.</param>
		/// <returns>True if an ancestor of type T is found; otherwise, false.</returns>
		public bool TryGetAncestor<T>([NotNullWhen(true)] out T? ancestor) where T : Node
		{
			ancestor = self.GetAncestorOrDefault<T>();
			return ancestor != null;
		}

		public T RequireAncestor<T>() where T : Node
			=> self.TryGetAncestor(out T? ancestor)
				? ancestor
				: throw new NoNullAllowedException($"Failed to find expected ancestor. Node \"{self.Name}\" ({self.GetType().Name}) must be a descendant of a {typeof(T).Name} node. Node Path: {self.GetPath()}");

		public T? GetChildOrDefault<T>() where T : Node
			=> self.GetChildren().OfType<T>().FirstOrDefault();

		public bool TryGetChild<T>([NotNullWhen(true)] out T? child) where T : Node
		{
			child = self.GetChildOrDefault<T>();
			return child != null;
		}

		public T RequireChild<T>() where T : Node
			=> self.TryGetChild(out T? child)
				? child
				: throw new NoNullAllowedException($"Failed to find expected child. Node \"{self.Name}\" ({self.GetType().Name}) must have a child of type {typeof(T).Name}.");
	}
}
