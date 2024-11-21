using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdTool.Visitors;

internal class Tags(Repository repository) : BatchVisitorNode<GitRef>
{
	public override List<GitRef> AllItems { get; } = [];
	public override VisitorNode Parent => repository;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(gitref => new Tag(this, gitref)));
			result.Add(new DeleteTags(this));
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
				AllItems.AddRange((await client.GetTagRefsAsync(repository.Item.Id))
					.OrderBy(gitRef => gitRef.Name));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
