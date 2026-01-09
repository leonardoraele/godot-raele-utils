using System;
using Godot;

namespace Raele.GodotUtils.ActivitySystem;

/// <summary>
/// Interface for activity nodes. An activity is a node that can be active or inactive, and performs some processing
/// while active.
///
/// To start the activity, call the <see cref="Start"/> method. To stop it, call the <see cref="Finish"/> method. The
/// virtual methods <see cref="_ActivityWillStart"/>, <see cref="_ActivityStarted"/>, <see cref="_ActivityWillFinish"/>, and
/// <see cref="_ActivityFinished"/> can be overridden to hook into the activity's lifecycle. Additionally activity also emits
/// events when starting and finishing, that others can subscribe to.
///
/// The activity automatically changes its process mode based on its state and the <see cref="ProcessModeWhenActive"/>
/// and <see cref="ProcessModeWhenInactive"/> properties. You can check whether the activity is active and for how long
/// using the <see cref="IsActive"/> and <see cref="ActiveTimeSpan"/> properties.
/// </summary>
public interface IActivity
{
	//==================================================================================================================
	#region STATICS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region ABSTRACTS
	//==================================================================================================================

	public bool IsActive { get; }
	public TimeSpan ActiveTimeSpan { get; }

	public event Action<string, Variant, GodotCancellationController> EventWillStart;
	public event Action<string, Variant> EventStarted;
	public event Action<string, Variant, GodotCancellationController> EventWillFinish;
	public event Action<string, Variant> EventFinished;

	public void Start(string mode = "", Variant argument = new Variant());
	public void Finish(string reason = "", Variant details = new Variant());

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region CONCRETES
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}

public static class IActivityExtensions
{
	public static IActivity AsActivity(this IActivity activity) => activity;
}
