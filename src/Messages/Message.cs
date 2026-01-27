using System;
using System.Collections.Generic;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

public abstract partial class Message : GodotObject
{
	//=================================================================================================================
	// FIELDS
	//=================================================================================================================

	public readonly Guid MessageId = Guid.NewGuid();
	public readonly DateTimeOffset Timestamp = DateTimeOffset.UtcNow;

	//=================================================================================================================
	// VIRTUALS
	//=================================================================================================================

	protected virtual IEnumerable<KeyValuePair<Variant, Variant>> _GetDetails() => [];

	//=================================================================================================================
	// METHODS
	//=================================================================================================================

	public override string ToString()
		=> $"{this.GetType().Name} {Json.Stringify(this._GetDetails().ToGodotDictionary())}";
	public void Publish()
		=> MessageBus.Singleton.Dispatch(this);
}
