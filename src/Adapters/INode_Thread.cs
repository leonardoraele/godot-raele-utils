using System;
using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public Variant CallDeferredThreadGroup(StringName method, params Variant[] args);
	public Variant CallDeferredThreadGroup(StringName method, ReadOnlySpan<Variant> args);
	public void SetDeferredThreadGroup(StringName property, Variant value);
	public void NotifyDeferredThreadGroup(int what);
	public Variant CallThreadSafe(StringName method, params Variant[] args);
	public Variant CallThreadSafe(StringName method, ReadOnlySpan<Variant> args);
	public void SetThreadSafe(StringName property, Variant value);
	public void NotifyThreadSafe(int what);
}
