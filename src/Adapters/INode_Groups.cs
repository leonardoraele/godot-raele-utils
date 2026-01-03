using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public void AddToGroup(StringName group, bool persistent = false);
	public void RemoveFromGroup(StringName group);
	public bool IsInGroup(StringName group);
	public Godot.Collections.Array<StringName> GetGroups();
}
