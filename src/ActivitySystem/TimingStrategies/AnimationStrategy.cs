using System;
using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem.TimingStrategies;

[Tool][GlobalClass]
public partial class AnimationStrategy : TimingStrategy
{
	[Export(PropertyHint.NodePathValidTypes, nameof(Godot.AnimationPlayer))] public NodePath AnimationPlayer = "";
	[Export] public string Animation = "";
	[Export] public TimingEnum Timing = TimingEnum.OnMarker;
	[Export] public string Marker = "";
	[Export] public float Time = 0;

	public enum TimingEnum : byte
	{
		OnAnimationStart = 32,
		OnAnimationEnd = 96,
		OnTime = 112,
		OnMarker = 128,
	}

	private AnimationPlayer? AnimationPlayerObject
		=> this.ResourceLocalToScene
			// FIXME: ERROR: The caller thread can't call the function `get_node_or_null()` on this node. Use
			// `call_deferred()` or `call_deferred_thread_group()` instead.
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
			case nameof(this.Timing):
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			case nameof(this.Marker):
				if (this.Timing != TimingEnum.OnMarker)
				{
					property["usage"] = (long) PropertyUsageFlags.None;
					break;
				}
				if (this.AnimationObject == null)
					break;
				property["hint"] = (long) PropertyHint.Enum;
				property["hint_string"] = this.AnimationObject.GetMarkerNames().JoinIntoString(",");
				break;
			case nameof(this.Time):
				if (this.Timing != TimingEnum.OnTime)
				{
					property["usage"] = (long) PropertyUsageFlags.None;
					break;
				}
				property["hint"] = (long) PropertyHint.Range;
				property["hint_string"] = $"0,{this.AnimationObject?.Length ?? 0},suffix:s";
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

	public override void Started(Activity? activity)
	{
		this.AnimationPlayerObject?.AnimationStarted += this.OnAnimationStarted;
		this.AnimationPlayerObject?.AnimationFinished += this.OnAnimationFinished;
	}

	private void OnAnimationStarted(StringName animationName)
		=> this.LastAnimationStarted = animationName.ToString();

	private void OnAnimationFinished(StringName animationName)
		=> this.LastAnimationFinished = animationName.ToString();

	private string LastAnimationStarted = "";
	private string LastAnimationFinished = "";

	public override bool Test(TimeSpan activeTime)
	{
		bool isCurrentAnimation = this.AnimationPlayerObject?.CurrentAnimation.ToString() == this.Animation;
		double currentPosition = this.AnimationPlayerObject?.CurrentAnimationPosition ?? 0;
		return this.Timing switch
		{
			TimingEnum.OnAnimationStart => this.LastAnimationStarted == this.Animation,
			TimingEnum.OnAnimationEnd => this.LastAnimationFinished == this.Animation,
			TimingEnum.OnTime => isCurrentAnimation && currentPosition > this.Time - Mathf.Epsilon,
			TimingEnum.OnMarker => isCurrentAnimation && currentPosition > this.MarkerTime - Mathf.Epsilon,
			_ => false,
		};
	}
}
