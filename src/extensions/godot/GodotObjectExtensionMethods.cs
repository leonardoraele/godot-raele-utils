using System;
using System.Collections.Generic;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class GodotObjectExtensionMethods
{
	private const double DEFAULT_DEBOUNCE_DELAY_SECONDS = 0.200d;
	private const double DEFAULT_THROTTLE_DELAY_SECONDS = 0.200d;

	extension(GodotObject self)
	{
		public bool IsInstanceValid() => GodotObject.IsInstanceValid(self);

		/// <summary>
		/// Disconnects a signal from a callable if it is currently connected.
		/// </summary>
		public void DisconnectSafe(StringName signalName, Callable callable)
		{
			if (self.IsConnected(signalName, callable))
				self.Disconnect(signalName, callable);
		}

		// public void Connect(
		// 	StringName signal,
		// 	Callable callable,
		// 	GodotCancellationToken cancellationToken,
		// 	params ConnectFlags[] connectFlags
		// )
		// 	=> self.Connect(signal, callable, cancellationToken.BackingToken, connectFlags);

		// public void Connect(
		// 	StringName signal,
		// 	Callable callable,
		// 	System.Threading.CancellationToken cancellationToken,
		// 	params ConnectFlags[] connectFlags
		// )
		// {
		// 	uint flags = connectFlags.Aggregate(0u, (acc, flag) => acc | (uint) flag);
		// 	self.Connect(signal, callable, flags);
		// 	cancellationToken.Register(() => self.Disconnect(signal, callable));
		// }

		public Variant CallSafe(StringName methodName, params Variant[] args)
		{
			if (!self.IsInstanceValid())
				return Variant.NULL;
			try
				{ return self.Call(methodName, args); }
			catch (Exception e)
			{
				GD.PushError(e);
				return Variant.NULL;
			}
		}

		public T? CallSafe<[MustBeVariant] T>(T defaultReturn, StringName methodName, params Variant[] args)
		{
			if (!self.IsInstanceValid())
				return defaultReturn;
			try
				{ return self.Call(methodName, args).As<T>(); }
			catch (Exception e)
			{
				GD.PushError(e);
				return defaultReturn;
			}
		}

		public T? CallSafe<[MustBeVariant] T>(Func<T> defaultReturnFactory, StringName methodName, params Variant[] args)
		{
			if (!self.IsInstanceValid())
				return defaultReturnFactory();
			try
				{ return self.Call(methodName, args).As<T>(); }
			catch (Exception e)
			{
				GD.PushError(e);
				return defaultReturnFactory();
			}
		}

		public void CallDebouncedRealTime(TimeSpan delay, StringName methodName, params Variant[] args)
			=> self._CallDebounced(methodName, delay.TotalSeconds, ignoreTimeScale: true, args);
		public void CallDebounced(TimeSpan delay, StringName methodName, params Variant[] args)
			=> self._CallDebounced(methodName, delay.TotalSeconds, ignoreTimeScale: false, args);
		public void CallDebouncedRealTime(double delaySeconds, StringName methodName, params Variant[] args)
			=> self._CallDebounced(methodName, delaySeconds, ignoreTimeScale: true, args);
		public void CallDebounced(double delaySeconds, StringName methodName, params Variant[] args)
			=> self._CallDebounced(methodName, delaySeconds, ignoreTimeScale: false, args);
		public void CallDebouncedRealTime(StringName methodName, params Variant[] args)
			=> self._CallDebounced(methodName, DEFAULT_DEBOUNCE_DELAY_SECONDS, ignoreTimeScale: true, args);
		public void CallDebounced(StringName methodName, params Variant[] args)
			=> self._CallDebounced(methodName, DEFAULT_DEBOUNCE_DELAY_SECONDS, ignoreTimeScale: false, args);
		/// <summary>
		/// Calls a method on the GodotObject after a delay, resetting the delay if called again before the timer
		/// elapses.
		/// </summary>
		private void _CallDebounced(
			StringName methodName,
			double delaySeconds = DEFAULT_DEBOUNCE_DELAY_SECONDS,
			bool ignoreTimeScale = false,
			params Variant[] args
		)
		{
			string metaProp = $"{nameof(Raele)}_{nameof(GodotUtils)}_{nameof(_CallDebounced)}";
			Tween? tween = self.HasMeta(metaProp)
				? self.GetMeta(metaProp).AsGodotObject() as Tween
				: null;
			tween?.Kill();
			tween = self is Node node
				? node.CreateTween()
				: Engine.GetSceneTree().CreateTween();
			self.SetMeta(metaProp, tween);
			tween.SetIgnoreTimeScale(ignoreTimeScale)
				.SetProcessMode(Tween.TweenProcessMode.Physics)
				.AddIntervalTweener(delaySeconds)
				.AddCallbackTweener(Callable.From(() =>
				{
					tween.Kill();
					if (self.IsInstanceValid())
					{
						self.RemoveMeta(metaProp);
						self.Call(methodName, args);
					}
				}));
		}

		public void CallThrottledRealTime(TimeSpan delay, StringName methodName, params Variant[] args)
			=> self._CallThrottled(methodName, delay.TotalSeconds, ignoreTimeScale: true, args);
		public void CallThrottled(TimeSpan delay, StringName methodName, params Variant[] args)
			=> self._CallThrottled(methodName, delay.TotalSeconds, ignoreTimeScale: false, args);
		public void CallThrottledRealTime(double delaySeconds, StringName methodName, params Variant[] args)
			=> self._CallThrottled(methodName, delaySeconds, ignoreTimeScale: true, args);
		public void CallThrottled(double delaySeconds, StringName methodName, params Variant[] args)
			=> self._CallThrottled(methodName, delaySeconds, ignoreTimeScale: false, args);
		public void CallThrottledRealTime(StringName methodName, params Variant[] args)
			=> self._CallThrottled(methodName, DEFAULT_THROTTLE_DELAY_SECONDS, ignoreTimeScale: true, args);
		public void CallThrottled(StringName methodName, params Variant[] args)
			=> self._CallThrottled(methodName, DEFAULT_THROTTLE_DELAY_SECONDS, ignoreTimeScale: false, args);

		/// <summary>
		/// Calls a method on the GodotObject after a delay, ignoring subsequent calls until the timer elapses.
		/// </summary>
		private void _CallThrottled(
			StringName methodName,
			double delaySeconds = DEFAULT_THROTTLE_DELAY_SECONDS,
			bool ignoreTimeScale = false,
			params Variant[] args
		)
		{
			string metaProp = $"{nameof(Raele)}_{nameof(GodotUtils)}_{nameof(_CallDebounced)}";
			Tween? tween = self.HasMeta(metaProp)
				? self.GetMeta(metaProp).AsGodotObject() as Tween
				: null;
			if (tween?.IsValid() == true)
				return;
			tween = self is Node node
				? node.CreateTween()
				: Engine.GetSceneTree().CreateTween();
			self.SetMeta(metaProp, tween);
			tween.SetIgnoreTimeScale(ignoreTimeScale)
				.SetProcessMode(Tween.TweenProcessMode.Physics)
				.AddIntervalTweener(delaySeconds)
				.AddCallbackTweener(Callable.From(() =>
				{
					tween.Kill();
					if (self.IsInstanceValid())
						self.RemoveMeta(metaProp);
				}));
			self.Call(methodName, args);
		}
	}

	extension <T>(T self) where T : GodotObject
	{
		public T WithMeta(StringName name, Variant value)
		{
			self.SetMeta(name, value);
			return self;
		}
	}
}
