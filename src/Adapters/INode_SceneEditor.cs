using Godot;

namespace Raele.GodotUtils.Adapters;

public partial interface INode
{
	public void SetOwner(Node owner);
	public Node GetOwner();
	public void SetSceneFilePath(string sceneFilePath);
	public string GetSceneFilePath();
	public void SetEditorDescription(string editorDescription);
	public string GetEditorDescription();
	public void SetUniqueNameInOwner(bool enable);
	public bool IsUniqueNameInOwner();
	public void UpdateConfigurationWarnings();
	public void SetSceneInstanceLoadPlaceholder(bool loadPlaceholder);
	public bool GetSceneInstanceLoadPlaceholder();
	public void SetEditableInstance(Node node, bool isEditable);
	public bool IsEditableInstance(Node node);
	public void SetDisplayFolded(bool fold);
	public bool IsDisplayedFolded();
}
