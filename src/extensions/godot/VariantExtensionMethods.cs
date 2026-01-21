using System;
using System.Runtime.CompilerServices;
using Godot;
using Godot.NativeInterop;

namespace Raele.GodotUtils.Extensions;

public static class VariantExtensionMethods
{
	extension (Variant self)
	{
		public static Variant NULL => new Variant();

		public T? AsGodotObject<T>() where T : GodotObject
			=> self.AsGodotObject() as T;

		public string ToJsonString(string indent = "", bool sortKeys = true, bool fullPrecision = false)
			=> Json.Stringify(self, indent, sortKeys, fullPrecision);

		public bool IsNull()
			=> self.VariantType == Variant.Type.Nil;

		public bool IsEmpty()
			=> self.VariantType == Variant.Type.Dictionary ? self.AsGodotDictionary().Count == 0
				: self.VariantType.IsArrayType() ? self.AsGodotArray().Count == 0
				: self.Equals(Variant.GetDefault(self.VariantType));

		public bool IsConvertibleTo(Variant.Type other, bool strict = false)
			=> self.VariantType.IsConvertibleTo(other, strict);

		public Variant.Type AsVariantType() => (Variant.Type) self.AsInt64();
		public PropertyHint AsPropertyHint() => (PropertyHint) self.AsInt64();
		public PropertyUsageFlags AsPropertyUsageFlags() => (PropertyUsageFlags) self.AsInt64();

		public Variant As(Variant.Type type)
			=> self.VariantType == type ? self : type switch
			{
				Variant.Type.Nil => new Variant(),
				Variant.Type.Bool => self.AsBool(),
				Variant.Type.Int => self.AsInt64(),
				Variant.Type.Float => self.AsDouble(),
				Variant.Type.String => self.AsString(),
				Variant.Type.Vector2 => self.AsVector2(),
				Variant.Type.Vector2I => self.AsVector2I(),
				Variant.Type.Rect2 => self.AsRect2(),
				Variant.Type.Rect2I => self.AsRect2I(),
				Variant.Type.Vector3 => self.AsVector3(),
				Variant.Type.Vector3I => self.AsVector3I(),
				Variant.Type.Transform2D => self.AsTransform2D(),
				Variant.Type.Vector4 => self.AsVector4(),
				Variant.Type.Vector4I => self.AsVector4I(),
				Variant.Type.Plane => self.AsPlane(),
				Variant.Type.Quaternion => self.AsQuaternion(),
				Variant.Type.Aabb => self.AsAabb(),
				Variant.Type.Basis => self.AsBasis(),
				Variant.Type.Transform3D => self.AsTransform3D(),
				Variant.Type.Projection => self.AsProjection(),
				Variant.Type.Color => self.AsColor(),
				Variant.Type.StringName => self.AsStringName(),
				Variant.Type.NodePath => self.AsNodePath(),
				Variant.Type.Rid => self.AsRid(),
				Variant.Type.Object => self.AsGodotObject(),
				Variant.Type.Callable => self.AsCallable(),
				Variant.Type.Signal => self.AsSignal(),
				Variant.Type.Dictionary => self.AsGodotDictionary(),
				Variant.Type.Array => self.AsGodotArray(),
				Variant.Type.PackedByteArray => self.AsByteArray(),
				Variant.Type.PackedInt32Array => self.AsInt32Array(),
				Variant.Type.PackedInt64Array => self.AsInt64Array(),
				Variant.Type.PackedFloat32Array => self.AsFloat32Array(),
				Variant.Type.PackedFloat64Array => self.AsFloat64Array(),
				Variant.Type.PackedStringArray => self.AsStringArray(),
				Variant.Type.PackedVector2Array => self.AsVector2Array(),
				Variant.Type.PackedVector3Array => self.AsVector3Array(),
				Variant.Type.PackedColorArray => self.AsColorArray(),
				Variant.Type.PackedVector4Array => self.AsVector4Array(),
				_ => self
			};

