using System;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem;

[Tool]
public partial class Activity : Node
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EXPORTS
	//==================================================================================================================

	[Export] public bool Enabled
	{
		get;
		set {
			field = value;
			if (!field && this.IsActive)
				this.ForceFinish();
		}
	}
		= true;

	[ExportGroup("Use Finish Strategy", "Finish")]
	[Export(PropertyHint.GroupEnable)] public bool FinishStrategyEnabled = false;
	[Export] public TimingStrategy? FinishStrategy;

	[ExportGroup("Options")]
	[Export] public DisableModeEnum DisableMode = DisableModeEnum.Abort;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

	public bool IsActive { get; private set; } = false;
	public TimeSpan ActiveTimeSpan { get; private set; } = TimeSpan.Zero;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	public bool CanStart => this.Enabled && !this.IsActive && this.DisableMode switch
	{
		DisableModeEnum.Pause => true,
		DisableModeEnum.Abort or _ => this.CanProcess(),
	};

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	[Signal] public delegate void WillStartEventHandler(string mode, Variant argument, GodotCancellationController controller);
	[Signal] public delegate void StartedEventHandler(string mode, Variant argument);
	[Signal] public delegate void ProcessActiveEventHandler(double delta);
	[Signal] public delegate void PhysicsProcessActiveEventHandler(double delta);
	[Signal] public delegate void WillFinishEventHandler(string reason, Variant details, GodotCancellationController controller);
	[Signal] public delegate void FinishedEventHandler(string reason, Variant details);

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region INTERNAL TYPES
	//==================================================================================================================

	public enum DisableModeEnum {
		/// <summary>
		/// The activity will not start if the node is disabled. The <see cref="Enabled"/> property will be ignored.
		/// </summary>
		Abort = 16,
		/// <summary>
		/// The activity will start as normal, but will not emit ProcessActive and PhysicsProcessActive signals while
		/// the node remains disabled. The activity duration timer will be paused.
		/// </summary>
		Pause = 32,
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	// public override void _ValidateProperty(Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// }

	// public override void _EnterTree()
	// {
	// 	base._EnterTree();
	// }

	// public override void _ExitTree()
	// {
	// 	base._ExitTree();
	// }

	// public override void _Ready()
	// {
	// 	base._Ready();
	// }

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Engine.IsEditorHint())
		{
			this.SetProcess(false);
			return;
		}
		if (!this.IsActive)
			return;
		this.ActiveTimeSpan += TimeSpan.FromSeconds(delta);
		try
		{
			this._ActivityProcessActive(delta);
		}
		finally
		{
			this.EmitSignalProcessActive(delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (Engine.IsEditorHint())
		{
			this.SetPhysicsProcess(false);
			return;
		}
		if (!this.IsActive)
			return;
		try
		{
			this._ActivityPhysicsProcessActive(delta);
		}
		finally
		{
			this.EmitSignalPhysicsProcessActive(delta);
		}
	}

	protected virtual void _ActivityWillStart(string mode, Variant argument, GodotCancellationController controller) { }
	protected virtual void _ActivityStarted(string mode, Variant argument) { }
	protected virtual void _ActivityProcessActive(double delta) { }
	protected virtual void _ActivityPhysicsProcessActive(double delta) { }
	protected virtual void _ActivityWillFinish(string reason, Variant details, GodotCancellationController controller) { }
	protected virtual void _ActivityFinished(string reason, Variant details) { }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	/// <summary>
	/// Emits the WillStart signal. If no handler aborts the activity, it will be started and the Started signal will be
	/// emitted.
	/// </summary>
	public void Start(string mode = "", Variant payload = new Variant())
	{
		if (!this.CanStart)
			return;
		if (!this.TestWillStart(mode, payload))
			return;
		this.ForceStart(mode, payload);
	}

	private bool TestWillStart(string mode, Variant argument)
	{
		GodotCancellationController controller = new GodotCancellationController();
		this._ActivityWillStart(mode, argument, controller);
		if (controller.IsCancellationRequested)
			return false;
		this.EmitSignalWillStart(mode, argument, controller);
		return !controller.IsCancellationRequested;
	}

	/// <summary>
	/// Immediately starts the activity, if not already active, without emitting WillStart signals or calling
	/// _ActivityWillStart.
	/// </summary>
	public void ForceStart(string mode = "", Variant argument = new Variant())
	{
		if (!this.CanStart)
			return;
		this.IsActive = true;
		this.ActiveTimeSpan = TimeSpan.Zero;
		try
		{
			this._ActivityStarted(mode, argument);
		}
		finally
		{
			this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.Started, mode, argument);
		}
	}

	//------------------------------------------------------------------------------------------------------------------

	public void Finish(string reason = "", Variant details = new Variant())
	{
		if (!this.IsActive)
			return;
		if (!this.TestWillFinish(reason, details))
			return;
		this.ForceFinish(reason, details);
	}

	private bool TestWillFinish(string reason, Variant details)
	{
		GodotCancellationController controller = new GodotCancellationController();
		this._ActivityWillFinish(reason, details, controller);
		if (controller.IsCancellationRequested)
			return false;
		this.EmitSignalWillFinish(reason, details, controller);
		return !controller.IsCancellationRequested;
	}

	/// <summary>
	/// Immediately finishes the activity, if currently active, without emitting WillFinish signals or calling
	/// _ActivityWillFinish.
	/// </summary>
	public void ForceFinish(string reason = "", Variant details = new Variant())
	{
		if (!this.IsActive)
			return;
		this.IsActive = false;
		this.ActiveTimeSpan = TimeSpan.Zero;
		try
		{
			this._ActivityFinished(reason, details);
		}
		finally
		{
			this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.Finished, reason, details);
		}
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
