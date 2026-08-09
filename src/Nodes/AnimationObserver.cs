using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

/// <summary>
/// This node observes the <see cref="AnimationPlayer"/> node that is its parent and emits a signal whenever the current
/// animation section changes. i.e. When it reaches any marker in the animation, it will emit a signal with the name of
/// the markers that define the section. If it's the first or last section of the animation, the signal will emit an
/// empty string for the marker at the edges.
/// </summary>
[Tool][GlobalClass][Icon("res://addons/RaeleUtils/src/Nodes/AnimationObserver.png")]
public partial class AnimationObserver : Node
{
	//==================================================================================================================
	// STATICS
	//==================================================================================================================

	// public static readonly string MyConstant = "";

	//==================================================================================================================
	// EXPORTS
	//==================================================================================================================

	[ExportGroup("Filter Animation", "Filter")]
	[Export(PropertyHint.GroupEnable)] public bool FilterEnabled;
	[Export] public string FilterAnimationName = "";

	//==================================================================================================================
	// FIELDS
	//==================================================================================================================

	//==================================================================================================================
	// COMPUTED PROPERTIES
	//==================================================================================================================

	public AnimationPlayer? AnimationPlayer => this.GetParentOrNull<AnimationPlayer>();
	public Animation? CurrentAnimation => this.AnimationPlayer?.GetAnimation(this.AnimationPlayer.CurrentAnimation);
	public double CurrentPlaybackPosition => this.AnimationPlayer?.CurrentAnimationPosition ?? 0f;

	//==================================================================================================================
	// SIGNALS
	//==================================================================================================================

	[Signal] public delegate void SectionChangedEventHandler(string animationName, string beginMarker, string endMarker);

	//==================================================================================================================
	// INTERNAL TYPES
	//==================================================================================================================

	// public enum Type {
	// 	Value1,
	// }

	//==================================================================================================================
	// VIRTUALS & OVERRIDES
	//==================================================================================================================

	public override string[] _GetConfigurationWarnings()
		=> (base._GetConfigurationWarnings() ?? [])
			.AppendIf(this.AnimationPlayer == null, $"{nameof(AnimationObserver)} node must be a direct child of an {nameof(AnimationPlayer)} node.")
			.AppendIf(this.FilterEnabled && this.AnimationPlayer?.HasAnimation(this.FilterAnimationName) == false, $"Animation '{this.FilterAnimationName}' does not exist in the parent {nameof(AnimationPlayer)}.")
			.ToArray();

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.FilterAnimationName):
				if (this.AnimationPlayer == null)
					break;
				string[] animations = this.AnimationPlayer.GetAnimationList();
				property["hint"] = (long) PropertyHint.Enum;
				property["hint_string"] = string.Join(",", animations);
				break;
		}
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (Engine.IsEditorHint())
			return;
		this.AnimationPlayer?.CurrentAnimationChanged += this.OnCurrentAnimationChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Engine.IsEditorHint())
			return;
		this.AnimationPlayer?.CurrentAnimationChanged -= this.OnCurrentAnimationChanged;
	}

	// public override void _Ready()
	// {
	// 	base._Ready();
	// }

	private string? PrevMarker = null;
	// private string? CurrMarker = null;
	private string? NextMarker = null;

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Engine.IsEditorHint())
		{
			this.SetProcess(false);
			return;
		}

		if (this.FilterEnabled && this.AnimationPlayer?.CurrentAnimation != this.FilterAnimationName)
			return;

		string? prevMarker = this.CurrentAnimation?.GetPrevMarker(this.CurrentPlaybackPosition);
		// string? currMarker = this.CurrentAnimation?.GetMarkerAtTime(this.CurrentPlaybackPosition);
		string? nextMarker = this.CurrentAnimation?.GetNextMarker(this.CurrentPlaybackPosition);

		if (!Enumerable.SequenceEqual([prevMarker, /*currMarker,*/ nextMarker], [this.PrevMarker, /*this.CurrMarker,*/ this.NextMarker]))
			this.EmitSignalSectionChanged(this.FilterAnimationName, prevMarker ?? "", nextMarker ?? "");

		this.PrevMarker = prevMarker;
		// this.CurrMarker = currMarker;
		this.NextMarker = nextMarker;
	}

	// public override void _PhysicsProcess(double delta)
	// {
	// 	base._PhysicsProcess(delta);
	// }

	//==================================================================================================================
	// METHODS
	//==================================================================================================================

	private void OnCurrentAnimationChanged(StringName animationName)
		=> this.PrevMarker = /*this.CurrMarker =*/ this.NextMarker = null;
}