		public static Variant.Type Typeof<[MustBeVariant] T>()
			=> Typeof(typeof(T));
		public static Variant.Type Typeof(Type type)
			=> type == typeof(bool) ? Variant.Type.Bool
			: type == typeof(char) ? Variant.Type.String
			: type == typeof(sbyte) ? Variant.Type.Int
			: type == typeof(short) ? Variant.Type.Int
			: type == typeof(int) ? Variant.Type.Int
			: type == typeof(long) ? Variant.Type.Int
			: type == typeof(byte) ? Variant.Type.Int
			: type == typeof(ushort) ? Variant.Type.Int
			: type == typeof(uint) ? Variant.Type.Int
			: type == typeof(ulong) ? Variant.Type.Int
			: type == typeof(float) ? Variant.Type.Float
			: type == typeof(double) ? Variant.Type.Float
			: type == typeof(Vector2) ? Variant.Type.Vector2
			: type == typeof(Vector2I) ? Variant.Type.Vector2I
			: type == typeof(Rect2) ? Variant.Type.Rect2
			: type == typeof(Rect2I) ? Variant.Type.Rect2I
			: type == typeof(Transform2D) ? Variant.Type.Transform2D
			: type == typeof(Vector3) ? Variant.Type.Vector3
			: type == typeof(Vector3I) ? Variant.Type.Vector3I
			: type == typeof(Basis) ? Variant.Type.Basis
			: type == typeof(Quaternion) ? Variant.Type.Quaternion
			: type == typeof(Transform3D) ? Variant.Type.Transform3D
			: type == typeof(Projection) ? Variant.Type.Projection
			: type == typeof(Vector4) ? Variant.Type.Vector4
			: type == typeof(Vector4I) ? Variant.Type.Vector4I
			: type == typeof(Aabb) ? Variant.Type.Aabb
			: type == typeof(Color) ? Variant.Type.Color
			: type == typeof(Plane) ? Variant.Type.Plane
			: type == typeof(Callable) ? Variant.Type.Callable
			: type == typeof(Signal) ? Variant.Type.Signal
			: type == typeof(string) ? Variant.Type.String
			: type == typeof(byte[]) ? Variant.Type.PackedByteArray
			: type == typeof(int[]) ? Variant.Type.PackedInt32Array
			: type == typeof(long[]) ? Variant.Type.PackedInt64Array
			: type == typeof(float[]) ? Variant.Type.PackedFloat32Array
			: type == typeof(double[]) ? Variant.Type.PackedFloat64Array
			: type == typeof(string[]) ? Variant.Type.PackedStringArray
			: type == typeof(Vector2[]) ? Variant.Type.PackedVector2Array
			: type == typeof(Vector3[]) ? Variant.Type.PackedVector3Array
			: type == typeof(Vector4[]) ? Variant.Type.PackedVector4Array
			: type == typeof(Color[]) ? Variant.Type.PackedColorArray
			: type == typeof(StringName[]) ? Variant.Type.PackedStringArray
			: type == typeof(NodePath[]) ? Variant.Type.PackedStringArray
			: type == typeof(Rid[]) ? Variant.Type.Array
			: type == typeof(StringName) ? Variant.Type.StringName
			: type == typeof(NodePath) ? Variant.Type.NodePath
			: type == typeof(Rid) ? Variant.Type.Rid
			: type == typeof(Godot.Collections.Dictionary) ? Variant.Type.Dictionary
			: type == typeof(Godot.Collections.Array) ? Variant.Type.Array
			: type == typeof(Variant) ? Variant.Type.Nil
			: typeof(GodotObject).IsAssignableFrom(type) ? Variant.Type.Object
			: type.IsValueType && typeof(Enum).IsAssignableFrom(type) ? Variant.Type.Int
			: Variant.Type.Nil;

