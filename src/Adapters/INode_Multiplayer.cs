using System;
using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public void SetMultiplayerAuthority(int id, bool recursive = true);
	public int GetMultiplayerAuthority();
	public bool IsMultiplayerAuthority();
	public MultiplayerApi GetMultiplayer();
	public void RpcConfig(StringName method, Variant config);
	public Variant GetNodeRpcConfig();
	public Error Rpc(StringName method, params Variant[] args);
	public Error Rpc(StringName method, ReadOnlySpan<Variant> args);
	public Error RpcId(long peerId, StringName method, params Variant[] args);
	public Error RpcId(long peerId, StringName method, ReadOnlySpan<Variant> args);
	public Variant GetRpcConfig();
}
