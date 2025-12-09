using Godot;

namespace Raele.GodotUtils.General;

[Tool]
public partial class MimicTransform2D : Node2D
{
	// -----------------------------------------------------------------------------------------------------------------
	// STATICS
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public Node2D? Target;
	[Export(PropertyHint.Flags, "Position:1,Rotation:2,Scale:4,Skew:8")] public uint Fields = 30;
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
				this.GlobalScale = this.Target.GlobalScale;
			}
			if ((this.Fields & 8) != 0)
			{
				this.GlobalSkew = this.Target.GlobalSkew;
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
			if ((this.Fields & 8) != 0)
			{
				this.Skew = this.Target.Skew;
			}
		}
	}
}
