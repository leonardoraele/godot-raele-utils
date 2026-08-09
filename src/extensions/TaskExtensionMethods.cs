using System;
using System.Threading.Tasks;

namespace Raele.GodotUtils.Extensions;

public static class TaskExtensionMethods
{
	extension(Task self)
	{
		public Task OnCompletedSuccessfully(Action action)
		{
			return self.ContinueWith(task =>
			{
				if (!task.IsCompletedSuccessfully)
					action();
			});
		}

		public Task OnFaulted(Action<Exception> action)
		{
			return self.ContinueWith(task =>
			{
				if (task.IsFaulted && task.Exception != null)
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
}
