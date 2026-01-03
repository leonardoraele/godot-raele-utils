using System;
using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface IGodotObject
{
	public Godot.Collections.Array<Godot.Collections.Dictionary> GetMethodList();
	public Variant Call(StringName method, params Variant[] args);
	public Variant Call(StringName method, ReadOnlySpan<Variant> args);
	public Variant CallDeferred(StringName method, params Variant[] args);
	public Variant CallDeferred(StringName method, ReadOnlySpan<Variant> args);
	public void SetDeferred(StringName property, Variant value);
	public Variant Callv(StringName method, Godot.Collections.Array argArray);
	public bool HasMethod(StringName method);
	public int GetMethodArgumentCount(StringName method);
}
