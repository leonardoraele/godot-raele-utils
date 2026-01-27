using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Messages;

public partial class CommandResultEvent<T> : Message where T : Command<T>
{
	public required T Command { get; init; }

	protected override IEnumerable<KeyValuePair<Variant, Variant>> _GetDetails()
		=> base._GetDetails()
			.Append(new(nameof(this.Command), this.Command.MessageId.ToString()));
}
