using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem;

[Tool]
public partial class ActivityComponent : Activity, IActivityComponent
{
	//==================================================================================================================
	#region STATICS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region EXPORTS
	//==================================================================================================================

	[Export] public bool Enabled = true;

	[ExportGroup("Use Start Strategy", "Start")]
	[Export(PropertyHint.GroupEnable)] public bool StartStrategyEnabled = false;
	[Export] public TimingStrategy? StartStrategy;
	[ExportGroup("Use Duration Strategy", "Finish")]
	[Export(PropertyHint.GroupEnable)] public bool FinishStrategyEnabled = false;
	[Export] public TimingStrategy? FinishStrategy;

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

	public IActivity? ParentActivity => this.GetAncestorOrDefault<IActivity>();

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
		/// The owner ability is not active.
		/// </summary>
		Inactive = 0,
		/// <summary>
		/// The ability has started but this ability component has not started itself yet.
		/// </summary>
		StandBy = 32,
		/// <summary>
		/// The ability component is active.
		/// </summary>
		Started = 64,
		/// <summary>
		/// The ability component has finished its activity and is now waiting for the owner ability to finish before it
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
		this.ParentActivity?.EventWillStart += this.OnParentActivityWillStart;
		this.ParentActivity?.EventStarted += this.OnParentActivityStarted;
		this.ParentActivity?.EventWillFinish += this.OnParentActivityWillFinish;
		this.ParentActivity?.EventFinished += this.OnParentActivityFinished;
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		if (Engine.IsEditorHint())
			return;
		this.ParentActivity?.EventWillStart -= this.OnParentActivityWillStart;
		this.ParentActivity?.EventStarted -= this.OnParentActivityStarted;
		this.ParentActivity?.EventWillFinish -= this.OnParentActivityWillFinish;
		this.ParentActivity?.EventFinished -= this.OnParentActivityFinished;
	}
	protected override void _ActivityPhysicsProcess(double delta)
	{
		base._ActivityPhysicsProcess(delta);
		if (Engine.IsEditorHint())
			return;
		if (this.State == StateEnum.Started && this.TestFinishConditions())
			this.Finish();
	}

	protected override void _ActivityWillStart(string mode, Variant argument, GodotCancellationController controller)
	{
		base._ActivityWillStart(mode, argument, controller);
		if (!this.Enabled)
			controller.Cancel();
	}

	protected override void _InternalPhysicsProcess()
	{
		base._InternalPhysicsProcess();
		if (
			this.State == StateEnum.StandBy
			// Prevents starting if the node wouldn't be able to process (e.g. starting while the game is paused)
			&& this.ProcessModeWhenActive switch
			{
			 	ProcessModeEnum.Always => true,
				ProcessModeEnum.Pausable => !Engine.GetSceneTree().Paused,
				ProcessModeEnum.WhenPaused => Engine.GetSceneTree().Paused,
				ProcessModeEnum.Inherit => this.GetParent()?.CanProcess() == true,
				_ => false
			}
			&& this.TestStartConditions()
		)
			this.Start();
	}

	protected override void _ActivityStarted(string mode, Variant argument)
	{
		base._ActivityStarted(mode, argument);
		this.State = StateEnum.Started;
	}

	protected override void _ActivityFinished(string reason, Variant details)
	{
		base._ActivityFinished(reason, details);
		this.State = this.ParentActivity?.IsActive == true
			? StateEnum.Finished
			: StateEnum.Inactive;
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
		{
			// No need to update the state here because finishing this component will trigger _ActivityFinished(), which
			// updates the state accordingly.
			this.ForceFinish();
		}
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
			&& this.ParentActivity?.IsActive == true
			&& (!this.StartStrategyEnabled || this.StartStrategy?.Test(this.ParentActivity) == true);
	private bool TestFinishConditions()
		=> !this.Enabled
			|| this.ParentActivity?.IsActive != true
			|| this.FinishStrategyEnabled && this.FinishStrategy?.Test(this.ParentActivity) == true;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
