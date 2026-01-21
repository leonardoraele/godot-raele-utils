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
	[ExportGroup("Overrides", "Override")]
	[Export] public GodotObject? OverrideContext;
	[Export] public Godot.Collections.Dictionary OverrideParameters = [];

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

	public Variant.Type Type => this._GetReturnType();

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

	public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
	{
		return base._GetPropertyList();
	}

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			// case nameof(this.Type):
			// 	property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
			// 	break;
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
				if (
					property["type"].AsVariantType() == Variant.Type.Object
					&& (property["usage"].AsInt64() | (long) PropertyUsageFlags.ScriptVariable) != 0
				)
				{
					property["usage"] = property["usage"].AsInt64() | (long) PropertyUsageFlags.UpdateAllIfModified;
					if (
						this.Get(property["name"].AsString()).AsGodotObject() is VariantSource provider
						&& this._GetExpectedType(property["name"].AsString()) is Variant.Type expectedType
						&& expectedType != Variant.Type.Nil
						// && !provider.Type.IsConvertibleTo(property["type"].AsVariantType())
					)
						property["error"] = $"Type mismatch. Expected value of type {property["type"].AsVariantType()}, but the assigned value is of type {provider.Type}.";
				}

				break;
		}
	}

	/// <summary>
	/// // TODO
	/// A map of parameter name/type pairs that this source requires to function.
	///
	/// Most VariantSource implementations will simply merge the parameters of their dependencies.
	///
	/// A specific VariantSource named Parameter should export a string/type vars and return a single-entry dictionary
	/// with that info. On _GetValue(), this Parameter source will look for a value with the given name in the provided
	/// parameters and return it (or Variant.NULL if not found).
	/// </summary>
	protected abstract Godot.Collections.Dictionary<string, Variant.Type> _GetParameters();
	/// <summary>
	/// Should return true if this source references a node within the scene. i.e. If any exported variable is a
	/// NodePath.
	///
	/// // TODO Should do it automatically in _ValidateProperty() and _GetPropertyList()
	/// </summary>
	protected abstract bool _ReferencesSceneNode();
	/// <summary>
	/// Determines what type is expected for a given exported property, or Variant.Type.Nil if any type is accepted.
	///
	/// We match this type with the one returned from _ReturnTypes to validate assignments in the editor.
	/// </summary>
	protected virtual Variant.Type _GetExpectedType(string propertyName) => Variant.Type.Nil;
	/// <summary>
	/// Determines what type this source implementation can return, or an empty collection if it can return any value.
	/// </summary>
	protected abstract Variant.Type _GetReturnType();
	protected abstract Variant _GetValue(GodotObject self, Godot.Collections.Dictionary @params);

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public Godot.Collections.Dictionary<string, Variant.Type> GetRequiredParamters()
		=> this._GetParameters();

	public Variant GetValue(GodotObject? context = null, Godot.Collections.Dictionary? args = null)
	{
		Variant value = this._GetValue(this.OverrideContext ?? context ?? Engine.GetSceneTree().Root, args ?? []);
		return value.As(this._GetReturnType());
		// return this.Type != Variant.Type.Nil
		// 	? value.As(this.Type)
		// 	: value;
	}

	public bool ReferencesSceneNode()
		=> this._ReferencesSceneNode();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
