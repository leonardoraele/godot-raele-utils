using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem;

[Tool]
public partial class ActivityComponent : Activity
{
	//==================================================================================================================
	#region STATICS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region EXPORTS
	//==================================================================================================================

	[ExportGroup("Use Start Strategy", "Start")]
	[Export(PropertyHint.GroupEnable)] public bool StartStrategyEnabled = false;
	[Export] public TimingStrategy? StartStrategy;

	// TODO
	// [ExportGroup("Options")]
	// [Export] public bool KeepActiveAfterParentFinishes = false;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region FIELDS
	//==================================================================================================================

	public StateEnum State { get; private set; } = StateEnum.Inactive;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region COMPUTED PROPERTIES
	//==================================================================================================================

	public Activity? ParentActivity => this.GetFirstAncestorOrDefault<Activity>();

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region EVENTS & SIGNALS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region INTERNAL TYPES
	//==================================================================================================================

	public enum StateEnum : sbyte
	{
		/// <summary>
		/// The owner activity is not active.
		/// </summary>
		Inactive = 0,
		/// <summary>
		/// The activity has started but this activity component has not started itself yet.
		/// </summary>
		StandBy = 32,
		/// <summary>
		/// The activity component is active.
		/// </summary>
		Started = 64,
		/// <summary>
		/// The activity component has finished its activity and is now waiting for the owner activity to finish before it
		/// can be activated again.
		/// </summary>
		Finished = 96,
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public override void _EnterTree()
	{
		base._EnterTree();
		if (Engine.IsEditorHint())
			return;
		this.ParentActivity?.WillStart += this.OnParentActivityWillStart;
		this.ParentActivity?.Started += this.OnParentActivityStarted;
		this.ParentActivity?.WillFinish += this.OnParentActivityWillFinish;
		this.ParentActivity?.Finished += this.OnParentActivityFinished;
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		if (Engine.IsEditorHint())
			return;
		this.ParentActivity?.WillStart -= this.OnParentActivityWillStart;
		this.ParentActivity?.Started -= this.OnParentActivityStarted;
		this.ParentActivity?.WillFinish -= this.OnParentActivityWillFinish;
		this.ParentActivity?.Finished -= this.OnParentActivityFinished;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Engine.IsEditorHint())
			return;
		if (this.TestStartConditions())
			this.Start();
		if (this.State == StateEnum.Started && this.ParentActivity?.IsActive != true)
			this.Finish();
	}

	protected override void _ActivityStarted(string mode, Variant argument)
	{
		base._ActivityStarted(mode, argument);
		this.State = StateEnum.Started;
		this.StartStrategy?.Started(this);
	}

	protected override void _ActivityFinished(string reason, Variant details)
	{
		base._ActivityFinished(reason, details);
		this.State = this.ParentActivity?.IsActive == true
			? StateEnum.Finished
			: StateEnum.Inactive;
		this.StartStrategy?.Finished(this);
	}

	private void OnParentActivityWillStart(string mode, Variant argument, GodotCancellationController controller)
		=> this._ParentActivityWillStart(mode, argument, controller);
	private void OnParentActivityStarted(string mode, Variant argument)
	{
		this.State = StateEnum.StandBy;
		this._ParentActivityStarted(mode, argument);
	}
	private void OnParentActivityWillFinish(string reason, Variant details, GodotCancellationController controller)
		=> this._ParentActivityWillFinish(reason, details, controller);
	private void OnParentActivityFinished(string reason, Variant details)
	{
		if (this.IsActive)
			// No need to update the state here because finishing this component will trigger _ActivityFinished(), which
			// updates the state accordingly.
			this.ForceFinish();
		else
			this.State = StateEnum.Inactive;
		this._ParentActivityFinished(reason, details);
	}

	protected virtual void _ParentActivityWillStart(string mode, Variant argument, GodotCancellationController controller) {}
	protected virtual void _ParentActivityStarted(string mode, Variant argument) {}
	protected virtual void _ParentActivityWillFinish(string reason, Variant details, GodotCancellationController controller) {}
	protected virtual void _ParentActivityFinished(string reason, Variant details) {}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	private bool TestStartConditions()
		=> this.Enabled
			&& this.State == StateEnum.StandBy
			&& !this.IsActive
			&& this.ParentActivity?.IsActive == true
			&& (!this.StartStrategyEnabled || this.StartStrategy?.Test(this.ActiveTimeSpan) == true);

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
