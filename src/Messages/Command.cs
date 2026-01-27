namespace Raele.GodotUtils.Messages;

public abstract partial class Command : Message
{
	protected virtual void _Execute() {}
	public void Execute()
		=> this._Execute();
}
