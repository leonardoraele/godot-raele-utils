namespace Raele.GodotUtils.Input;

public enum InputTestEnum : sbyte
{
	InputIsJustPressed = 24,
	InputIsPressed = 48,
	InputIsJustReleased = 72,
	InputIsReleased = 96,
	None = 0,
}

public static class InputTestEnumExtensions
{
	extension (InputTestEnum actionType)
	{
		public bool Test(string actionName)
			=> !string.IsNullOrWhiteSpace(actionName)
				&& actionType switch
				{
					InputTestEnum.InputIsJustPressed => Godot.Input.IsActionJustPressed(actionName),
					InputTestEnum.InputIsPressed => Godot.Input.IsActionPressed(actionName),
					InputTestEnum.InputIsJustReleased => Godot.Input.IsActionJustReleased(actionName),
					InputTestEnum.InputIsReleased => !Godot.Input.IsActionPressed(actionName),
					_ => false,
				};
	}
}
