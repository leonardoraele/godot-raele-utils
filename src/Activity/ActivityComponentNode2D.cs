using System;
using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public abstract partial class ActivityComponentNode2D : Node2D, IActivityNode, ActivityComponentImpl.IWrapper
{
	//==================================================================================================================
	#region STATICS & CONSTRUCTORS
	//==================================================================================================================

	public ActivityComponentNode2D() : base()
	{
		this.Impl = new(this);
		this.Impl.EventWillStart += this.EmitSignalWillStart;
		this.Impl.EventStarted += this.EmitSignalStarted;
		this.Impl.EventWillFinish += this.EmitSignalWillFinish;
		this.Impl.EventFinished += this.EmitSignalFinished;
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region EXPORTS
	//==================================================================================================================

	// [Export] public

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region FIELDS
	//==================================================================================================================

	private ActivityComponentImpl Impl;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region COMPUTED PROPERTIES
	//==================================================================================================================

	bool IActivity.IsActive => this.Impl.IsActive;
	TimeSpan IActivity.ActiveTimeSpan => this.Impl.ActiveTimeSpan;
	public bool Enabled
	{
		get => this.Impl.Enabled;
		set => this.Impl.Enabled = value;
	}
	public ActivityComponentImpl.TimingStrategyEnum StartStrategy
	{
		get => this.Impl.StartStrategy;
		set => this.Impl.StartStrategy = value;
	}
	public ActivityComponentImpl.TimingStrategyEnum FinishStrategy
	{
		get => this.Impl.FinishStrategy;
		set => this.Impl.FinishStrategy = value;
	}
	public ActivityComponentImpl.StateEnum State => this.Impl.State;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region EVENTS & SIGNALS
	//==================================================================================================================

	event Action<string, Variant, GodotCancellationController> IActivity.EventWillStart
	{
		add => this.Connect(SignalName.WillStart, value.ToCallable());
		remove => this.Disconnect(SignalName.WillStart, value.ToCallable());
	}
	event Action<string, Variant> IActivity.EventStarted
	{
		add => this.Connect(SignalName.Started, value.ToCallable());
		remove => this.Disconnect(SignalName.Started, value.ToCallable());
	}
	event Action<string, Variant, GodotCancellationController> IActivity.EventWillFinish
	{
		add => this.Connect(SignalName.WillFinish, value.ToCallable());
		remove => this.Disconnect(SignalName.WillFinish, value.ToCallable());
	}
	event Action<string, Variant> IActivity.EventFinished
	{
		add => this.Connect(SignalName.Finished, value.ToCallable());
		remove => this.Disconnect(SignalName.Finished, value.ToCallable());
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

	public override Array<Dictionary> _GetPropertyList() => this.Impl._GetPropertyList();
	public override void _ValidateProperty(Dictionary property) => this.Impl._ValidateProperty(property);
	public override Variant _Get(StringName property) => this.Impl._Get(property);
	public override bool _Set(StringName property, Variant value) => this.Impl._Set(property, value);
	public override void _EnterTree() => this.Impl._EnterTree();
	public override void _ExitTree() => this.Impl._ExitTree();
	public override void _Ready() => this.Impl._Ready();
	public override void _Process(double delta) => this.Impl._Process(delta);
	public override void _PhysicsProcess(double delta) => this.Impl._PhysicsProcess(delta);

	public virtual void _ParentActivityWillStart(string mode, Variant argument, GodotCancellationController controller) {}
	public virtual void _ParentActivityStarted(string mode, Variant argument) {}
	public virtual void _ParentActivityWillFinish(string reason, Variant details, GodotCancellationController controller) {}
	public virtual void _ParentActivityFinished(string reason, Variant details) {}

	void ActivityImpl.IWrapper._ActivityWillStart(string mode, Variant argument, GodotCancellationController controller)
		=> this._SelfActivityWillStart(mode, argument, controller);
	void ActivityImpl.IWrapper._ActivityStarted(string mode, Variant argument)
		=> this._SelfActivityStarted(mode, argument);
	void ActivityImpl.IWrapper._ActivityProcess(double delta)
		=> this._SelfActivityProcess(delta);
	void ActivityImpl.IWrapper._ActivityPhysicsProcess(double delta)
		=> this._SelfActivityPhysicsProcess(delta);
	void ActivityImpl.IWrapper._ActivityWillFinish(string reason, Variant details, GodotCancellationController controller)
		=> this._SelfActivityWillFinish(reason, details, controller);
	void ActivityImpl.IWrapper._ActivityFinished(string reason, Variant details)
		=> this._SelfActivityFinished(reason, details);

	protected virtual void _SelfActivityWillStart(string mode, Variant argument, GodotCancellationController controller) {}
	protected virtual void _SelfActivityStarted(string mode, Variant argument) {}
	protected virtual void _SelfActivityProcess(double delta) {}
	protected virtual void _SelfActivityPhysicsProcess(double delta) {}
	protected virtual void _SelfActivityWillFinish(string reason, Variant details, GodotCancellationController controller) {}
	protected virtual void _SelfActivityFinished(string reason, Variant details) {}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	public bool Start(string mode = "", Variant arguments = new Variant())
		=> this.Impl.AsActivity().Start(mode, arguments);
	public bool Finish(string reason = "", Variant details = new Variant())
		=> this.Impl.AsActivity().Finish(reason, details);

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
