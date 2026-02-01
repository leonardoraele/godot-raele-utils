using System;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class TweenExtensionMethods
{
	extension(Tween self)
	{
		public Tween AddCallbackTweener(Action callback)
		{
			self.TweenCallback(Callable.From(callback));
			return self;
		}
		public Tween AddCallbackTweener(Callable callback)
		{
			self.TweenCallback(callback);
			return self;
		}
		public Tween AddCallbackTweener(Callable callback, Action<CallbackTweener> builder)
		{
			builder(self.TweenCallback(callback));
			return self;
		}
		public Tween AddIntervalTweener(double time)
		{
			self.TweenInterval(time);
			return self;
		}
		public Tween AddIntervalTweener(double time, Action<IntervalTweener> builder)
		{
			builder(self.TweenInterval(time));
			return self;
		}
		public Tween AddMethodTweener(Callable method, Variant from, Variant to, double duration)
		{
			self.TweenMethod(method, from, to, duration);
			return self;
		}
		public Tween AddMethodTweener(
			Callable method,
			Variant from,
			Variant to,
			double duration,
			Action<MethodTweener> builder
		)
		{
			builder(self.TweenMethod(method, from, to, duration));
			return self;
		}
		public Tween AddPropertyTweener(GodotObject @object, NodePath property, Variant finalVal, double duration)
		{
			self.TweenProperty(@object, property, finalVal, duration);
			return self;
		}
		public Tween AddPropertyTweener(
			GodotObject @object,
			NodePath property,
			Variant finalVal,
			double duration,
			Action<PropertyTweener> builder
		)
		{
			builder(self.TweenProperty(@object, property, finalVal, duration));
			return self;
		}
		public Tween AddSubtweenTweener(Tween subtween)
		{
			self.TweenSubtween(subtween);
			return self;
		}
		public Tween AddSubtweenTweener(Tween subtween, Action<SubtweenTweener> builder)
		{
			builder(self.TweenSubtween(subtween));
			return self;
		}
	}
}
