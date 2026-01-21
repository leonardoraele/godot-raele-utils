using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantTests;

[Tool][GlobalClass]
public partial class EqualityTest : VariantTest
{
	[Export] public VariantSource? Parameter;
	[Export] public bool Not;

	protected override Dictionary<string, Variant.Type> _GetParameters()
		=> this.Parameter?.GetRequiredParamters() ?? [];
	protected override bool _ReferencesSceneNode()
		=> this.Parameter?.ReferencesSceneNode() ?? false;
	protected override bool _Test(Variant variant)
		=> (this.Parameter?.GetValue() ?? Variant.NULL).Equals(variant) != this.Not;
}
