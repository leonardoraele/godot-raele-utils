using Godot;

namespace Raele.GodotUtils;

public abstract partial class ValueMapper : Resource
{
	public abstract Variant MapValue(Variant value);
}
