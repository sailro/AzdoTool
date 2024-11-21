using AzdoTool.Actions;
using AzdoTool.Nodes;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
namespace AzdoTool.Visitors;

internal class AgentPools(Organization organization) : BatchVisitorNode<TaskAgentPool>
{
	public override List<TaskAgentPool> AllItems { get; } = [];
	public override VisitorNode Parent => organization;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(task => new AgentPool(this, task)));
			result.Add(new Refresh(this));
			result.Add(new GoBack(organization));
			result.Add(new Exit());

			return result;
		}
	}

	public override async Task<INode> ExecuteAsync()
	{
		await FetchNotifyAsync(async ctx =>
		{
			await organization.ExecuteClientAsync<TaskAgentHttpClient>(async client =>
			{
				AllItems.Clear();
				AllItems.AddRange((await client.GetAgentPoolsAsync())
					.Where(task => !task.IsLegacy.HasValue || !task.IsLegacy.Value)
					.OrderBy(task => task.Name));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
