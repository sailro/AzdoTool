using AzdoTool.Nodes;

namespace AzdoTool.Actions;

internal class Exit : Node
{
	public override string Title => "[grey]>> Exit[/]";

	public override Task<INode> ExecuteAsync()
	{
		Environment.Exit(0);
		return Task.FromResult<INode>(null!);
	}
}
