using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Raele.GodotUtils;

public partial class GodotCancellationToken : GodotObject
{
	public static GodotCancellationToken None => field ??= new() { BackingToken = CancellationToken.None };
	public static GodotCancellationToken Cancelled => field ??= new() { BackingToken = new CancellationToken(true) };
	public static GodotCancellationToken From(CancellationToken token)
		=> new() { BackingToken = token };
	public static GodotCancellationToken WhenNodeExitingTree(Node treeNode)
	{
		GodotCancellationController controller = new GodotCancellationController();
		treeNode.Connect(Node.SignalName.TreeExiting, Callable.From(controller.Cancel), (uint) ConnectFlags.OneShot);
		return controller.Token;
	}
	public static GodotCancellationToken WhenTaskCompletes(Task task)
	{
		GodotCancellationController controller = new GodotCancellationController();
		_ = task.ContinueWith(_ => controller.Cancel());
		return controller.Token;
	}
	public static GodotCancellationToken WhenSignalEmitted(GodotObject subject, StringName signalName)
	{
		GodotCancellationController controller = new GodotCancellationController();
		subject.Connect(signalName, Callable.From(controller.Cancel), (uint) ConnectFlags.OneShot);
		return controller.Token;
	}

	public required CancellationToken BackingToken
	{
		get;
		init
		{
			field = value;
			value.Register(() => this.EmitSignal(SignalName.CancellationRequested));
		}
	}

	[Signal] public delegate void CancellationRequestedEventHandler();

	public void Register(Action callback)
		=> this.BackingToken.Register(callback);
}
