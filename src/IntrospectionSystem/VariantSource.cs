using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem;

[Tool][GlobalClass]
public abstract partial class VariantSource : Resource
{
	//==================================================================================================================
	#region STATICS
	//==================================================================================================================

	public static readonly string ADDITIONAL_PARAMETERS_PREFIX = "additional_parameters/";

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

	private Dictionary<string, Variant> AdditionalParameters = [];

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
		=> (base._GetPropertyList() ?? [])
			.Select(GodotPropertyInfo.FromGodotDictionary)
			.Append(new() {
				Name = nameof(VariantSource),
				Usage = [PropertyUsageFlags.Category],
			})
			.Concat(
				this.GetAdditionalParameters()
					.Select(property => property with { Name = ADDITIONAL_PARAMETERS_PREFIX + property.Name })
			)
			.Select(GodotPropertyInfo.ToGodotDictionary)
			.ToGodotArrayT();

	public override Variant _Get(StringName property)
	{
		if (property.ToString().StartsWith(ADDITIONAL_PARAMETERS_PREFIX))
		{
			string paramName = property.ToString().Substring(ADDITIONAL_PARAMETERS_PREFIX.Length);
			if (this.AdditionalParameters.ContainsKey(paramName))
				return this.AdditionalParameters[paramName];
		}
		return Variant.NULL;
	}

	public override bool _Set(StringName property, Variant value)
	{
		if (property.ToString().StartsWith(ADDITIONAL_PARAMETERS_PREFIX))
		{
			string paramName = property.ToString().Substring(ADDITIONAL_PARAMETERS_PREFIX.Length);
			if (value.Equals(Variant.NULL))
				this.AdditionalParameters.Remove(paramName);
			else
				this.AdditionalParameters[paramName] = value;
			return true;
		}
		return false;
	}

	public override Variant _PropertyGetRevert(StringName property)
		=> property.ToString().StartsWith(ADDITIONAL_PARAMETERS_PREFIX)
			? Variant.NULL
			: base._PropertyGetRevert(property);
	public override bool _PropertyCanRevert(StringName property)
		=> property.ToString().StartsWith(ADDITIONAL_PARAMETERS_PREFIX)
			&& this.AdditionalParameters.ContainsKey(property.ToString().Substring(ADDITIONAL_PARAMETERS_PREFIX.Length))
			|| base._PropertyCanRevert(property);

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
				// if (
				// 	property["type"].AsVariantType() == Variant.Type.Object
				// 	&& (property["usage"].AsInt64() | (long) PropertyUsageFlags.ScriptVariable) != 0
				// )
				// {
				// 	property["usage"] = property["usage"].AsInt64() | (long) PropertyUsageFlags.UpdateAllIfModified;
				// 	if (
				// 		this.Get(property["name"].AsString()).AsGodotObject() is VariantSource provider
				// 		&& this._GetExpectedType(property["name"].AsString()) is Variant.Type expectedType
				// 		&& expectedType != Variant.Type.Nil
				// 		// && !provider.Type.IsConvertibleTo(property["type"].AsVariantType())
				// 	)
				// 		property["error"] = $"Type mismatch. Expected value of type {property["type"].AsVariantType()}, but the assigned value is of type {provider.Type}.";
				// }

				break;
		}
	}

	/// <summary>
	/// Determines what type this source implementation can return, or an empty collection if it can return any value.
	/// </summary>
	protected abstract Variant.Type _GetReturnType();
	/// <summary>
	/// A list of properties this source expects to receive as parameters when _GetValue() is called.
	///
	/// Most VariantSource implementations will simply merge the parameters of their dependencies.
	/// </summary>
	protected abstract IEnumerable<GodotPropertyInfo> _GetAdditionalParameters();
	/// <summary>
	/// Should return true if this source references a node within the scene. i.e. If any exported variable is a
	/// NodePath.
	///
	/// This should probably only return true for the Constant source when the selected type is NodePath.
	/// </summary>
	protected abstract bool _ReferencesSceneNode();
	protected abstract Variant _GetValue(Dictionary<string, Variant> args);

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public IEnumerable<GodotPropertyInfo> GetAdditionalParameters()
	{
		return this._GetAdditionalParameters()
			.GroupBy(property => property.Name)
			.Select(grouping =>
			{
				if (grouping.Select(property => property.Type).Distinct().Count() > 1)
					GD.PushWarning(
						$"Conflicting types for parameter \"{grouping.Key}\" in {nameof(VariantSource)} \"{this.ResourcePath}\". " +
						$"Using type '{grouping.First().Type}'"
					);
				return grouping.First() is GodotPropertyInfo info ? info with
				{
					Usage = grouping.SelectMany(property => property.Usage)
						.Concat([PropertyUsageFlags.ScriptVariable])
						.ToHashSet(),
				} : null!;
			});
	}

	public Variant GetValue(Dictionary<string, Variant>? args = null)
	{
		Variant value = this._GetValue(
			new Dictionary<string, Variant>()
				.Concat(args ?? [])
				.Concat(this.AdditionalParameters)
				.ToDictionary()
		);
		return this.Type != Variant.Type.Nil
			? value.As(this.Type)
			: value;
	}

	public T GetValue<[MustBeVariant] T>(Dictionary<string, Variant>? args = null)
	{
		Variant value = this.GetValue(args);
		Variant.Type expectedType = Variant.Typeof<T>();
		if (value.VariantType != expectedType)
		{
			GD.PushWarning(
				$"Type mismatch when getting value of {nameof(VariantSource)} \"{this.ResourcePath}\". " +
				$"Expected type {expectedType}, but got type {value.VariantType}."
			);
			return Variant.GetDefault(expectedType).As<T>();
		}
		return value.As<T>();
	}

	public bool ReferencesSceneNode()
	{
		return this._ReferencesSceneNode()
			|| this.GetPropertyList()
				.Select(GodotPropertyInfo.FromGodotDictionary)
				.Any(property =>
					property.Type == Variant.Type.NodePath
					// TODO Test if this is necessary:
					// || (
					// 	property.Type == Variant.Type.Object
					// 	&& (this.Get(property.Name).AsGodotObject() is VariantSource provider)
					// 	&& provider.ReferencesSceneNode()
					// )
				);
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
