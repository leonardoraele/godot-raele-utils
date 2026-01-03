using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public T GetNode<T>(NodePath path) where T : class;
	public T GetNodeOrNull<T>(NodePath path) where T : class;
	public T GetChild<T>(int idx, bool includeInternal = false) where T : class;
	public T GetChildOrNull<T>(int idx, bool includeInternal = false) where T : class;
	public T GetOwner<T>() where T : class;
	public T GetOwnerOrNull<T>() where T : class;
	public T GetParent<T>() where T : class;
	public T GetParentOrNull<T>() where T : class;

	public void AddSibling(Node sibling, bool forceReadableName = false);
	public void AddChild(Node node, bool forceReadableName = false, Node.InternalMode @internal = Node.InternalMode.Disabled);
	public void RemoveChild(Node node);
	public void Reparent(Node newParent, bool keepGlobalTransform = true);
	public int GetChildCount(bool includeInternal = false);
	public Godot.Collections.Array<Node> GetChildren(bool includeInternal = false);
	public Node GetChild(int idx, bool includeInternal = false);
	public bool HasNode(NodePath path);
	public Node GetNode(NodePath path);
	public Node GetNodeOrNull(NodePath path);
	public Node GetParent();
	public Node FindChild(string pattern, bool recursive = true, bool owned = true);
	public Godot.Collections.Array<Node> FindChildren(string pattern, string type = "", bool recursive = true, bool owned = true);
	public Node FindParent(string pattern);
	public bool HasNodeAndResource(NodePath path);
	public Godot.Collections.Array GetNodeAndResource(NodePath path);
	public bool IsInsideTree();
	public bool IsPartOfEditedScene();
	public bool IsAncestorOf(Node node);
	public bool IsGreaterThan(Node node);
	public NodePath GetPath();
	public NodePath GetPathTo(Node node, bool useUniquePath = false);
	public void MoveChild(Node childNode, int toIndex);
	public int GetIndex(bool includeInternal = false);
	public void ReplaceBy(Node node, bool keepGroups = false);

	public void PrintTree();
	public void PrintTreePretty();
	public string GetTreeString();
	public string GetTreeStringPretty();

	public void PropagateNotification(int what);
	public void PropagateCall(StringName method);
	public void PropagateCall(StringName method, Godot.Collections.Array args, bool parentFirst = false);

	public SceneTree GetTree();
}
