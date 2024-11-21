using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdTool.Visitors;

internal class Repositories(Project project) : BatchVisitorNode<GitRepository>
{
	public override List<GitRepository> AllItems { get; } = [];
	public override VisitorNode Parent => project;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(repository => new Repository(this, repository)));
			result.Add(new Refresh(this));
			result.Add(new GoBack(project));
			result.Add(new Exit());

			return result;
		}
	}

	public override async Task<INode> ExecuteAsync()
	{
		await FetchNotifyAsync(async ctx =>
		{
			var organization = Ancestor<Organization>();

			await organization.ExecuteClientAsync<GitHttpClient>(async client =>
			{
				AllItems.Clear();
				AllItems.AddRange((await client.GetRepositoriesAsync(project.Item.Name))
					.Where(repository => !repository.IsDisabled.HasValue || !repository.IsDisabled.Value)
					.OrderBy(repository => repository.Name));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
