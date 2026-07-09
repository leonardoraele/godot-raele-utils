using System;
using Godot;

namespace Raele.GodotUtils.ActivitySystem;

[Tool][GlobalClass]
public abstract partial class TimingStrategy : Resource
{
	public virtual void Started(Activity? activity) {}
	public virtual void Finished(Activity? activity) {}
	public abstract bool Test(TimeSpan activeTime);
}
