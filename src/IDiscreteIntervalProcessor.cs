using System;
using Godot;
using Raele.GodotUtils.Adapters;

namespace Raele.GodotUtils;

/// <summary>
/// Interface for processors that operate over discrete time intervals. For nodes that perform certain processsing over
/// a defined period of time, allowing for start and finish events, as well as tracking active time.
/// </summary>
public interface IDiscreteIntervalProcessor : IGodotObject
{
	public bool IsActive { get; }
	public TimeSpan ActiveTimeSpan { get; }

	public event Action<Variant, GodotCancellationController> EventWillStart;
	public event Action<Variant> EventStarted;
	/// <summary>
	/// Only emitted when Finish() is called, not when the processor stops by itself, e.g. if its max process time has
	/// been reached.
	/// </summary>
	public event Action<Variant, GodotCancellationController> EventWillFinish;
	/// <summary>
	/// Emitted when the processor stops, regardless of Stop() been called or it stopped by itself.
	/// </summary>
	public event Action<Variant> EventFinished;

	/// <summary>
	/// Requests the processor to start.
	/// </summary>
	public void Start(Variant argument);
	/// <summary>
	/// Requests the processor to stop.
	/// </summary>
	public void Finish(Variant reason);
}
