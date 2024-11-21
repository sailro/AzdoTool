namespace AzdTool.Nodes;

internal interface INode
{
	public string Title { get; }
	public Task<INode> ExecuteAsync();
}
