using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface IGodotObject
{
	public void Set(StringName property, Variant value);
	public Variant Get(StringName property);
	public void SetIndexed(NodePath propertyPath, Variant value);
	public Variant GetIndexed(NodePath propertyPath);
	public Godot.Collections.Array<Godot.Collections.Dictionary> GetPropertyList();
	public bool PropertyCanRevert(StringName property);
	public Variant PropertyGetRevert(StringName property);
	public void NotifyPropertyListChanged();
}
