using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

/// <summary>
/// A generic message type, in case you don't feel like creating a new class for every message type.
///
/// It has a Type field to identify the type of message, and an Args field for custom data.
/// </summary>
public partial class GenericMessage : Message
{
	public required string Type { get; init; }
	public IReadOnlyDictionary<Variant, Variant> Args { get; init; } = new Dictionary<Variant, Variant>();

	protected override IEnumerable<KeyValuePair<Variant, Variant>> _GetDetails()
		=> base._GetDetails()
			.Concat(this.Args);

	protected override void _Publish()
		=> MessageBus.Singleton.Dispatch(this);
	public override string ToString()
		=> $"{nameof(Message)} \"{this.Type}\" {Json.Stringify(this._GetDetails().ToGodotDictionary())}";
}
