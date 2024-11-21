using Humanizer;
using Spectre.Console;

namespace AzdoTool.Visitors;

internal abstract class BatchVisitorNode<TEntity> : VisitorNode
{
	public abstract List<TEntity> AllItems { get; }

	public override string Title => $"[cyan]{GetType().Name.Humanize()}[/]";

	public virtual async Task FetchNotifyAsync(Func<StatusContext, Task> action)
	{
		await AnsiConsole
			.Status()
			.Spinner(Spinner.Known.Star)
			.SpinnerStyle(Style.Parse("green bold"))
			.StartAsync($"Fetching {Title}...", action);
	}
}
