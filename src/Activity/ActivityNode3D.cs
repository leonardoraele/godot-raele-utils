using System;
using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public abstract partial class ActivityNode3D : Node3D, IActivity, ActivityImpl.IWrapper
{
	//==================================================================================================================
		#region STATICS & CONSTRUCTORS
	//==================================================================================================================

	// public static readonly string MyConstant = "";

	public ActivityNode3D() : base()
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



	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

	private ActivityImpl Impl;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	public bool IsActive => this.Impl.IsActive;
	public TimeSpan ActiveTimeSpan => this.Impl.ActiveTimeSpan;
	public ProcessModeEnum ProcessModeWhenActive
	{
		get => this.Impl.ProcessModeWhenActive;
		set => this.Impl.ProcessModeWhenActive = value;
	}
	public ProcessModeEnum ProcessModeWhenInactive
	{
		get => this.Impl.ProcessModeWhenInactive;
		set => this.Impl.ProcessModeWhenInactive = value;
	}

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

	public override Array<Dictionary> _GetPropertyList() => this.Impl._GetPropertyList();
	public override Variant _Get(StringName property) => this.Impl._Get(property);
	public override bool _Set(StringName property, Variant value) => this.Impl._Set(property, value);
	public override void _EnterTree() => this.Impl._EnterTree();
	public override void _ExitTree() => this.Impl._ExitTree();
	public override void _Ready() => this.Impl._Ready();
	public override void _Process(double delta) => this.Impl._Process(delta);
	public override void _PhysicsProcess(double delta) => this.Impl._PhysicsProcess(delta);

	void ActivityImpl.IWrapper._ActivityWillStart(string mode, Variant argument, GodotCancellationController controller)
		=> this._ActivityWillStart(mode, argument, controller);
	void ActivityImpl.IWrapper._ActivityStarted(string mode, Variant argument)
		=> this._ActivityStarted(mode, argument);
	void ActivityImpl.IWrapper._ActivityProcess(double delta)
		=> this._ActivityProcess(delta);
	void ActivityImpl.IWrapper._ActivityPhysicsProcess(double delta)
		=> this._ActivityPhysicsProcess(delta);
	void ActivityImpl.IWrapper._ActivityWillFinish(string reason, Variant details, GodotCancellationController controller)
		=> this._ActivityWillFinish(reason, details, controller);
	void ActivityImpl.IWrapper._ActivityFinished(string reason, Variant details)
		=> this._ActivityFinished(reason, details);

	protected virtual void _ActivityWillStart(string mode, Variant argument, GodotCancellationController controller) {}
	protected virtual void _ActivityStarted(string mode, Variant argument) {}
	protected virtual void _ActivityProcess(double delta) {}
	protected virtual void _ActivityPhysicsProcess(double delta) {}
	protected virtual void _ActivityWillFinish(string reason, Variant details, GodotCancellationController controller) {}
	protected virtual void _ActivityFinished(string reason, Variant details) {}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public bool Start(string mode = "", Variant argument = new Variant()) => this.Impl.Start(mode, argument);
	public bool Finish(string reason = "", Variant details = new Variant()) => this.Impl.Finish(reason, details);

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
