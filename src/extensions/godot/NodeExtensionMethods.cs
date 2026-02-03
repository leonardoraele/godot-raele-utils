using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Extensions;

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
		public IEnumerable<Node> GetAncestors()
		{
			for (Node? current = self.GetParent(); current != null; current = current.GetParent())
			{
				yield return current;
			}
		}

		public T? GetAncestorOrDefault<T>()
			=> self.GetAncestors().OfType<T>().FirstOrDefault();

		public T? GetChildOrDefault<T>() where T : Node
			=> self.GetChildren().OfType<T>().FirstOrDefault();

		public IEnumerable<Node> GetDescendants()
		{
			foreach (Node child in self.GetChildren())
			{
				yield return child;
				foreach (Node descendant in child.GetDescendants())
					yield return descendant;
			}
		}
	}
}
