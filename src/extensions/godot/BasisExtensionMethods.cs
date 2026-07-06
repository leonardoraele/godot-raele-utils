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
		public Basis RotateToward(Vector3 target, Vector3 up, Radians deltaAngle)
		{
			Vector3 targetDirection = target.Normalized();
			if (targetDirection.IsZeroApprox())
				return self;
			Vector3 right = targetDirection.Cross(up);
			if (right.IsZeroApprox())
				return self;
			up = right.Cross(targetDirection);
			return self.RotateToward(new Basis(right, up, targetDirection * -1), deltaAngle);
		}
		public Basis RotateToward(Basis target, Radians deltaAngle)
		{
			float angle = self.Forward.AngleTo(target.Forward);
			float weight = (deltaAngle / angle).AtMost(1);
			return self.Slerp(target, weight);
			// Vector3 newForward = self.Forward.RotateToward(target.Forward, deltaAngle, self.Up).Normalized();
			// Vector3 newRight = newForward.Cross(self.Up).DefaultIfZero(Vector3.Right).Normalized();
			// Vector3 newUp = newForward.Rotated(newRight, Mathf.Pi / 2).Normalized();
			// return new Basis(newRight, newUp, newForward * -1);
		}

		public bool IsOrthonormalized()
			=> self.Right.IsNormalized()
				&& self.Up.IsNormalized()
				&& self.Back.IsNormalized()
				&& self.Right.Dot(self.Up).IsZeroApprox()
				&& self.Right.Dot(self.Back).IsZeroApprox()
				&& self.Up.Dot(self.Back).IsZeroApprox();
	}
}
