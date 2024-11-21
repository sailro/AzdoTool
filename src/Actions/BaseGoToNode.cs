using AzdoTool.Nodes;

namespace AzdoTool.Actions;

internal abstract class BaseGoToNode(string title, INode node) : Node
{
	public override string Title => title;

	public override Task<INode> ExecuteAsync()
	{
		return node.ExecuteAsync();
	}
}
