using System;
using System.Threading;
using Godot;

namespace Raele.GodotUtils;

public partial class GodotCancellationToken : GodotObject
{
	public static GodotCancellationToken ForNodeExiting(Node treeNode)
	{
		GodotCancellationController controller = new GodotCancellationController();
		treeNode.Connect(Node.SignalName.TreeExiting, Callable.From(controller.Cancel), (uint) ConnectFlags.OneShot);
		return controller.Token;
	}

	[Signal] public delegate void CancellationRequestedEventHandler();
	public required CancellationToken BackingToken
	{
		get;
		init
		{
			field = value;
			value.Register(() => this.EmitSignal(SignalName.CancellationRequested));
		}
	}

	public void Register(Action callback)
		=> this.BackingToken.Register(callback);
}
