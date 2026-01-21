using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantTests;

[Tool][GlobalClass]
public partial class BooleanTest : VariantTest
{
	[Export(PropertyHint.Enum, "StrictFalse:0,StrictTrue:1,AnyFalsy:2,AnyTruthy:3")] public int Expect = 3;

	protected override Dictionary<string, Variant.Type> _GetParameters()
		=> [];
	protected override bool _ReferencesSceneNode() => false;
	protected override bool _Test(Variant variant)
		=> this.Expect switch
		{
			0 => variant.VariantType == Variant.Type.Bool && variant.AsBool() == false,
			1 => variant.VariantType == Variant.Type.Bool && variant.AsBool() == true,
			2 => variant.IsEmpty() == true,
			3 => variant.IsEmpty() == false,
			_ => false
		};
}
