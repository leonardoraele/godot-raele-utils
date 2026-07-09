using System;
using Godot;

namespace Raele.GodotUtils.ActivitySystem.TimingStrategies;

[Tool][GlobalClass]
public partial class AfterDurationStrategy : TimingStrategy
{
	[Export(PropertyHint.None, "suffix:s")] public double Duration = 1;
	public override bool Test(TimeSpan activeTime)
		=> activeTime.TotalSeconds >= this.Duration;
}
