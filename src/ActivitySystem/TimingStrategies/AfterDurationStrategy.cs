using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem.TimingStrategies;

[Tool][GlobalClass]
public partial class AfterDurationStrategy : TimingStrategy
{
	private const double DEFAULT_DURATION = 1d;
	private double Duration = DEFAULT_DURATION;
	private double ElapsedSeconds;
	public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
		=> new List<GodotPropertyInfo>()
		{
			new()
			{
				Name = nameof(Duration),
				Type = Variant.Type.Float,
				HintString = "suffix:s",
				Usage = [PropertyUsageFlags.Default],
				DefaultValue = DEFAULT_DURATION,
			},
		}
		.Select(GodotPropertyInfo.ToGodotDictionary)
		.ToGodotArrayT();
	public override Variant _Get(StringName property)
		=> property.ToString() == nameof(Duration) ? this.Duration : Variant.NULL;
	public override bool _Set(StringName property, Variant value)
	{
		switch (property.ToString())
		{
			case nameof(Duration):
				this.Duration = value.AsDouble();
				return true;
		}
		return false;
	}
	public override bool _PropertyCanRevert(StringName property)
		=> property.ToString() == nameof(Duration) && this.Duration != DEFAULT_DURATION;
	public override Variant _PropertyGetRevert(StringName property)
		=> property.ToString() == nameof(Duration) ? DEFAULT_DURATION : Variant.NULL;
	public override bool Test(Activity? activity)
		=> activity?.ActiveTimeSpan.TotalSeconds >= this.Duration;
}
