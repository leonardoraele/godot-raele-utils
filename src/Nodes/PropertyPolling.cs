using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Nodes;

[Tool][GlobalClass][Icon($"./{nameof(PropertyPolling)}.svg")]
public partial class PropertyPolling : Node
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

	[Export] public string ParentProperty
		{ get; set { field = value; this.UpdateConfigurationWarnings(); } }
		= "";
	[Export] public Node? ReferenceNode
		{ get; set { field = value; this.NotifyPropertyListChanged(); this.UpdateConfigurationWarnings(); } }
	[Export] public string ReferenceProperty
		{ get; set { field = value; this.UpdateConfigurationWarnings(); } }
		= "";
	[Export] public UpdateModeEnum UpdateMode = UpdateModeEnum.IdleFrames;

	[ExportGroup("Additional Options")]
	[Export] public ValueMapper? ValueMapper;

	[ExportGroup("Debug")]
	[Export] public bool RunInEditor = false;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

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
		IdleFrames = 16,
		PhysicsFrames = 32,
		Manually = 96,
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public override string[] _GetConfigurationWarnings()
		=> (base._GetConfigurationWarnings() ?? [])
			.AppendIf(this.ParentProperty.IsNullOrWhiteSpace(), "ParentProperty is empty.")
			.AppendIf(this.ReferenceNode == null, "ReferenceNode is null.")
			.AppendIf(this.ReferenceProperty.IsNullOrWhiteSpace(), "ReferenceProperty is empty.")
			.ToArray();

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.ParentProperty): {
				if (this.GetParent() is not Node parent)
					return;
				string options = parent.GetPropertyList()
					.Where(prop => prop["usage"].AsPropertyUsageFlags() != PropertyUsageFlags.Group
						&& prop["usage"].AsPropertyUsageFlags() != PropertyUsageFlags.Subgroup
						&& prop["usage"].AsPropertyUsageFlags() != PropertyUsageFlags.Category
					)
					.Select(prop => prop["name"].AsString())
					.ToArray()
					.SortChainable(string.Compare)
					.JoinIntoString(",");
				property["hint"] = (long) PropertyHint.EnumSuggestion;
				property["hint_string"] = options;
				break;
			}
			case nameof(this.ReferenceProperty): {
				if (this.ReferenceNode == null)
					return;
				Variant.Type type = this.GetParentPropertyType();
				string options = this.ReferenceNode.GetPropertyList()
					.Where(prop => prop["usage"].AsPropertyUsageFlags() != PropertyUsageFlags.Group
						&& prop["usage"].AsPropertyUsageFlags() != PropertyUsageFlags.Subgroup
						&& prop["usage"].AsPropertyUsageFlags() != PropertyUsageFlags.Category
					)
					.Where(prop => prop["type"].AsVariantType().IsConvertibleTo(type))
					.Select(prop => prop["name"].AsString())
					.ToArray()
					.SortChainable(string.Compare)
					.JoinIntoString(",");
				property["hint"] = (long) PropertyHint.EnumSuggestion;
				property["hint_string"] = options;
				break;
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (this.UpdateMode == UpdateModeEnum.IdleFrames)
			this.UpdateProperty();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (this.UpdateMode == UpdateModeEnum.PhysicsFrames)
			this.UpdateProperty();
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	private Variant.Type GetParentPropertyType()
		=> this.GetParent()
			?.GetPropertyList()
			.FirstOrDefault(prop => prop["name"].AsString() == this.ParentProperty)
			?["type"].AsVariantType()
			?? Variant.Type.Nil;

	public void UpdateProperty()
	{
		if (Engine.IsEditorHint() && !this.RunInEditor)
			return;
		Variant? polledVariant = this.ReferenceNode?.GetIndexed(this.ReferenceProperty);
		if (!polledVariant.HasValue)
			return;
		polledVariant = this.ValueMapper?.MapValue(polledVariant.Value) ?? polledVariant.Value;
		this.GetParent()?.SetIndexed(this.ParentProperty, polledVariant.Value);
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
