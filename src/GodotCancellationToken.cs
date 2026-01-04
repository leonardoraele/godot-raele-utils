using System;
using System.Threading;
using Godot;

namespace Raele.GodotUtils;

public partial class GodotCancellationToken : GodotObject
{
	[Signal] public delegate void CancellationRequestedEventHandler();
	public required CancellationToken Token
	{
		get;
		init
		{
			field = value;
			value.Register(() => this.EmitSignal(SignalName.CancellationRequested));
		}
	}

	public void Register(Action callback)
		=> this.Token.Register(callback);
}
