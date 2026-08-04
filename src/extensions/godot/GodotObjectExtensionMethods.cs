using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class GodotObjectExtensionMethods
{
	private const double DEFAULT_DEBOUNCE_DELAY_SECONDS = 0.200d;
	private const double DEFAULT_THROTTLE_DELAY_SECONDS = 0.200d;

	private static ConditionalWeakTable<GodotObject, Dictionary<string, Tween>> DebouncedCallsTable = new();
	private static ConditionalWeakTable<GodotObject, Dictionary<string, Tween>> ThrottledCallsTable = new();

	extension(GodotObject self)
	{
		public bool IsInstanceValid() => GodotObject.IsInstanceValid(self);

		public Variant GetMetaOrDefault(StringName name, Variant defaultValue = new())
			=> self.HasMeta(name)
				? self.GetMeta(name)
				: defaultValue;

		public T GetMetaOrDefault<[MustBeVariant] T>(StringName name)
			=> self.HasMeta(name)
				? self.GetMeta(name).As<T>()
				: Variant.GetDefault<T>();

		public T GetMetaOrDefault<[MustBeVariant] T>(StringName name, T defaultValue)
			=> self.HasMeta(name)
				? self.GetMeta(name).As<T>()
				: defaultValue;

		/// <summary>
		/// Disconnects a signal from a callable if it is currently connected.
		/// </summary>
		public void DisconnectSafe(StringName signalName, Callable callable)
		{
			if (self.IsConnected(signalName, callable))
				self.Disconnect(signalName, callable);
		}

		public void ConnectCancellable(
			StringName signal,
			Callable callable,
			GodotCancellationToken cancellationToken
		)
			=> self.ConnectCancellable(signal, callable, cancellationToken, 0);

		public void ConnectCancellable(
			StringName signal,
			Callable callable,
			GodotCancellationToken cancellationToken,
			GodotObject.ConnectFlags connectFlags
		)
		{
			self.Connect(signal, callable, (uint) connectFlags);
			cancellationToken.Register(() => self.Disconnect(signal, callable));
		}

		public Variant CallSafe(StringName methodName, params Variant[] args)
		{
			self.CallSafe(out Variant @return, methodName, args);
			return @return;
		}
		public bool CallSafe<[MustBeVariant] T>(out T @return, StringName methodName, params Variant[] args)
			=> self.CallSafe(out @return, methodName, args);
		public bool CallSafe(out Variant @return, StringName methodName, params Variant[] args)
		{
			try
			{
				@return = self.Call(methodName, args);
				return true;
			}
			catch (Exception e)
			{
				GD.PushError(e);
				@return = default;
				return false;
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
			string methodName,
			double delaySeconds,
			bool ignoreTimeScale,
			Variant[] args
		)
		{
			Dictionary<string, Tween> tweens = DebouncedCallsTable.GetOrCreateValue(self);
			Tween? tween = tweens.GetValueOrDefault(methodName);
			tween?.Kill();
			tweens[methodName] = tween = self is Node node
				? node.CreateTween()
				: Engine.GetSceneTree().CreateTween();
			tween.SetIgnoreTimeScale(ignoreTimeScale)
				.SetProcessMode(ignoreTimeScale ? Tween.TweenProcessMode.Physics : Tween.TweenProcessMode.Idle)
				.AddIntervalTweener(delaySeconds)
				.AddCallbackTweener(Callable.From(() =>
				{
					tween.Kill();
					tweens.Remove(methodName);
					if (self.IsInstanceValid())
						self.Call(methodName, args);
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
			Dictionary<string, Tween> tweens = ThrottledCallsTable.GetOrCreateValue(self);
			Tween? tween = tweens.GetValueOrDefault(methodName);
			if (tween?.IsValid() == true)
				return;
			tweens[methodName] = tween = self is Node node
				? node.CreateTween()
				: Engine.GetSceneTree().CreateTween();
			tween.SetIgnoreTimeScale(ignoreTimeScale)
				.SetProcessMode(ignoreTimeScale ? Tween.TweenProcessMode.Physics : Tween.TweenProcessMode.Idle)
				.AddIntervalTweener(delaySeconds)
				.AddCallbackTweener(Callable.From(() =>
				{
					tween.Kill();
					tweens.Remove(methodName);
				}));
			self.Call(methodName, args);
		}

		public Dictionary<string, Variant> GetMetaAsDictionary()
			=> self.GetMetaList().ToDictionary(meta => meta.ToString(), meta => self.GetMeta(meta));
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
