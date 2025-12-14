using Godot;

namespace Raele.GodotUtils.General;

[Tool]
public partial class MimicTransform3D : Node3D
{
	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public Node3D? Target;
	[Export(PropertyHint.Flags, "Position:1,Rotation:2,Scale:4")] public uint Fields;
	[Export] public bool UseGlobals;

	// -----------------------------------------------------------------------------------------------------------------
	// LIFECYCLE HANDLERS
	// -----------------------------------------------------------------------------------------------------------------

	public override void _Process(double delta)
	{
		if (this.Target == null)
		{
			return;
		}
		if (this.UseGlobals)
		{
			if ((this.Fields & 1) != 0)
			{
				this.GlobalPosition = this.Target.GlobalPosition;
			}
			if ((this.Fields & 2) != 0)
			{
				this.GlobalRotation = this.Target.GlobalRotation;
			}
			if ((this.Fields & 4) != 0)
			{
				// TODO Godot has no GlobalScale property for Node3D
			}
		}
		else
		{
			if ((this.Fields & 1) != 0)
			{
				this.Position = this.Target.Position;
			}
			if ((this.Fields & 2) != 0)
			{
				this.Rotation = this.Target.Rotation;
			}
			if ((this.Fields & 4) != 0)
			{
				this.Scale = this.Target.Scale;
			}
		}
	}
}
