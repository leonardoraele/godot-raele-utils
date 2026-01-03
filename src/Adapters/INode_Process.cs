using static Godot.Node;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public void SetPhysicsProcess(bool enable);
	public double GetPhysicsProcessDeltaTime();
	public bool IsPhysicsProcessing();
	public double GetProcessDeltaTime();
	public void SetProcess(bool enable);
	public void SetProcessPriority(int priority);
	public int GetProcessPriority();
	public void SetPhysicsProcessPriority(int priority);
	public int GetPhysicsProcessPriority();
	public bool IsProcessing();
	public void SetProcessInput(bool enable);
	public bool IsProcessingInput();
	public void SetProcessShortcutInput(bool enable);
	public bool IsProcessingShortcutInput();
	public void SetProcessUnhandledInput(bool enable);
	public bool IsProcessingUnhandledInput();
	public void SetProcessUnhandledKeyInput(bool enable);
	public bool IsProcessingUnhandledKeyInput();
	public void SetProcessMode(ProcessModeEnum mode);
	public ProcessModeEnum GetProcessMode();
	public bool CanProcess();
	public void SetProcessThreadGroup(ProcessThreadGroupEnum mode);
	public ProcessThreadGroupEnum GetProcessThreadGroup();
	public void SetProcessThreadMessages(ProcessThreadMessagesEnum flags);
	public ProcessThreadMessagesEnum GetProcessThreadMessages();
	public void SetProcessThreadGroupOrder(int order);
	public int GetProcessThreadGroupOrder();
	public void SetProcessInternal(bool enable);
	public bool IsProcessingInternal();
	public void SetPhysicsProcessInternal(bool enable);
	public bool IsPhysicsProcessingInternal();
}