		public static Variant GetDefault(Variant.Type type)
			=> type switch
			{
				Variant.Type.Nil => Variant.NULL,
				Variant.Type.Bool => default(bool),
				Variant.Type.Int => default(int),
				Variant.Type.Float => default(float),
				Variant.Type.String => "",
				Variant.Type.Vector2 => default(Vector2),
				Variant.Type.Vector2I => default(Vector2I),
				Variant.Type.Rect2 => default(Rect2),
				Variant.Type.Rect2I => default(Rect2I),
				Variant.Type.Vector3 => default(Vector3),
				Variant.Type.Vector3I => default(Vector3I),
				Variant.Type.Transform2D => default(Transform2D),
				Variant.Type.Vector4 => default(Vector4),
				Variant.Type.Vector4I => default(Vector4I),
				Variant.Type.Plane => default(Plane),
				Variant.Type.Quaternion => default(Quaternion),
				Variant.Type.Aabb => default(Aabb),
				Variant.Type.Basis => default(Basis),
				Variant.Type.Transform3D => default(Transform3D),
				Variant.Type.Projection => default(Projection),
				Variant.Type.Color => default(Color),
				Variant.Type.StringName => "",
				Variant.Type.NodePath => "",
				Variant.Type.Rid => default(Rid),
				Variant.Type.Object => Variant.NULL,
				Variant.Type.Callable => default(Callable),
				Variant.Type.Signal => default(Signal),
				Variant.Type.Dictionary => new Godot.Collections.Dictionary(),
				Variant.Type.Array => new Godot.Collections.Array(),
				Variant.Type.PackedByteArray => new byte[0],
				Variant.Type.PackedInt32Array => new int[0],
				Variant.Type.PackedInt64Array => new long[0],
				Variant.Type.PackedFloat32Array => new float[0],
				Variant.Type.PackedFloat64Array => new double[0],
				Variant.Type.PackedStringArray => new string[0],
				Variant.Type.PackedVector2Array => new Vector2[0],
				Variant.Type.PackedVector3Array => new Vector3[0],
				Variant.Type.PackedColorArray => new Color[0],
				Variant.Type.PackedVector4Array => new Vector4[0],
				_ => Variant.NULL
			};
	}

	extension <[MustBeVariant] T>(T self)
	{
		public bool IsVariantCompatible()
			=> typeof(T) == typeof(bool)
				|| typeof(T) == typeof(char)
				|| typeof(T) == typeof(sbyte)
				|| typeof(T) == typeof(short)
				|| typeof(T) == typeof(int)
				|| typeof(T) == typeof(long)
				|| typeof(T) == typeof(byte)
				|| typeof(T) == typeof(ushort)
				|| typeof(T) == typeof(uint)
				|| typeof(T) == typeof(ulong)
				|| typeof(T) == typeof(float)
				|| typeof(T) == typeof(double)
				|| typeof(T) == typeof(Vector2)
				|| typeof(T) == typeof(Vector2I)
				|| typeof(T) == typeof(Rect2)
				|| typeof(T) == typeof(Rect2I)
				|| typeof(T) == typeof(Transform2D)
				|| typeof(T) == typeof(Vector3)
				|| typeof(T) == typeof(Vector3I)
				|| typeof(T) == typeof(Basis)
				|| typeof(T) == typeof(Quaternion)
				|| typeof(T) == typeof(Transform3D)
				|| typeof(T) == typeof(Projection)
				|| typeof(T) == typeof(Vector4)
				|| typeof(T) == typeof(Vector4I)
				|| typeof(T) == typeof(Aabb)
				|| typeof(T) == typeof(Color)
				|| typeof(T) == typeof(Plane)
				|| typeof(T) == typeof(Callable)
				|| typeof(T) == typeof(Signal)
				|| typeof(T) == typeof(string)
				|| typeof(T) == typeof(byte[])
				|| typeof(T) == typeof(int[])
				|| typeof(T) == typeof(long[])
				|| typeof(T) == typeof(float[])
				|| typeof(T) == typeof(double[])
				|| typeof(T) == typeof(string[])
				|| typeof(T) == typeof(Vector2[])
				|| typeof(T) == typeof(Vector3[])
				|| typeof(T) == typeof(Vector4[])
				|| typeof(T) == typeof(Color[])
				|| typeof(T) == typeof(StringName[])
				|| typeof(T) == typeof(NodePath[])
				|| typeof(T) == typeof(Rid[])
				|| typeof(T) == typeof(StringName)
				|| typeof(T) == typeof(NodePath)
				|| typeof(T) == typeof(Rid)
				|| typeof(T) == typeof(Godot.Collections.Dictionary)
				|| typeof(T) == typeof(Godot.Collections.Array)
				|| typeof(T) == typeof(Variant)
				|| typeof(GodotObject).IsAssignableFrom(typeof(T))
				|| typeof(T).IsValueType
					&& typeof(Enum).IsAssignableFrom(typeof(T))
					&& Unsafe.SizeOf<T>() is 1 or 2 or 4 or 8;
		public Variant ToVariant()
		{
			try
			{
				return Variant.From(self);
			}
			catch(Exception e)
			{
				GD.PushError(e);
				return Variant.GetDefault(Variant.Typeof<T>());
			}
		}
		public string ToJsonString(string indent = "", bool sortKeys = true, bool fullPrecision = false)
		{
			try
			{
				return Variant.From(self).ToJsonString(indent, sortKeys, fullPrecision);
			}
			catch(Exception e)
			{
				GD.PushError(e);
				return e.ToString().ToJsonString();
			}
		}
	}

