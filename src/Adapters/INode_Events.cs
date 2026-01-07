using System;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public event Action Ready;
	public event Action Renamed;
	public event Action TreeEntered;
	public event Action TreeExiting;
	public event Action TreeExited;
	public event Action ChildOrderChanged;
	public event Action EditorStateChanged;
}
