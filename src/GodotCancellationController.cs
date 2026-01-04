using System.Threading;
using Godot;

namespace Raele.GodotUtils;

public partial class GodotCancellationController : GodotObject
{
	public CancellationTokenSource Source { get; private set; } = new();
	public GodotCancellationToken Token => field ??= new() { Token = this.Source.Token };
	public bool IsCancellationRequested => this.Source.IsCancellationRequested;
	public void Cancel() => this.Source.Cancel();
}
