using System.Text.RegularExpressions;
using AzdTool.Visitors;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdTool.Actions;

internal class AbandonPullRequests(VisitorNode node) : BatchOrUnitAction<GitPullRequest>(node, "pull-request", "abandon")
{
	protected override bool Filter(GitPullRequest item, string filter) => Regex.IsMatch(item.Title, filter, RegexOptions.IgnoreCase);

	protected override async Task ActionAsync(IEnumerable<GitPullRequest> items)
	{
		var repository = Node.Ancestor<Repository>();
		var project = repository.Ancestor<Project>();
		var organization = project.Ancestor<Organization>();

		await organization.ExecuteClientAsync<GitHttpClient>(async client =>
		{
			foreach (var item in items)
			{
				await client.UpdatePullRequestAsync(
					new GitPullRequest
					{
						Status = PullRequestStatus.Abandoned
					},
					project.Item.Id,
					repository.Item.Id,
					item.PullRequestId
				);
			}
		});
	}
}
