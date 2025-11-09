using Godot;

namespace Raele.GodotUtils.StateMachine;

public partial class StateTransition : Node
{
	// -----------------------------------------------------------------------------------------------------------------
	// STATICS
	// -----------------------------------------------------------------------------------------------------------------

	// public static readonly string MyConstant = "";

	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public StringName Signal = "";
	[Export] public Node? NextState;

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------

	private Node? ParentCache;
	private Callable Callback;

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------



	// -----------------------------------------------------------------------------------------------------------------
	// SIGNALS
	// -----------------------------------------------------------------------------------------------------------------

	// [Signal] public delegate void EventHandler()

	// -----------------------------------------------------------------------------------------------------------------
	// INTERNAL TYPES
	// -----------------------------------------------------------------------------------------------------------------

	// private enum Type {
	// 	Value1,
	// }

	// -----------------------------------------------------------------------------------------------------------------
	// EVENTS
	// -----------------------------------------------------------------------------------------------------------------

	// public override void _EnterTree()
	// {
	// 	base._EnterTree();
	// }

	// public override void _Ready()
	// {
	// 	base._Ready();
	// }

	// public override void _Process(double delta)
	// {
	// 	base._Process(delta);
	// }

	// public override void _PhysicsProcess(double delta)
	// {
	// 	base._PhysicsProcess(delta);
	// }

	public override void _Notification(int what)
	{
		base._Notification(what);
		if (what == Node.NotificationParented)
		{
			this.ParentCache = this.GetParent()!;
			if (this.ParentCache.GetParent() is StateMachine stateMachine && this.NextState != null)
			{
				string nextStateName = this.NextState.Name;
				this.Callback = Callable.From(() => stateMachine.QueueTransition(nextStateName));
				this.ParentCache.Connect(this.Signal, this.Callback);
			}
		}
		else if (what == Node.NotificationUnparented)
		{
			this.ParentCache?.Disconnect(this.Signal, this.Callback);
			this.Callback = Callable.From(() => { });
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------


}
