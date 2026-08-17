using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class TaskExtensionMethods
{
	extension(Task self)
	{
		public Task OnCompletedSuccessfully(Action action)
		{
			return self.ContinueWith(task =>
			{
				if (task.IsCompletedSuccessfully)
					action();
			});
		}

		public Task OnFaulted(Action<Exception> action)
		{
			return self.ContinueWith(task =>
			{
				if (!task.IsFaulted)
					return;
				System.Diagnostics.Debug.Assert(task.Exception != null, "Task.Exception should not be null when the task is faulted.");
				action(task.Exception);
			});
		}

		public Task OnCanceled(Action action)
		{
			return self.ContinueWith(task =>
			{
				if (task.IsCanceled)
					action();
			});
		}
	}

	extension (CancellationToken self)
	{
		public SignalAwaiter ToSignal()
		{
			GodotObject source = new();
			source.AddUserSignal("cancelled");
			self.Register(() => source.EmitSignal("cancelled"));
			return source.ToSignal(source, "cancelled");
		}
	}
}
