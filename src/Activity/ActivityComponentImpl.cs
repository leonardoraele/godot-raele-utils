using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public partial class ActivityComponentImpl : ActivityImpl, IActivityComponent
{
	//==================================================================================================================
		#region STATICS & CONSTRUCTORS
	//==================================================================================================================

	public const string PROPERTY_CATEGORY_NAME = "ActivityComponent";
	public const string PROPERTY_GROUP_START_STRATEGY_NAME = "StartStrategy";
	public const string PROPERTY_GROUP_START_STRATEGY_PREFIX = "Start";
	public const string PROPERTY_GROUP_FINISH_STRATEGY_NAME = "FinishStrategy";
	public const string PROPERTY_GROUP_FINISH_STRATEGY_PREFIX = "Finish";

	public ActivityComponentImpl(IWrapper wrapper) : base(wrapper) {
		WRAPPER = wrapper;
		this.EventStarted += (_mode, _argument) => this.State = StateEnum.Started;
		this.EventFinished += (_reason, _details) => this.State = StateEnum.Finished;
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

	public bool Enabled;
	public TimingStrategyEnum StartStrategy
		{ get; set { field = value; this.StartStrategyImpl = this.CreateTimingStrategyHandler(value); } }
	public TimingStrategyEnum FinishStrategy
		{ get; set { field = value; this.FinishStrategyImpl = this.CreateTimingStrategyHandler(value); } }
	public StateEnum State { get; private set; }

	private TimingStrategy StartStrategyImpl = new ImmediateTimingStrategy();
	private TimingStrategy FinishStrategyImpl = new NeverTimingStrategy();
	private IWrapper WRAPPER { get; init; }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	private IActivity? ParentActivity => WRAPPER.GetParentOrNull<IActivity>();
	IActivity? IActivityComponent.ParentActivity => this.ParentActivity;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	// [Signal] public delegate void EventHandler();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region INTERNAL TYPES
	//==================================================================================================================

	public new interface IWrapper : ActivityImpl.IWrapper
	{
		public void _ParentActivityWillStart(string mode, Variant argument, GodotCancellationController controller);
		public void _ParentActivityStarted(string mode, Variant argument);
		public void _ParentActivityWillFinish(string reason, Variant details, GodotCancellationController controller);
		public void _ParentActivityFinished(string reason, Variant details);
	}

	public enum StateEnum : byte
	{
		/// <summary>
		/// The owner ability is not active.
		/// </summary>
		Inactive,
		/// <summary>
		/// The ability has started but this ability component has not started itself yet.
		/// </summary>
		StandBy,
		/// <summary>
		/// The ability component is active.
		/// </summary>
		Started,
		/// <summary>
		/// The ability component has finished its activity and is now waiting for the owner ability to finish before it
		/// can be activated again.
		/// </summary>
		Finished,
	}

	public enum TimingStrategyEnum : byte
	{
		Immediate,
		AfterDuration,
		AnimationMarker,
		WhenExpressionIsTrue,
		Never,
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
		=> base._GetPropertyList()
			.Select(GodotPropertyInfo.FromGodotDictionary)
			.Append(new()
			{
				Name = PROPERTY_CATEGORY_NAME,
				Usage = [PropertyUsageFlags.Category],
			})
			.Append(new()
			{
				Name = nameof(this.Enabled),
				Type = Variant.Type.Bool,
				DefaultValue = true,
			})
			.Append(new()
			{
				Name = PROPERTY_GROUP_START_STRATEGY_NAME,
				Usage = [PropertyUsageFlags.Group],
				HintString = PROPERTY_GROUP_START_STRATEGY_PREFIX,
			})
			.Append(new()
			{
				Name = nameof(this.StartStrategy),
				Type = Variant.Type.Int,
				Hint = PropertyHint.Enum,
				HintString = Enum.GetNames<TimingStrategyEnum>().Join(","),
				Usage = [PropertyUsageFlags.Default, PropertyUsageFlags.UpdateAllIfModified],
				DefaultValue = (long) TimingStrategyEnum.Immediate,
			})
			.Concat(
				this.StartStrategyImpl._GetPropertyList()
					.Select(GodotPropertyInfo.FromGodotDictionary)
					.Select(property => new GodotPropertyInfo(property)
					{
						Name = PROPERTY_GROUP_START_STRATEGY_PREFIX + property.Name,
					})
			)
			.Append(new GodotPropertyInfo()
			{
				Name = PROPERTY_GROUP_FINISH_STRATEGY_NAME,
				Usage = [PropertyUsageFlags.Group],
				HintString = PROPERTY_GROUP_FINISH_STRATEGY_PREFIX,
			})
			.Append(new GodotPropertyInfo()
			{
				Name = nameof(this.FinishStrategy),
				Type = Variant.Type.Int,
				Hint = PropertyHint.Enum,
				HintString = Enum.GetNames<TimingStrategyEnum>().Join(","),
				Usage = [PropertyUsageFlags.Default, PropertyUsageFlags.UpdateAllIfModified],
				DefaultValue = (long) TimingStrategyEnum.Never,
			})
			.Concat(
				this.FinishStrategyImpl._GetPropertyList()
					.Select(GodotPropertyInfo.FromGodotDictionary)
					.Select(property => new GodotPropertyInfo(property)
					{
						Name = PROPERTY_GROUP_FINISH_STRATEGY_PREFIX + property.Name,
					})
			)
			.Select(GodotPropertyInfo.ToGodotDictionary)
			.ToGodotArrayT();
	public override Variant _Get(StringName property)
		=> property.ToString() switch
		{
			nameof(this.Enabled) => this.Enabled,
			nameof(this.StartStrategy) => (long) this.StartStrategy,
			nameof(this.FinishStrategy) => (long) this.FinishStrategy,
			string propertyName when propertyName.StartsWith(PROPERTY_GROUP_START_STRATEGY_PREFIX)
				=> this.StartStrategyImpl._Get(property.ToString().Substring(PROPERTY_GROUP_START_STRATEGY_PREFIX.Length)),
			string propertyName when propertyName.StartsWith(PROPERTY_GROUP_FINISH_STRATEGY_PREFIX)
				=> this.FinishStrategyImpl._Get(property.ToString().Substring(PROPERTY_GROUP_FINISH_STRATEGY_PREFIX.Length)),
			_ => Variant.NULL,
		};
	public override bool _Set(StringName property, Variant value)
	{
		switch (property.ToString())
		{
			case nameof(this.Enabled):
				this.Enabled = value.AsBool();
				return true;
			case nameof(this.StartStrategy):
				this.StartStrategy = (TimingStrategyEnum) value.AsInt64();
				return true;
			case nameof(this.FinishStrategy):
				this.FinishStrategy = (TimingStrategyEnum) value.AsInt64();
				return true;
			default:
				if (property.ToString().StartsWith(PROPERTY_GROUP_START_STRATEGY_PREFIX))
					return this.StartStrategyImpl._Set(property.ToString().Substring(PROPERTY_GROUP_START_STRATEGY_PREFIX.Length), value);
				if (property.ToString().StartsWith(PROPERTY_GROUP_FINISH_STRATEGY_PREFIX))
					return this.FinishStrategyImpl._Set(property.ToString().Substring(PROPERTY_GROUP_FINISH_STRATEGY_PREFIX.Length), value);
				return false;
		}
	}
	public override void _EnterTree()
	{
		base._EnterTree();
		if (Engine.IsEditorHint())
			return;
		this.ParentActivity?.EventWillStart += this.OnActivityWillStart;
		this.ParentActivity?.EventStarted += this.OnActivityStarted;
		this.ParentActivity?.EventWillFinish += this.OnActivityWillFinish;
		this.ParentActivity?.EventFinished += this.OnActivityFinished;
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		if (Engine.IsEditorHint())
			return;
		this.ParentActivity?.EventWillStart -= this.OnActivityWillStart;
		this.ParentActivity?.EventStarted -= this.OnActivityStarted;
		this.ParentActivity?.EventWillFinish -= this.OnActivityWillFinish;
		this.ParentActivity?.EventFinished -= this.OnActivityFinished;
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (Engine.IsEditorHint())
			return;
		if (this.State == StateEnum.StandBy && this.TestStartConditions())
			this.AsActivity().Start();
		if (this.State == StateEnum.Started && this.TestFinishConditions())
			this.AsActivity().Finish();
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	private void OnActivityWillStart(string mode, Variant argument, GodotCancellationController controller)
		=> WRAPPER._ParentActivityWillStart(mode, argument, controller);
	private void OnActivityStarted(string mode, Variant argument)
	{
		this.State = StateEnum.StandBy;
		WRAPPER._ParentActivityStarted(mode, argument);
	}
	private void OnActivityWillFinish(string reason, Variant details, GodotCancellationController controller)
		=> WRAPPER._ParentActivityWillFinish(reason, details, controller);
	private void OnActivityFinished(string reason, Variant details)
	{
		this.State = StateEnum.Finished;
		WRAPPER._ParentActivityFinished(reason, details);
	}

	private bool TestStartConditions() => this.StartStrategyImpl.Test();
	private bool TestFinishConditions() => this.FinishStrategyImpl.Test();

	private TimingStrategy CreateTimingStrategyHandler(TimingStrategyEnum strategy)
		=> strategy switch
		{
			TimingStrategyEnum.Immediate => new ImmediateTimingStrategy(),
			TimingStrategyEnum.AfterDuration => new AfterDurationTimingStrategy(WRAPPER),
			TimingStrategyEnum.AnimationMarker => new AnimationMarkerTimingStrategy(WRAPPER),
			TimingStrategyEnum.WhenExpressionIsTrue => new ExpressionTimingStrategy(WRAPPER),
			TimingStrategyEnum.Never => new NeverTimingStrategy(),
			_ => throw new NotImplementedException($"Timing strategy {strategy} is not implemented yet."),
		};

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region Strategy Implementations
	//==================================================================================================================

	private abstract partial class TimingStrategy : GodotObject
	{
		public bool TryGetProperty(string propertyName, out Variant value)
		{
			value = this._Get(propertyName);
			return value.VariantType != Variant.Type.Nil;
		}
		public abstract bool Test();
	}

	private partial class ImmediateTimingStrategy : TimingStrategy
	{
		public override bool Test() => true;
	}

	private partial class AfterDurationTimingStrategy(IWrapper WRAPPER) : TimingStrategy
	{
		private double Duration { get; set; } = 1f;
		private double ElapsedSeconds = 0f;

		public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
			=> new List<GodotPropertyInfo>()
			{
				new()
				{
					Name = nameof(Duration),
					Type = Variant.Type.Float,
					HintString = "suffix:s",
					Usage = [PropertyUsageFlags.Default],
					DefaultValue = 1d,
				},
			}
			.Select(GodotPropertyInfo.ToGodotDictionary)
			.ToGodotArrayT();
		public override Variant _Get(StringName property)
			=> property.ToString() == nameof(Duration) ? this.Duration : Variant.NULL;
		public override bool _Set(StringName property, Variant value)
		{
			switch (property.ToString())
			{
				case nameof(Duration):
					this.Duration = value.AsDouble();
					return true;
			}
			return false;
		}
		public override bool Test()
		{
			this.ElapsedSeconds += WRAPPER.AsNode().GetProcessDeltaTime();
			if (this.ElapsedSeconds >= this.Duration)
			{
				this.ElapsedSeconds = 0.0;
				return true;
			}
			return false;
		}
	}

	private partial class AnimationMarkerTimingStrategy(IWrapper wrapper) : TimingStrategy
	{
		private AnimationPlayer? AnimationPlayer;
		private string Animation = "";
		private string Marker = "";
		private Animation? AnimationObject
		{
			get
			{
				if (this.AnimationPlayer == null)
					return null;
				string animationName = string.IsNullOrWhiteSpace(this.Animation)
					? this.AnimationPlayer.CurrentAnimation
					: this.Animation;
				if (string.IsNullOrWhiteSpace(animationName))
					return null;
				return this.AnimationPlayer.GetAnimation(animationName);
			}
		}
		private double MarkerTime
			=> this.AnimationObject is Animation animation
				&& animation.HasMarker(this.Marker)
					? animation.GetMarkerTime(this.Marker)
					: double.PositiveInfinity;
		public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
			=> new GodotPropertyInfo[]
				{
					new()
					{
						Name = nameof(AnimationPlayer),
						Type = Variant.Type.Object,
						Hint = PropertyHint.NodeType,
						HintString = nameof(Godot.AnimationPlayer),
						Usage = [PropertyUsageFlags.Default, PropertyUsageFlags.UpdateAllIfModified],
						DefaultValue = Variant.NULL,
					},
					new()
					{
						Name = nameof(Animation),
						Type = Variant.Type.String,
						DefaultValue = "",
					},
					new()
					{
						Name = nameof(Marker),
						Type = Variant.Type.String,
						DefaultValue = "",
					},
				}
				.Select(GodotPropertyInfo.ToGodotDictionary)
				.ToGodotArrayT();
		public override void _ValidateProperty(Godot.Collections.Dictionary property)
		{
			switch (property["name"].AsString())
			{
				case nameof(this.Animation):
					if (this.AnimationPlayer == null)
						break;
					property["hint"] = (long) PropertyHint.Enum;
					property["hint_string"] = this.AnimationPlayer.GetAnimationList().Join(",");
					property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
					if (!this.AnimationPlayer.HasAnimation(this.Animation))
						wrapper.Set(nameof(this.Animation), "");
					break;
				case nameof(this.Marker):
					if (this.AnimationObject is not Animation animation)
						break;
					property["hint"] = (long) PropertyHint.Enum;
					property["hint_string"] = animation.GetMarkerNames().Join(",");
					if (!animation.HasMarker(this.Marker))
						wrapper.Set(nameof(this.Marker), "");
					break;
			}
		}
		public override Variant _Get(StringName property)
			=> property.ToString() switch
			{
				nameof(AnimationPlayer) => this.AnimationPlayer ?? Variant.NULL,
				nameof(Animation) => this.Animation,
				nameof(Marker) => this.Marker,
				_ => Variant.NULL,
			};
		public bool HandleSetProperty(string propertyName, Variant value)
		{
			switch (propertyName)
			{
				case nameof(AnimationPlayer):
					this.AnimationPlayer = value.AsGodotObject<AnimationPlayer>();
					return true;
				case nameof(Animation):
					this.Animation = value.AsString();
					return true;
				case nameof(Marker):
					this.Marker = value.AsString();
					return true;
			}
			return false;
		}
		public override bool Test()
			=> (this.AnimationPlayer?.CurrentAnimationPosition ?? 0d)
					>= this.MarkerTime - Mathf.Epsilon;
	}

	private partial class ExpressionTimingStrategy(IWrapper wrapper) : TimingStrategy
	{
		private Node? Context;
		private Variant Param;
		private string Expression
			{ get; set { field = value; this.Interpreter = null!; } }
			= "";
		private Expression Interpreter
		{
			get
			{
				if (field == null)
				{
					field = new();
					field.Parse(this.Expression, ["param"]);
				}
				return field;
			}
			set;
		}
		public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
			=> new GodotPropertyInfo[]
			{
				new()
				{
					Name = nameof(Context),
					Type = Variant.Type.Object,
					Hint = PropertyHint.NodeType,
					DefaultValue = wrapper.AsNode(),
				},
				new()
				{
					Name = nameof(Param),
					Type = Variant.Type.Nil,
					Usage = [PropertyUsageFlags.Default, PropertyUsageFlags.NilIsVariant],
					DefaultValue = Variant.NULL,
				},
				new()
				{
					Name = nameof(Expression),
					Type = Variant.Type.String,
					Hint = PropertyHint.Expression,
					DefaultValue = "",
				},
			}
			.Select(GodotPropertyInfo.ToGodotDictionary)
			.ToGodotArrayT();
		public override Variant _Get(StringName property)
			=> property.ToString() switch
			{
				nameof(Context) => this.Context ?? Variant.NULL,
				nameof(Param) => this.Param,
				nameof(Expression) => this.Expression,
				_ => new Variant(),
			};
		public override bool _Set(StringName property, Variant value)
		{
			switch (property.ToString())
			{
				case nameof(Context):
					this.Context = value.AsGodotObject<Node>();
					return true;
				case nameof(Param):
					this.Param = value;
					return true;
				case nameof(Expression):
					this.Expression = value.AsString();
					return true;
			}
			return false;
		}
		public override bool Test()
			=> this.Interpreter.Execute([this.Param], this.Context).AsBool();
	}

	private partial class NeverTimingStrategy : TimingStrategy
	{
		public override bool Test() => false;
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
