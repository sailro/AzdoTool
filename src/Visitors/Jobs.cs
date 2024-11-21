using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace AzdTool.Visitors;

internal class Jobs(AgentPool agentPool) : BatchVisitorNode<TaskAgentJobRequest>
{
	public override List<TaskAgentJobRequest> AllItems { get; } = [];
	public override VisitorNode Parent => agentPool;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(job => new Job(this, job)));
			result.Add(new CancelJobs(this));
			result.Add(new Refresh(this));
			result.Add(new GoBack(agentPool));
			result.Add(new Exit());

			return result;
		}
	}

	public override async Task<INode> ExecuteAsync()
	{
		await FetchNotifyAsync(async ctx =>
		{
			var organization = Ancestor<Organization>();

			await organization.ExecuteClientAsync<TaskAgentHttpClient>(async client =>
			{

				AllItems.Clear();
				AllItems.AddRange((await client.GetAgentRequestsForAgentsAsync(agentPool.Item.Id))
					.Where(task => task.FinishTime == null)
					.OrderByDescending(task => task.QueueTime));

				ctx.Refresh();
			});
		});

		return await base.ExecuteAsync();
	}
}
