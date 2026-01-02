using System.Linq;
using Godot;

namespace Raele.GodotUtils.Adapters;

public interface ISignalEmitter
{
	//------------------------------------------------------------------------------------------------------------------
	// ABSTRACTS
	//------------------------------------------------------------------------------------------------------------------

	public ISignalEmitter AsSignalEmitter();
	public GodotObject AsGodotObject();

	/// <summary>
	/// <para>Returns <see langword="true"/> if the given <paramref name="signal"/> name exists in the object.</para>
	/// <para><b>Note:</b> In C#, <paramref name="signal"/> must be in snake_case when referring to built-in Godot signals. Prefer using the names exposed in the <c>SignalName</c> class to avoid allocating a new <see cref="Godot.StringName"/> on each call.</para>
	/// </summary>
	public bool HasSignal(StringName signal);

	/// <summary>
	/// <para>Returns the list of existing signals as an <see cref="Godot.Collections.Array"/> of dictionaries.</para>
	/// <para><b>Note:</b> Due of the implementation, each <see cref="Godot.Collections.Dictionary"/> is formatted very similarly to the returned values of <see cref="Godot.GodotObject.GetMethodList()"/>.</para>
	/// </summary>
	public Godot.Collections.Array<Godot.Collections.Dictionary> GetSignalList();

	/// <summary>
	/// <para>Returns an <see cref="Godot.Collections.Array"/> of connections for the given <paramref name="signal"/> name. Each connection is represented as a <see cref="Godot.Collections.Dictionary"/> that contains three entries:</para>
	/// <para>- <c>signal</c> is a reference to the <see cref="Godot.Signal"/>;</para>
	/// <para>- <c>callable</c> is a reference to the connected <see cref="Godot.Callable"/>;</para>
	/// <para>- <c>flags</c> is a combination of <see cref="Godot.GodotObject.ConnectFlags"/>.</para>
	/// </summary>
	public Godot.Collections.Array<Godot.Collections.Dictionary> GetSignalConnectionList(StringName signal);

	/// <summary>
	/// <para>Returns an <see cref="Godot.Collections.Array"/> of signal connections received by this object. Each connection is represented as a <see cref="Godot.Collections.Dictionary"/> that contains three entries:</para>
	/// <para>- <c>signal</c> is a reference to the <see cref="Godot.Signal"/>;</para>
	/// <para>- <c>callable</c> is a reference to the <see cref="Godot.Callable"/>;</para>
	/// <para>- <c>flags</c> is a combination of <see cref="Godot.GodotObject.ConnectFlags"/>.</para>
	/// </summary>
	public Godot.Collections.Array<Godot.Collections.Dictionary> GetIncomingConnections();

	/// <summary>
	/// <para>Connects a <paramref name="signal"/> by name to a <paramref name="callable"/>. Optional <paramref name="flags"/> can be also added to configure the connection's behavior (see <see cref="Godot.GodotObject.ConnectFlags"/> constants).</para>
	/// <para>A signal can only be connected once to the same <see cref="Godot.Callable"/>. If the signal is already connected, this method returns <see cref="Godot.Error.InvalidParameter"/> and generates an error, unless the signal is connected with <see cref="Godot.GodotObject.ConnectFlags.ReferenceCounted"/>. To prevent this, use <see cref="Godot.GodotObject.IsConnected(StringName, Callable)"/> first to check for existing connections.</para>
	/// <para><b>Note:</b> If the <paramref name="callable"/>'s object is freed, the connection will be lost.</para>
	/// <para><b>Note:</b> In GDScript, it is generally recommended to connect signals with <c>Signal.connect</c> instead.</para>
	/// <para><b>Note:</b> This operation (and all other signal related operations) is thread-safe.</para>
	/// </summary>
	public Error Connect(StringName signal, Callable callable, uint flags = 0);

	/// <summary>
	/// <para>Disconnects a <paramref name="signal"/> by name from a given <paramref name="callable"/>. If the connection does not exist, generates an error. Use <see cref="Godot.GodotObject.IsConnected(StringName, Callable)"/> to make sure that the connection exists.</para>
	/// </summary>
	public void Disconnect(StringName signal, Callable callable);

	/// <summary>
	/// <para>Returns <see langword="true"/> if a connection exists between the given <paramref name="signal"/> name and <paramref name="callable"/>.</para>
	/// <para><b>Note:</b> In C#, <paramref name="signal"/> must be in snake_case when referring to built-in Godot signals. Prefer using the names exposed in the <c>SignalName</c> class to avoid allocating a new <see cref="Godot.StringName"/> on each call.</para>
	/// </summary>
	public bool IsConnected(StringName signal, Callable callable);

	/// <summary>
	/// <para>Returns <see langword="true"/> if any connection exists on the given <paramref name="signal"/> name.</para>
	/// <para><b>Note:</b> In C#, <paramref name="signal"/> must be in snake_case when referring to built-in Godot methods. Prefer using the names exposed in the <c>SignalName</c> class to avoid allocating a new <see cref="Godot.StringName"/> on each call.</para>
	/// </summary>
	public bool HasConnections(StringName signal);

	/// <summary>
	/// <para>If set to <see langword="true"/>, the object becomes unable to emit signals. As such, <see cref="Godot.GodotObject.EmitSignal(StringName, Variant[])"/> and signal connections will not work, until it is set to <see langword="false"/>.</para>
	/// </summary>
	public void SetBlockSignals(bool enable);

	/// <summary>
	/// <para>Returns <see langword="true"/> if the object is blocking its signals from being emitted. See <see cref="Godot.GodotObject.SetBlockSignals(bool)"/>.</para>
	/// </summary>
	public bool IsBlockingSignals();

	//------------------------------------------------------------------------------------------------------------------
	// INTERNAL TYPES
	//------------------------------------------------------------------------------------------------------------------

	//------------------------------------------------------------------------------------------------------------------
	// CONCRETES
	//------------------------------------------------------------------------------------------------------------------

	public ConnectedSignal[] GetSignalConnectionListParsed(StringName signal)
		=> this.GetSignalConnectionList(signal)
			.Select(ConnectedSignal.FromDictionary)
			.ToArray();

	public ConnectedSignal[] GetIncomingConnectionsParsed()
		=> this.GetIncomingConnections()
			.Select(ConnectedSignal.FromDictionary)
			.ToArray();

	public GodotMethodInfo[] GetSignalListParsed()
		=> this.GetSignalList()
			.Select(GodotMethodInfo.FromDictionary)
			.ToArray();
}
