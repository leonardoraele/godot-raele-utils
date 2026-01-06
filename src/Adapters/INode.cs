using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode : IGodotObject
{
	public Node AsNode() => (Node) this;

	public void SetName(StringName name);
	public void SetName(string name);
	public StringName GetName();

	public Node Duplicate(int flags = 15);
	public void QueueFree();
	public void RequestReady();
	public bool IsNodeReady();

	public Tween CreateTween();

	public void QueueAccessibilityUpdate();
	public Rid GetAccessibilityElement();
}
