using Godot;
using Raele.GodotUtils.Structs;

namespace Raele.GodotUtils.Extensions;

public static class BasisExtensionMethods
{
	extension(Basis self)
	{
		public Vector3 Up => self.Y;
		public Vector3 Down => -self.Y;
		public Vector3 Right => self.X;
		public Vector3 Left => -self.X;
		public Vector3 Forward => -self.Z;
		public Vector3 Back => self.Z;
		public Basis RotateToward(Vector3 target)
			=> self.RotateToward(target, float.PositiveInfinity);
		public Basis RotateToward(Vector3 target, Radians deltaAngle)
		{
			Vector3 newForward = self.Forward.RotateToward(target, deltaAngle, self.Up).Normalized();
			Vector3 newRight = newForward.Cross(self.Up).DefaultIfZero(Vector3.Right).Normalized();
			Vector3 newUp = newForward.Rotated(newRight, Mathf.Pi / 2).Normalized();
			return new Basis(newRight, newUp, newForward * -1);
		}
	}
}
