using AzdTool.Extensions;
using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace AzdTool.Visitors;

internal class Job(Jobs jobs, TaskAgentJobRequest taskAgentJobRequest) : UnitVisitorNode<TaskAgentJobRequest>
{
	public override TaskAgentJobRequest Item => taskAgentJobRequest;
	public override VisitorNode Parent => jobs;
	public override string Title => taskAgentJobRequest.GetFormattedTitle();

	public override List<INode> Children =>
	[
		new CancelJobs(this),
		new GoBack(jobs),
		new Exit()
	];
}
