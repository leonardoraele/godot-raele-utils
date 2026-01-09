using System.Threading;
using Godot;

namespace Raele.GodotUtils;

public partial class GodotCancellationController : GodotObject
{
	public CancellationTokenSource TokenSource { get; private set; } = new();
	public GodotCancellationToken Token => field ??= new() { BackingToken = this.TokenSource.Token };
	public bool IsCancellationRequested => this.TokenSource.IsCancellationRequested;
	public void Cancel() => this.TokenSource.Cancel();
}
