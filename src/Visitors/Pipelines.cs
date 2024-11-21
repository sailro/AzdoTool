using AzdoTool.Actions;
using AzdoTool.Nodes;
using Microsoft.Azure.Pipelines.WebApi;
using AZPipeline = Microsoft.Azure.Pipelines.WebApi.Pipeline;

namespace AzdoTool.Visitors;

internal class Pipelines(Project project) : BatchVisitorNode<AZPipeline>
{
	public override List<AZPipeline> AllItems { get; } = [];
	public override VisitorNode Parent => project;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(pipeline => new Pipeline(this, pipeline)));
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

			await organization.ExecuteClientAsync<PipelinesHttpClient>(async client =>
			{
				AllItems.Clear();
				AllItems.AddRange((await client.ListPipelinesAsync(project.Item.Id))
					.OrderBy(pipeline => pipeline.Name));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
