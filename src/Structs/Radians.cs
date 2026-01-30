using Godot;

namespace Raele.GodotUtils.Structs;

public struct Radians(double VALUE)
{
	public static Radians Pi => Mathf.Pi;
	public static Radians Tau => Mathf.Tau;

	public Degrees ToDegrees()
		=> Mathf.RadToDeg(this);

	public double AsDouble() => VALUE;
	public float AsFloat() => (float) VALUE;
	public Variant AsVariant() => VALUE;

	public static implicit operator double(Radians radians) => radians.AsDouble();
	public static implicit operator Radians(double value) => new Radians(value);
	public static implicit operator float(Radians radians) => radians.AsFloat();
	public static implicit operator Radians(float value) => new Radians(value);
	public static implicit operator Variant(Radians radians) => radians.AsDouble();
}

public static class RadiansExtensionMethods
{
	public static Radians AsRadians(this float value) => value;
	public static Radians AsRadians(this double value) => value;
	public static Radians AsRadians(this Variant value) => value.AsDouble();
}
