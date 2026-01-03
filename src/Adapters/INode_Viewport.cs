using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public Window GetWindow();
	public Window GetLastExclusiveWindow();
	public Viewport GetViewport();
}
