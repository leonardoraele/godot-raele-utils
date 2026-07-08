using Godot;

namespace Raele.GodotUtils.ActivitySystem;

[Tool][GlobalClass]
public abstract partial class TimingStrategy : Resource
{
	public abstract bool Test(Activity? activity);
}
