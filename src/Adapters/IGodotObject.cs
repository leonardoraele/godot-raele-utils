using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface IGodotObject
{
	public static IGodotObject From(GodotObject obj) => (IGodotObject) obj;
	public GodotObject AsGodotObject() => (GodotObject) this;

	public SignalAwaiter ToSignal(GodotObject source, StringName signal);

	public ulong GetInstanceId();

	public void Notification(int what, bool reversed = false);

	public string GetClass();
	public bool IsClass(string @class);

	public void SetScript(Variant script);
	public Variant GetScript();

	public void Free();
	public bool IsQueuedForDeletion();
	public void CancelFree();
}
