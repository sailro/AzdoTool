using AzdTool.Nodes;

namespace AzdTool.Actions;

internal abstract class BaseGoToNode(string title, INode node) : Node
{
	public override string Title => title;

	public override Task<INode> ExecuteAsync()
	{
		return node.ExecuteAsync();
	}
}
