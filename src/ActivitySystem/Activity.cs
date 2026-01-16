using System;
using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.ActivitySystem;

[Tool]
public partial class Activity : Node, IActivity
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EXPORTS
	//==================================================================================================================

	[ExportGroup("Set Process Mode Per State", "ProcessMode")]
	[Export(PropertyHint.GroupEnable)] public bool ProcessModeManagedEnabled
		{ get; set { field = value; this.NotifyPropertyListChanged(); } }
		= true;
	[Export] public ProcessModeEnum ProcessModeWhenActive
		{ get; set { field = value; this.RefreshProcessMode(); } }
		= ProcessModeEnum.Inherit;
	[Export] public ProcessModeEnum ProcessModeWhenInactive
		{ get; set { field = value; this.RefreshProcessMode(); } }
		= ProcessModeEnum.Disabled;

	[ExportGroup("Debug", "Debug")]
	[Export] public bool DebugInactiveInEditor
		{ get; set { field = value; this.RefreshProcessMode(); } }
		= false;

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

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	event Action<string, Variant, GodotCancellationController> IActivity.EventWillStart
	{
		add => Connect(nameof(WillStart), value.ToCallable());
		remove => Disconnect(nameof(WillStart), value.ToCallable());
	}
	event Action<string, Variant> IActivity.EventStarted
	{
		add => Connect(nameof(Started), value.ToCallable());
		remove => Disconnect(nameof(Started), value.ToCallable());
	}
	event Action<string, Variant, GodotCancellationController> IActivity.EventWillFinish
	{
		add => Connect(nameof(WillFinish), value.ToCallable());
		remove => Disconnect(nameof(WillFinish), value.ToCallable());
	}
	event Action<string, Variant> IActivity.EventFinished
	{
		add => Connect(nameof(Finished), value.ToCallable());
		remove => Disconnect(nameof(Finished), value.ToCallable());
	}

	[Signal] public delegate void WillStartEventHandler(string mode, Variant argument, GodotCancellationController controller);
	[Signal] public delegate void StartedEventHandler(string mode, Variant argument);
	[Signal] public delegate void WillFinishEventHandler(string reason, Variant details, GodotCancellationController controller);
	[Signal] public delegate void FinishedEventHandler(string reason, Variant details);

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region INTERNAL TYPES
	//==================================================================================================================

	// public enum Type {
	// 	Value1,
	// }

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public override void _ValidateProperty(Dictionary property)
	{
		base._ValidateProperty(property);
		if (
			property["name"].AsString() == Node.PropertyName.ProcessMode.ToString()
			&& this.ProcessModeManagedEnabled
		)
		{
			property["usage"] = (long) PropertyUsageFlags.Editor | (long) PropertyUsageFlags.ReadOnly;
			property["comment"] = $"The {Node.PropertyName.ProcessMode} property is managed by the {nameof(Activity)} script.";
		}
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (Engine.IsEditorHint())
			return;
		this.GetTree().ProcessFrame += this._ActivityProcessAlways;
		this.GetTree().PhysicsFrame += this._ActivityPhysicsProcessAlways;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Engine.IsEditorHint())
			return;
		this.GetTree().ProcessFrame -= this._ActivityProcessAlways;
		this.GetTree().PhysicsFrame -= this._ActivityPhysicsProcessAlways;
	}

	public override void _Ready()
	{
		base._Ready();
		if (Engine.IsEditorHint())
		{
			this.SetProcess(false);
			this.SetPhysicsProcess(false);
			return;
		}
		this.ForceFinish();
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			this.SetProcess(false);
			return;
		}
		if (this.IsActive)
			this._ActivityProcess(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint())
		{
			this.SetPhysicsProcess(false);
			return;
		}
		if (!this.IsActive)
			return;
		this.ActiveTimeSpan += TimeSpan.FromSeconds(delta);
		this._ActivityPhysicsProcess(delta);
	}

	protected virtual void _ActivityWillStart(string mode, Variant argument, GodotCancellationController controller) { }
	protected virtual void _ActivityStarted(string mode, Variant argument) { }
	protected virtual void _ActivityProcess(double delta) { }
	protected virtual void _ActivityPhysicsProcess(double delta) { }
	protected virtual void _ActivityProcessAlways() { }
	protected virtual void _ActivityPhysicsProcessAlways() { }
	protected virtual void _ActivityWillFinish(string reason, Variant details, GodotCancellationController controller) { }
	protected virtual void _ActivityFinished(string reason, Variant details) { }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	/// <summary>
	/// Returns the same as <see cref="CanProcess"/> would if this activity is activated.
	/// </summary>
	public bool TestActiveStateCanProcess()
		=> (this.IsActive || !this.ProcessModeManagedEnabled)
			? this.CanProcess()
			: this.ProcessModeWhenActive switch
			{
				ProcessModeEnum.Always => true,
				ProcessModeEnum.Pausable => !this.GetTree().Paused,
				ProcessModeEnum.WhenPaused => this.GetTree().Paused,
				ProcessModeEnum.Inherit => this.GetParent()?.CanProcess() == true,
				_ => false
			};

	//------------------------------------------------------------------------------------------------------------------

	public void Start(string mode = "", Variant payload = new Variant())
	{
		if (this.IsActive)
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
		if (!this.IsActive)
			this.CallDeferred(MethodName.OnAfterStarted, mode, argument);
		this.IsActive = true;
		this.ActiveTimeSpan = TimeSpan.Zero;
		this.RefreshProcessMode();
	}

	private void OnAfterStarted(string mode, Variant argument)
	{
		this._ActivityStarted(mode, argument);
		this.EmitSignalStarted(mode, argument);
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
		if (this.IsActive)
			this.CallDeferred(MethodName.OnAfterFinished, reason, details);
		this.IsActive = false;
		this.ActiveTimeSpan = TimeSpan.Zero;
		this.RefreshProcessMode();
	}

	private void OnAfterFinished(string reason, Variant details)
	{
		this._ActivityFinished(reason, details);
		this.EmitSignalFinished(reason, details);
	}
	private void RefreshProcessMode()
	{
		if (!this.ProcessModeManagedEnabled)
			return;
		this.ProcessMode = Engine.IsEditorHint()
			? this.DebugInactiveInEditor
				? this.ProcessModeWhenInactive
				: this.ProcessModeWhenActive
			: this.IsActive
				? this.ProcessModeWhenActive
				: this.ProcessModeWhenInactive;
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
