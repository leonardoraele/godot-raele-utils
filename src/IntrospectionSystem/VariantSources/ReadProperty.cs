using System;
using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class ReadProperty : VariantSource
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

	[ExportCategory(nameof(ReadProperty))]
	[Export(PropertyHint.NodePathValidTypes, nameof(Node))] public NodePath PropertyOwner = "";
	[Export] public string Property = "";

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

	public Node? PropertyOwnerNode
		=> this.GetLocalScene()?.GetNodeOrNull(this.PropertyOwner);

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

	// public enum Type {
	// 	Value1,
	// }

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	// public override string[] _GetConfigurationWarnings()
	// 	=> (base._GetConfigurationWarnings() ?? [])
	// 		.Concat(
	// 			false
	// 				? ["This node is not configured correctly. Did you forget to assign a required field?"]
	// 				: []
	// 		)
	// 		.ToArray();

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.PropertyOwner):
				property["usage"] = (long) PropertyUsageFlags.Default
					| (long) PropertyUsageFlags.NodePathFromSceneRoot
					| (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			case nameof(this.Property):
				if (this.PropertyOwnerNode is not Node subject)
					break;
				string[] options = subject.GetPropertyList()
					.Where(property => property["type"].AsVariantType().IsConvertibleTo(this.Type, strict: this.StrictType))
					.Select(dict => dict["name"].AsString())
					.Where(name => name.Split('/').All(part => !part.StartsWith('_')))
					.ToArray();
				options.Sort();
				property["hint"] = (long) PropertyHint.EnumSuggestion;
				property["hint_string"] = options.Join(",");
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				if (this.Property.IsWhiteSpace())
					break;
				Variant rawValue = this._GetValue();
				if (rawValue.VariantType.IsConvertibleTo(this.Type, strict: this.StrictType))
				{
					Variant value = rawValue.As(this.Type);
					property["comment"] = $"Current value: {Json.Stringify(value).BBCCode()} ({value.VariantType}).";
				}
				else
					property["error"] = "The selected property does not exist on the context node or does not match the expected type.";
				break;
		}
	}

	protected override bool _ReferencesSceneNode() => this.PropertyOwnerNode != null;
	protected override Variant _GetValue()
		=> this.PropertyOwnerNode?.GetIndexed(this.Property) ?? Variant.NULL;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
