using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

[Tool][GlobalClass]
public partial class RemoteProperty : Node
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	// public static readonly string MyConstant = "";

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EXPORTS
	//==================================================================================================================

	[Export] public string ParentProperty = "";
	[Export] public Node? ReferenceNode;
	[Export] public string ReferenceProperty = "";
	[Export] public Variant DefaultValue;
	[Export] public UpdateModeEnum UpdateMode = UpdateModeEnum.ProcessFrames;
	[Export] public int FrameSkipping
		{ get; set { field = value.AtLeast(0); } }
		= 0;
	[Export(PropertyHint.None, "suffix:s")] public float UpdateFrequency
		{ get; set { field = value.AtLeast(0f); } }
		= 1f;

	[ExportGroup("Debug", "Debug")]
	[Export] public bool RunInEditor = false;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

	private int SkippedFrames = 0;
	private float UpdateDelay = 0f;

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

	// [Signal] public delegate void EventHandler();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region INTERNAL TYPES
	//==================================================================================================================

	public enum UpdateModeEnum : sbyte {
		ProcessFrames = 16,
		PhysicsFrames = 32,
		Timed = 64,
		Manually = 96,
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	// public override string[] _GetConfigurationWarnings()
	// 	=> (base._GetConfigurationWarnings() ?? [])
	// 		.AppendIf(false "This node is not configured correctly. Did you forget to assign a required field?")
	// 		.ToArray();

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.ParentProperty): {
				if (this.GetParent() is not Node parent)
					return;
				string[] options = parent.GetPropertyList()
					.Select(prop => prop["name"].AsString())
					.ToArray();
				property["hint"] = (long) PropertyHint.EnumSuggestion;
				property["hint_string"] = options.JoinIntoString(",");
				if (!options.Contains(this.ParentProperty))
					property["error"] = $"The property '{this.ParentProperty}' does not exist on the parent node.";
				break;
			}
			case nameof(this.ReferenceProperty): {
				if (this.ReferenceNode is not Node reference)
					return;
				Variant.Type type = this.GetPropertyType();
				string[] options = reference.GetPropertyList()
					.Where(prop => prop["type"].AsVariantType().IsConvertibleTo(type))
					.Select(prop => prop["name"].AsString())
					.ToArray();
				property["hint"] = (long) PropertyHint.EnumSuggestion;
				property["hint_string"] = options.JoinIntoString(",");
				if (!options.Contains(this.ReferenceProperty))
					property["error"] = $"The property '{this.ReferenceProperty}' does not exist on the reference node or is not compatible with the type '{type}'.";
				break;
			}
			case nameof(this.DefaultValue): {
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.NilIsVariant;
				break;
			}
			case nameof(this.FrameSkipping): {
				if (this.UpdateMode != UpdateModeEnum.ProcessFrames && this.UpdateMode != UpdateModeEnum.PhysicsFrames)
					property["usage"] = (long) PropertyUsageFlags.None;
				break;
			}
			case nameof(this.UpdateFrequency): {
				if (this.UpdateMode != UpdateModeEnum.Timed)
					property["usage"] = (long) PropertyUsageFlags.None;
				break;
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (this.UpdateMode != UpdateModeEnum.ProcessFrames)
		{
			this.SetProcess(false);
			return;
		}
		if (this.SkippedFrames >= this.FrameSkipping)
		{
			this.SkippedFrames = 0;
			this.ForceUpdateProperty();
		}
		else
			this.SkippedFrames++;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		switch (this.UpdateMode)
		{
			case UpdateModeEnum.Timed:
				this.UpdateDelay += (float) delta;
				if (this.UpdateDelay >= this.UpdateFrequency)
				{
					this.UpdateDelay -= this.UpdateFrequency;
					this.ForceUpdateProperty();
				}
				break;
			case UpdateModeEnum.Manually:
				if (this.SkippedFrames >= this.FrameSkipping)
				{
					this.SkippedFrames = 0;
					this.ForceUpdateProperty();
				}
				else
					this.SkippedFrames++;
				break;
			default:
				this.SetPhysicsProcess(false);
				break;
		}
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	private Variant.Type GetPropertyType()
		=> this.GetParent()
			?.GetPropertyList()
			.FirstOrDefault(prop => prop["name"].AsString() == this.ParentProperty)
			?["type"].AsVariantType()
			?? Variant.Type.Nil;

	public void ForceUpdateProperty()
	{
		if (Engine.IsEditorHint() && !this.RunInEditor)
			return;
		this.GetParent()?.Set(this.ParentProperty, this.ReferenceNode?.Get(this.ReferenceProperty) ?? this.DefaultValue);
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
