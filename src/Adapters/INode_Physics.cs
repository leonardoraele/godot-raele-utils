using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public void SetPhysicsInterpolationMode(Node.PhysicsInterpolationModeEnum mode);
	public Node.PhysicsInterpolationModeEnum GetPhysicsInterpolationMode();
	public bool IsPhysicsInterpolated();
	public bool IsPhysicsInterpolatedAndEnabled();
	public void ResetPhysicsInterpolation();
}
