using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem;

[Tool][GlobalClass]
public abstract partial class VariantSource : Resource
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

	[ExportCategory(nameof(VariantSource))]
	[Export] public Variant.Type Type = Variant.Type.Nil;
	[Export] public bool StrictType = false;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

	private bool StrongType = false;

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

	// public enum Type {
	// 	Value1,
	// }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.Type):
				property["usage"] = (long) PropertyUsageFlags.Default
					| (long) PropertyUsageFlags.UpdateAllIfModified
					| (this.StrongType ? (long) PropertyUsageFlags.ReadOnly : 0);
				break;
			case nameof(this.StrictType):
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			default:
				if (property["name"].AsStringName() == Resource.PropertyName.ResourceLocalToScene)
				{
					if (!this._ReferencesSceneNode())
						break;
					this.ResourceLocalToScene = true;
					property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.ReadOnly;
					property["comment"] = "This resource must be local to the scene because it references a node within the scene.";
					break;
				}
				break;
		}
	}

	protected abstract bool _ReferencesSceneNode();
	protected abstract Variant _GetValue();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public void SetStrongType(Variant.Type type)
	{
		this.Type = type;
		this.StrongType = true;
	}

	public Variant GetValue()
		=> this._GetValue().As(this.Type);

	public bool ReferencesSceneNode()
		=> this._ReferencesSceneNode();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