	extension (Variant.Type self)
	{
		public bool IsNumericType()
			=> self == Variant.Type.Int || self == Variant.Type.Float;

		public bool IsArrayType()
			=> self == Variant.Type.Array
			|| self == Variant.Type.PackedByteArray
			|| self == Variant.Type.PackedInt32Array
			|| self == Variant.Type.PackedInt64Array
			|| self == Variant.Type.PackedFloat32Array
			|| self == Variant.Type.PackedFloat64Array
			|| self == Variant.Type.PackedStringArray
			|| self == Variant.Type.PackedColorArray
			|| self == Variant.Type.PackedVector2Array
			|| self == Variant.Type.PackedVector3Array
			|| self == Variant.Type.PackedVector4Array;

		public bool IsNullableType()
			=> self.IsConvertibleTo(Variant.Type.Nil);

		/// <summary>
		/// Algorithm replicated from https://github.com/godotengine/godot/blob/master/core/variant/variant.cpp#L192
		/// </summary>
		public bool IsConvertibleTo(Variant.Type other, bool strict = false)
			=> self == other
			|| other == Variant.Type.Nil
			|| self == Variant.Type.Nil && other == Variant.Type.Object
			|| other switch
			{
				Variant.Type.Bool => strict
					? self.IsAnyOf([Variant.Type.Int, Variant.Type.Float])
					: self.IsAnyOf([Variant.Type.Int, Variant.Type.Float, Variant.Type.String]),
				Variant.Type.Int => strict
					? self.IsAnyOf([Variant.Type.Bool, Variant.Type.Float])
					: self.IsAnyOf([Variant.Type.Bool, Variant.Type.Float, Variant.Type.String]),
				Variant.Type.Float => strict
					? self.IsAnyOf([Variant.Type.Bool, Variant.Type.Int])
					: self.IsAnyOf([Variant.Type.Bool, Variant.Type.Int, Variant.Type.String]),
				Variant.Type.String => strict
					? self.IsAnyOf([Variant.Type.NodePath, Variant.Type.StringName])
					: self != Variant.Type.Object,
				Variant.Type.Vector2 => self == Variant.Type.Vector2I,
				Variant.Type.Vector2I => self == Variant.Type.Vector2,
				Variant.Type.Rect2 => self == Variant.Type.Rect2I,
				Variant.Type.Rect2I => self == Variant.Type.Rect2,
				Variant.Type.Transform2D => self == Variant.Type.Transform3D,
				Variant.Type.Vector3 => self == Variant.Type.Vector3I,
				Variant.Type.Vector3I => self == Variant.Type.Vector3,
				Variant.Type.Vector4 => self == Variant.Type.Vector4I,
				Variant.Type.Vector4I => self == Variant.Type.Vector4,
				Variant.Type.Quaternion => self == Variant.Type.Basis,
				Variant.Type.Basis => self == Variant.Type.Quaternion,
				Variant.Type.Transform3D => self.IsAnyOf([Variant.Type.Transform2D, Variant.Type.Quaternion, Variant.Type.Basis, Variant.Type.Projection]),
				Variant.Type.Projection => self == Variant.Type.Transform3D,
				Variant.Type.Color => self.IsAnyOf([Variant.Type.String, Variant.Type.Int]),
				Variant.Type.Rid => self == Variant.Type.Object,
				Variant.Type.StringName => self == Variant.Type.String,
				Variant.Type.NodePath => self == Variant.Type.String,
				Variant.Type.Array => self.IsArrayType(),
				_ => other.IsArrayType() && self == Variant.Type.Array,
			};

		private bool IsAnyOf(Variant.Type[] types)
			=> types.Contains(self);
	}
}
