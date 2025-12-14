#if TOOLS
using Godot;
using Raele.GodotUtils.General;
using Raele.GodotUtils.StateMachine;

[Tool]
public partial class GodotUtilsCSPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		this.AddCustomType(
			nameof(MimicTransform2D),
			nameof(Node2D),
			GD.Load<Script>("res://addons/godot-utils-csharp/src/nodes/MimicTransform2D.cs"),
			null
		);
		this.AddCustomType(
			nameof(MimicTransform3D),
			nameof(Node3D),
			GD.Load<Script>("res://addons/godot-utils-csharp/src/nodes/MimicTransform3D.cs"),
			null
		);
		this.AddCustomType(
			nameof(StateMachineNode),
			nameof(Node),
			GD.Load<Script>("res://addons/godot-utils-csharp/src/StateMachine/StateMachine.cs"),
			null
		);
	}

	public override void _ExitTree()
	{
		this.RemoveCustomType(nameof(MimicTransform2D));
		this.RemoveCustomType(nameof(MimicTransform3D));
		this.RemoveCustomType(nameof(StateMachineNode));
	}
}
#endif
