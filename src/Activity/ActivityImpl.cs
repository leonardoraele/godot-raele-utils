using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public class ActivityImpl(ActivityImpl.IWrapper WRAPPER) : IActivity
{
	//==================================================================================================================
	#region STATICS & CONSTRUCTORS
	//==================================================================================================================

	// public static readonly string MyConstant = "";

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

	public bool IsActive { get; private set; }
	public TimeSpan ActiveTimeSpan { get; private set; }
	public Node.ProcessModeEnum ProcessModeWhenActive;
	public Node.ProcessModeEnum ProcessModeWhenInactive;

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

	public event Action<string, Variant, GodotCancellationController>? EventWillStart;
	public event Action<string, Variant>? EventStarted;
	public event Action<string, Variant, GodotCancellationController>? EventWillFinish;
	public event Action<string, Variant>? EventFinished;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region INTERNAL TYPES
	//==================================================================================================================

	public interface IWrapper : INode
	{
		public void _ActivityWillStart(string mode, Variant argument, GodotCancellationController controller);
		public void _ActivityStarted(string mode, Variant argument);
		public void _ActivityProcess(double delta);
		public void _ActivityPhysicsProcess(double delta);
		public void _ActivityWillFinish(string reason, Variant details, GodotCancellationController controller);
		public void _ActivityFinished(string reason, Variant details);
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public virtual Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
		=> new List<GodotPropertyInfo>()
			.Append(new()
			{
				Name = nameof(IActivity),
				Usage = [PropertyUsageFlags.Category]
			})
			.Append(new()
			{
				Name = nameof(this.ProcessModeWhenActive),
				Type = Variant.Type.Int,
				Hint = PropertyHint.Enum,
				HintString = Enum.GetNames<Node.ProcessModeEnum>().Join(","),
				DefaultValue = (long) Node.ProcessModeEnum.Inherit,
			})
			.Append(new()
			{
				Name = nameof(this.ProcessModeWhenInactive),
				Type = Variant.Type.Int,
				Hint = PropertyHint.Enum,
				HintString = Enum.GetNames<Node.ProcessModeEnum>().Join(","),
				DefaultValue = (long) Node.ProcessModeEnum.Disabled,
			})
			.Select(GodotPropertyInfo.ToGodotDictionary)
			.ToGodotArrayT();
	public virtual void _ValidateProperty(Godot.Collections.Dictionary property) {}
	public virtual bool _Set(StringName property, Variant value)
	{
		switch (property.ToString())
		{
			case nameof(this.ProcessModeWhenActive):
				this.ProcessModeWhenActive = (Node.ProcessModeEnum) value.AsInt64();
				return true;
			case nameof(this.ProcessModeWhenInactive):
				this.ProcessModeWhenInactive = (Node.ProcessModeEnum) value.AsInt64();
				return true;
		}
		return false;
	}
	public virtual Variant _Get(StringName property)
		=> property.ToString() switch
		{
			nameof(this.ProcessModeWhenActive) => (long) this.ProcessModeWhenActive,
			nameof(this.ProcessModeWhenInactive) => (long) this.ProcessModeWhenInactive,
			_ => new Variant(),
		};
	public virtual void _EnterTree() {}
	public virtual void _ExitTree() {}
	public virtual void _Ready() {}
	public virtual void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			WRAPPER.SetProcess(false);
			return;
		}
		if (this.IsActive)
			WRAPPER._ActivityProcess(delta);
	}

	public virtual void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint())
		{
			WRAPPER.SetPhysicsProcess(false);
			return;
		}
		if (!this.IsActive)
			return;
		this.ActiveTimeSpan += TimeSpan.FromSeconds(delta);
		WRAPPER._ActivityPhysicsProcess(delta);
	}

	// public override Variant _Get(StringName property)
	// 	=> property.ToString() switch
	// 	{
	// 		nameof(ProcessModeWhenActive) => (long) this.ProcessModeWhenActive,
	// 		nameof(ProcessModeWhenInactive) => (long) this.ProcessModeWhenInactive,
	// 		_ => new Variant(),
	// 	};
	// public override bool _Set(StringName property, Variant value)
	// {
	// 	switch (property.ToString())
	// 	{
	// 		case nameof(ProcessModeWhenActive):
	// 			this.ProcessModeWhenActive = (Node.ProcessModeEnum) value.AsInt64();
	// 			return true;
	// 		case nameof(ProcessModeWhenInactive):
	// 			this.ProcessModeWhenInactive = (Node.ProcessModeEnum) value.AsInt64();
	// 			return true;
	// 	}
	// 	return false;
	// }

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	/// <summary>
	/// Requests the activity to start. Returns true if the activity did start, or false if the request was rejected.
	/// </summary>
	public bool Start(string mode, Variant argument)
	{
		GodotCancellationController controller = new();
		WRAPPER._ActivityWillStart(mode, argument, controller);
		this.EventWillStart?.Invoke(mode, argument, controller);
		if (controller.IsCancellationRequested)
			return false;
		WRAPPER.ProcessMode = this.ProcessModeWhenActive;
		this.ActiveTimeSpan = TimeSpan.Zero;
		this.IsActive = true;
		Callable.From(() =>
		{
			WRAPPER._ActivityStarted(mode, argument);
			this.EventStarted?.Invoke(mode, argument);
		}).CallDeferred();
		return true;
	}

	/// <summary>
	/// Requests the activity to finish. Returns true if the activity did finish, or false if the request was rejected.
	/// </summary>
	public bool Finish(string reason, Variant details)
	{
		GodotCancellationController controller = new();
		WRAPPER._ActivityWillFinish(reason, details, controller);
		this.EventWillFinish?.Invoke(reason, details, controller);
		if (controller.IsCancellationRequested)
			return false;
		WRAPPER.ProcessMode = this.ProcessModeWhenInactive;
		this.ActiveTimeSpan = TimeSpan.Zero;
		this.IsActive = false;
		Callable.From(() =>
		{
			WRAPPER._ActivityFinished(reason, details);
			this.EventFinished?.Invoke(reason, details);
		}).CallDeferred();
		return true;
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
