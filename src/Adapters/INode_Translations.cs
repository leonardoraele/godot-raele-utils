using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public void SetAutoTranslateMode(Node.AutoTranslateModeEnum mode);
	public Node.AutoTranslateModeEnum GetAutoTranslateMode();
	public bool CanAutoTranslate();
	public void SetTranslationDomainInherited();
	public string Atr(string message);
	public string Atr(string message, StringName context);
	public string AtrN(string message, StringName pluralMessage, int n);
	public string AtrN(string message, StringName pluralMessage, int n, StringName context);
}
