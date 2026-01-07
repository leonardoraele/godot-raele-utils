using Godot;
using static Godot.Node;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public StringName Name { get; set; }
	public bool UniqueNameInOwner { get; set; }
	public string SceneFilePath { get; set; }
	public Node Owner { get; set; }
	public MultiplayerApi Multiplayer { get; }
	public ProcessModeEnum ProcessMode { get; set; }
	public int ProcessPriority { get; set; }
	public int ProcessPhysicsPriority { get; set; }
	public ProcessThreadGroupEnum ProcessThreadGroup { get; set; }
	public int ProcessThreadGroupOrder { get; set; }
	public ProcessThreadMessagesEnum ProcessThreadMessages { get; set; }
	public PhysicsInterpolationModeEnum PhysicsInterpolationMode { get; set; }
	public AutoTranslateModeEnum AutoTranslateMode { get; set; }
	public string EditorDescription { get; set; }
}
