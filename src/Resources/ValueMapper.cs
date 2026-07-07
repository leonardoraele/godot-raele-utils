using Godot;

namespace Raele.GodotUtils;

[Tool][GlobalClass]
public abstract partial class ValueMapper : Resource
{
	public abstract Variant MapValue(Variant value);
}
