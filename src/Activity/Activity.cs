using System;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public abstract partial class Activity : Node, IActivity
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EXPORTS
	//==================================================================================================================

	[Export] public Node.ProcessModeEnum ProcessModeWhenActive = Node.ProcessModeEnum.Inherit;
	[Export] public Node.ProcessModeEnum ProcessModeWhenInactive = Node.ProcessModeEnum.Disabled;

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
	protected virtual void _ActivityWillFinish(string reason, Variant details, GodotCancellationController controller) { }
	protected virtual void _ActivityFinished(string reason, Variant details) { }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public bool Start(string mode = "", Variant argument = new Variant())
	{
		if (this.IsActive)
			return true;
		GodotCancellationController controller = new GodotCancellationController();
		this._ActivityWillStart(mode, argument, controller);
		if (controller.IsCancellationRequested)
			return false;
		this.EmitSignalWillStart(mode, argument, controller);
		if (controller.IsCancellationRequested)
			return false;
		this.SetProcessMode(this.ProcessModeWhenActive);
		this.IsActive = true;
		this.ActiveTimeSpan = TimeSpan.Zero;
		Callable.From(() =>
		{
			this._ActivityStarted(mode, argument);
			this.EmitSignalStarted(mode, argument);
		}).CallDeferred();
		return true;
	}
	public bool Finish(string reason = "", Variant details = new Variant())
	{
		if (!this.IsActive)
			return true;
		GodotCancellationController controller = new GodotCancellationController();
		this._ActivityWillFinish(reason, details, controller);
		if (controller.IsCancellationRequested)
			return false;
		this.EmitSignalWillFinish(reason, details, controller);
		if (controller.IsCancellationRequested)
			return false;
		this.SetProcessMode(this.ProcessModeWhenInactive);
		this.IsActive = false;
		this.ActiveTimeSpan = TimeSpan.Zero;
		Callable.From(() =>
		{
			this._ActivityFinished(reason, details);
			this.EmitSignalFinished(reason, details);
		}).CallDeferred();
		return true;
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
