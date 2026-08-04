namespace Raele.GodotUtils.Extensions;

public static class ObjectExtensionMethods
{
	extension<T>(T self)
	{
		// public T With(Action<T> action)
		// {
		// 	action(self);
		// 	return self;
		// }
	}

	extension<T>(T? self) where T : class
	{
		public T AssertNotNull(string? message = null)
		{
			System.Diagnostics.Debug.Assert(self != null, $"Assertion failed: {typeof(T).FullName}.{nameof(AssertNotNull)}(\"{message}\")");
			return self;
		}
	}
}
