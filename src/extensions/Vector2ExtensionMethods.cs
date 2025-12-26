using Godot;

namespace Raele.GodotUtils.Extensions;

public static class Vector2ExtensionMethods
{
	extension(Vector2 self)
	{
		public void Deconstruct(out float x, out float y)
		{
			x = self.X;
			y = self.Y;
		}
	}
}
