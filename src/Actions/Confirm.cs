using AzdTool.Nodes;

namespace AzdTool.Actions;

internal class Confirm(string prompt, Func<Task> action, Node node) : BaseGoToNode(prompt, node)
{
	public override async Task<INode> ExecuteAsync()
	{
		await action();
		return await node.ExecuteAsync();
	}
}
