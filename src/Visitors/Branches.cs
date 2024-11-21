using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdTool.Visitors;

internal class Branches(Repository repository) : BatchVisitorNode<GitBranchStats>
{
	public override List<GitBranchStats> AllItems { get; } = [];
	public override VisitorNode Parent => repository;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(branch => new Branch(this, branch)));
			result.Add(new DeleteBranches(this));
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
				AllItems.Clear();
				AllItems.AddRange((await client.GetBranchesAsync(project.Item.Id, repository.Item.Id))
					.OrderBy(branch => branch.Name));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
