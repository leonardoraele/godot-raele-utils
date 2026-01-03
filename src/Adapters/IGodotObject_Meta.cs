using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface IGodotObject
{
	public void SetMeta(StringName name, Variant value);
	public void RemoveMeta(StringName name);
	public Variant GetMeta(StringName name, Variant @default = default);
	public bool HasMeta(StringName name);
	public Godot.Collections.Array<StringName> GetMetaList();
}
