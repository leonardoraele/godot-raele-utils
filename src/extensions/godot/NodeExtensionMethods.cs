using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class ExtensionMethods
{
	extension(Node self)
	{
		/// <summary>
		/// Gets all ancestors of the given node in the scene tree. The immediate parent is returned first, and the
		/// ancestor directly under the tree root of the scene tree is returned last. If the given node has no parent,
		/// an empty collection is returned.
		/// </summary>
		public IEnumerable<Node> GetAncestors()
		{
			for (Node? current = self.GetParent(); current != null; current = current.GetParent())
				yield return current;
		}

		public T? GetFirstAncestorOrDefault<T>()
			=> self.GetAncestors().OfType<T>().FirstOrDefault();

		public T? GetFirstChildOrDefault<T>() where T : Node
			=> self.GetChildren().OfType<T>().FirstOrDefault();

		/// <summary>
		/// Gets all descendants of the given node in the scene tree, in the order they appear in the editor tree.
		/// i.e. In depth-first order, where a parent node always precedes its children, and a parent node's children
		/// always precede the later siblings of that parent.
		///
		/// If the given node has no children, an empty collection is returned.
		/// </summary>
		/// <example>
		/// Given the following scene tree...
		///
		/// - A
		///   - B
		///     - C
		///     - D
		///   - E
		///     - F
		///     - G
		///
		/// The descendants of A are returned in this order: B, C, D, E, F, G.
		///
		/// </example>
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
