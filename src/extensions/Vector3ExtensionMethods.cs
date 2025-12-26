using Godot;

namespace Raele.GodotUtils.Extensions;

public static class Vector3ExtensionMethods
{
	extension(Vector3 self)
	{
		public void Deconstruct(out float x, out float y, out float z)
		{
			x = self.X;
			y = self.Y;
			z = self.Z;
		}
	}
}
