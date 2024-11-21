using System.Text.RegularExpressions;
using AzdoTool.Nodes;
using AzdoTool.Visitors;
using Humanizer;
using Spectre.Console;

namespace AzdoTool.Actions;

internal abstract class BatchOrUnitAction<TEntity>(VisitorNode node, string entity, string action) : Node
{
	protected List<TEntity> Targets { get; } = [];
	protected VisitorNode Node => node;

	public override string Title => node is BatchVisitorNode<TEntity>
			? $"[red]>> Batch {action} {entity.Pluralize()} by name[/]"
			: $"[red]>> {action.Pascalize()}[/]";

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			switch (node)
			{
				case UnitVisitorNode<TEntity> unit:
					result.Add(new Confirm($"Confirm to {action} '{unit.Title}'",
						() => ActionAsync([unit.Item]), unit.Parent!));
					break;
				case BatchVisitorNode<TEntity> batch:
					var targetsCount = Targets.Count;
					var itemsCount = batch.AllItems.Count;
					result.Add(new Confirm($"Confirm to {action} [red]{targetsCount}[/] {entity.ToQuantity(targetsCount, ShowQuantityAs.None)}, leaving [green]{itemsCount - targetsCount}[/]/{itemsCount} {entity.ToQuantity(itemsCount, ShowQuantityAs.None)}",
						() => ActionAsync(Targets), batch));
					break;
			}

			result.Add(new GoBack(node));
			result.Add(new Exit());
			return result;
		}
	}

	public override Task<INode> ExecuteAsync()
	{
		if (node is UnitVisitorNode<TEntity>)
			return base.ExecuteAsync();

		if (node is not BatchVisitorNode<TEntity> batch)
			throw new InvalidOperationException("Invalid node type");

		var filter = AnsiConsole.Ask<string>("Name filter (regex): ");
		AnsiConsole.Clear();

		try
		{
			Targets.Clear();
			Targets.AddRange(batch.AllItems.Where(item => Filter(item, filter)));
		}
		catch (RegexParseException ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}. [/]");
			return node.ExecuteAsync();
		}

		return base.ExecuteAsync();
	}

	protected abstract bool Filter(TEntity item, string filter);

	protected abstract Task ActionAsync(IEnumerable<TEntity> items);
}
