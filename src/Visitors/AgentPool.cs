using AzdoTool.Actions;
using AzdoTool.Nodes;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Spectre.Console;

namespace AzdoTool.Visitors;

internal class AgentPool(AgentPools agentPools, TaskAgentPool taskAgentPool) : UnitVisitorNode<TaskAgentPool>
{
	public override TaskAgentPool Item => taskAgentPool;
	public override string Title => taskAgentPool.Name.EscapeMarkup();
	public override VisitorNode Parent => agentPools;

	public override List<INode> Children => [
		new Jobs(this),
		new GoBack(agentPools),
		new Exit()
	];

}
