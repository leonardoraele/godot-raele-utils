using Godot;

namespace Raele.GodotUtils.General;

[Tool]
public partial class MimicTransform3D : Node3D
{
	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public Node3D? Target;
	[Export(PropertyHint.Flags, "LocalPosition:1,GlobalPosition:2,Rotation:4,Scale:8")] public uint Fields;

	// -----------------------------------------------------------------------------------------------------------------
	// LIFECYCLE HANDLERS
	// -----------------------------------------------------------------------------------------------------------------

	public override void _Process(double delta)
	{
		if (this.Target == null)
		{
			return;
		}

		if ((this.Fields & 3) == 1)
		{
			this.Position = this.Target.Position;
		}
		else if ((this.Fields & 3) == 2)
		{
			this.GlobalPosition = this.Target.GlobalPosition;
		}
		else if ((this.Fields & 3) == 3)
		{
			GD.PrintErr($"{nameof(MimicTransform3D)}: Cannot mimic both LocalPosition and GlobalPosition at the same time. Please turn on either one or the other, but not both.");
			this.Fields &= ~(uint) 3;
		}
		if ((this.Fields & 4) != 0)
		{
			this.Rotation = this.Target.Rotation;
		}
		if ((this.Fields & 8) != 0)
		{
			this.Scale = this.Target.Scale;
		}
	}
}
