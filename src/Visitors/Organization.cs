using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Spectre.Console;

namespace AzdTool.Visitors;

internal class Organization(string orgId, VssCredentials credentials) : VisitorNode
{
	private VssCredentials Credentials => credentials;
	private Uri Url => new($"https://dev.azure.com/{orgId}");
	public override string Title => orgId.EscapeMarkup();
	public override VisitorNode? Parent => null;

	public override List<INode> Children => [
		new AgentPools(this),
		new Projects(this),
		new Exit()
	];

	public async Task ExecuteClientAsync<T>(Func<T, Task> action) where T : IVssHttpClient
	{
		try
		{
			var connection = new VssConnection(Url, Credentials);
			using var client = connection.GetClient<T>();

			await action(client);
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}. [/]");
		}
	}
}
