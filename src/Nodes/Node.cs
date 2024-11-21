using AzdTool.Actions;
using Spectre.Console;

namespace AzdTool.Nodes;

internal abstract class Node : INode
{
	public virtual string Title => GetType().Name;
	public virtual List<INode> Children => [new Exit()];

	public virtual Task<INode> ExecuteAsync()
	{
		var result = AnsiConsole
			.Prompt(new SelectionPrompt<INode>()
				.Title(GetSelectionTitle())
				.PageSize(20)
				.UseConverter(prompt => prompt.Title)
				.AddChoices(Children));

		return result.ExecuteAsync();
	}

	protected virtual string GetSelectionTitle() => Title;
}
