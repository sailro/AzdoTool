using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdTool.Visitors;

internal class PullRequests(Repository repository) : BatchVisitorNode<GitPullRequest>
{
	public override List<GitPullRequest> AllItems { get; } = [];
	public override VisitorNode Parent => repository;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(pr => new PullRequest(this, pr)));
			result.Add(new AbandonPullRequests(this));
			result.Add(new Refresh(this));
			result.Add(new GoBack(repository));
			result.Add(new Exit());

			return result;
		}
	}

	public override async Task<INode> ExecuteAsync()
	{
		await FetchNotifyAsync(async ctx =>
		{
			var project = Ancestor<Project>();
			var organization = project.Ancestor<Organization>();

			await organization.ExecuteClientAsync<GitHttpClient>(async client =>
			{
				var criteria = new GitPullRequestSearchCriteria { Status = PullRequestStatus.Active };
				AllItems.Clear();
				AllItems.AddRange((await client.GetPullRequestsAsync(project.Item.Id, repository.Item.Id, criteria))
					.OrderByDescending(pr => pr.CreationDate));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
