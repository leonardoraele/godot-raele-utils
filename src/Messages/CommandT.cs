namespace Raele.GodotUtils.Messages;

public abstract partial class Command<T> : Command where T : Command<T>
{
	public abstract CommandResultEvent<T> _Execute();
	public CommandResultEvent<T> Execute()
		=> this._Execute();
}
