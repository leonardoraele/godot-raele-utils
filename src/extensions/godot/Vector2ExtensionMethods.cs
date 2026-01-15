using Godot;

namespace Raele.GodotUtils.Extensions;

public static class Vector2ExtensionMethods
{
	extension(Vector2 self)
	{
		public Vector2I AsVector2I()
			=> new Vector2I((int) self.X, (int) self.Y);
		public void Deconstruct(out float x, out float y)
		{
			x = self.X;
			y = self.Y;
		}
	}

	extension(Vector2I self)
	{
		public Vector2 AsVector2()
			=> new Vector2(self.X, self.Y);
		public void Deconstruct(out int x, out int y)
		{
			x = self.X;
			y = self.Y;
		}
	}
}
