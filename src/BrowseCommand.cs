using System.ComponentModel;
using AzdoTool.Visitors;
using Azure.Core;
using Azure.Identity;
using Microsoft.VisualStudio.Services.Common;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AzdoTool;

internal sealed class BrowseCommand : AsyncCommand<BrowseCommand.Settings>
{
	internal class Settings : CommandSettings
	{
		[Description("Organization")]
		[CommandArgument(0, "<organization>")]
		public string Organization { get; set; } = string.Empty;

		[Description("Use a PAT instead of interactive browser credential.")]
		[CommandOption("-p|--pat")]
		public string? PersonalAccessToken { get; set; }
	}

	public override async Task<int> ExecuteAsync(CommandContext commandContext, Settings settings)
	{
		try
		{
			var credentials = settings.PersonalAccessToken is not null
				? new VssBasicCredential(string.Empty, settings.PersonalAccessToken)
				: await GetImpersonationCredentialAsync();

			AnsiConsole.Clear();

			var organization = new Organization(settings.Organization, credentials);
			await organization.ExecuteAsync();

			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}. [/]");
			return 1;
		}
	}

	private static async Task<VssCredentials> GetImpersonationCredentialAsync(CancellationToken cancellationToken = default)
	{
		var azureDevOpsAuthScopes = new[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" };

		AnsiConsole.MarkupLine("[grey]Authenticating in your browser...[/]");

		var browserCredential = new InteractiveBrowserCredential();
		var context = new TokenRequestContext(azureDevOpsAuthScopes);
		var accessToken = await browserCredential.GetTokenAsync(context, cancellationToken);
		var vssCredentials = new VssCredentials(new VssBasicCredential(string.Empty, accessToken.Token));

		AnsiConsole.MarkupLine("[grey]Authenticated.[/]");

		return vssCredentials;
	}
}
