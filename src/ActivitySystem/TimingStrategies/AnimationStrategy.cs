using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem.TimingStrategies;

[Tool][GlobalClass]
public partial class AnimationStrategy : TimingStrategy
{
	[Export(PropertyHint.NodePathValidTypes, nameof(Godot.AnimationPlayer))] public NodePath AnimationPlayer = "";
	[Export] public string Animation = "";
	// [Export] public TimingEnum Timing = TimingEnum.OnMarker;
	[Export] public string Marker = "";

	// public enum TimingEnum : byte
	// {
	// 	// OnAnimationStart = 32, // TODO
	// 	// OnAnimationLoop = 64, // TODO
	// 	// OnAnimationEnd = 96, // TODO
	// 	OnMarker = 128,
	// }

	private AnimationPlayer? AnimationPlayerObject
		=> this.ResourceLocalToScene
			? this.GetLocalScene().GetNodeOrNull<AnimationPlayer>(this.AnimationPlayer)
			: null;
	private Animation? AnimationObject
	{
		get
		{
			if (this.AnimationPlayerObject == null)
				return null;
			string animationName = this.Animation.IsNullOrWhiteSpace()
				? this.AnimationPlayerObject.CurrentAnimation
				: this.Animation;
			if (animationName?.IsNullOrWhiteSpace() != false)
				return null;
			return this.AnimationPlayerObject.GetAnimation(animationName);
		}
	}
	private double MarkerTime
		=> this.AnimationObject is Animation animation
			&& animation.HasMarker(this.Marker)
				? animation.GetMarkerTime(this.Marker)
				: double.PositiveInfinity;

	public override void _ValidateProperty(Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(AnimationPlayer):
				property["usage"] = (long) PropertyUsageFlags.Default
					| (long) PropertyUsageFlags.UpdateAllIfModified
					| (long) PropertyUsageFlags.NodePathFromSceneRoot;
				break;
			case nameof(this.Animation):
				if (this.AnimationPlayerObject == null)
					break;
				property["hint"] = (long) PropertyHint.Enum;
				property["hint_string"] = this.AnimationPlayerObject.GetAnimationList().JoinIntoString(",");
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			// case nameof(this.Timing):
			// 	property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
			// 	break;
			case nameof(this.Marker):
				// if (this.Timing != TimingEnum.OnMarker)
				// {
				// 	property["usage"] = (long) PropertyUsageFlags.None;
				// 	break;
				// }
				if (this.AnimationObject == null)
					break;
				property["hint"] = (long) PropertyHint.Enum;
				property["hint_string"] = this.AnimationObject.GetMarkerNames().JoinIntoString(",");
				break;
			default:
				if (
					property["name"].AsString() == Resource.PropertyName.ResourceLocalToScene.ToString()
					&& !string.IsNullOrWhiteSpace(this.AnimationPlayer.ToString())
				)
				{
					this.ResourceLocalToScene = true;
					property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.ReadOnly;
					property["info"] = $"{nameof(AnimationStrategy).BBCCode()}.{Resource.PropertyName.ResourceLocalToScene.ToString().BBCCode()} must be checked because it references an {nameof(AnimationPlayer).BBCCode()} node in the scene.";
				}
				break;
		}
	}
	public override bool Test(Activity? activity)
		=> (this.AnimationPlayerObject?.CurrentAnimationPosition ?? 0d)
				>= this.MarkerTime - Mathf.Epsilon;
}
