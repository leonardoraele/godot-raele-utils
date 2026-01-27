using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

public partial class MessageBus : Node
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	public static MessageBus Singleton
	{
		get
		{
			if (field == null)
			{
				field = new();
				Engine.GetSceneTree().Root.AddChild(field);
			}
			return field;
		}
		private set;
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EXPORTS
	//==================================================================================================================

	// [Export] public

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

	public TimeSpan PresentationEventTimeout = TimeSpan.FromSeconds(10);

	private List<PresentationEvent> PresentationEventQueue = [];

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	public bool IsWaitingPresentation => this.PresentationEventQueue.Count != 0;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	[Signal] public delegate void MessagePublishedEventHandler(Message message);
	[Signal] public delegate void BeforeCommandPublishedEventHandler(Command commnad, GodotCancellationController controller);
	[Signal] public delegate void CommandPublishedEventHandler(Command commnad);
	[Signal] public delegate void PresentationSequenceStartedEventHandler();
	[Signal] public delegate void PresentationEventStartedEventHandler(PresentationEvent @event);
	[Signal] public delegate void PresentationEventFinishedEventHandler(PresentationEvent @event);
	[Signal] public delegate void PresentationSequenceFinishedEventHandler();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region INTERNAL TYPES
	//==================================================================================================================

	// public enum Type {
	// 	Value1,
	// }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	// public override string[] _GetConfigurationWarnings()
	// 	=> (base._GetConfigurationWarnings() ?? [])
	// 		.AppendIf(false "This node is not configured correctly. Did you forget to assign a required field?")
	// 		.ToArray();

	// public override void _ValidateProperty(Godot.Collections.Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// 	switch (property["name"].AsString())
	// 	{
	// 		case nameof():
	// 			break;
	// 	}
	// }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public void Dispatch(Message message)
	{
		this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.MessagePublished, message);
	}

	public void DispatchPresentationEvent(PresentationEvent @event)
	{
		this.Dispatch(@event);
		if (!this.IsWaitingPresentation)
		{
			this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.PresentationSequenceStarted);
			this.CallDeferred(MethodName.ProcessPresentationEvent, @event);
		}
		this.PresentationEventQueue.Add(@event);
	}

	private async void ProcessPresentationEvent(PresentationEvent @event)
	{
		this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.PresentationEventStarted, @event);
		try
			{ await @event.Completed.WaitAsync(PresentationEventTimeout); }
		catch (TimeoutException)
			{ GD.PushWarning($"{nameof(MessageBus)}: {nameof(PresentationEvent)} timed out after {PresentationEventTimeout}. Event: {@event}."); }
		this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.PresentationEventFinished, @event);
		this.PresentationEventQueue.RemoveAt(0);
		if (this.PresentationEventQueue.Count > 0)
			this.CallDeferred(MethodName.ProcessPresentationEvent, this.PresentationEventQueue.First());
		else
			this.EmitSignalPresentationSequenceFinished();
	}

	public void DispatchCommand(Command command)
	{
		this.Dispatch(command);
		this.CallDeferred(MethodName.ProcessCommand, command);
	}

	private void ProcessCommand(Command command)
	{
		GodotCancellationController controller = new();
		this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.BeforeCommandPublished, command, controller);
		if (controller.IsCancellationRequested)
			return;
		this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.CommandPublished, command);
		command.Execute();
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
