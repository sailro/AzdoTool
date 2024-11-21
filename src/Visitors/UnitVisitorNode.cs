namespace AzdTool.Visitors;

internal abstract class UnitVisitorNode<TEntity> : VisitorNode
{
	public abstract TEntity Item { get; }
}
