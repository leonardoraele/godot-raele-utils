using System;
using System.Collections.Generic;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

public partial class Message : GodotObject
{
	//=================================================================================================================
	// FIELDS
	//=================================================================================================================

	public readonly Guid MessageId = Guid.NewGuid();
	// TODO Is this necessary?
	// public readonly DateTimeOffset Timestamp = DateTimeOffset.UtcNow;

	//=================================================================================================================
	// VIRTUALS
	//=================================================================================================================

	protected virtual IEnumerable<KeyValuePair<Variant, Variant>> _GetDetails() => [];
	protected virtual void _Publish()
		=> MessageBus.Singleton.Dispatch(this);

	//=================================================================================================================
	// METHODS
	//=================================================================================================================

	public override string ToString()
		=> $"{this.GetType().Name} {Json.Stringify(this._GetDetails().ToGodotDictionary())}";
	public void Publish()
		=> this._Publish();
}
