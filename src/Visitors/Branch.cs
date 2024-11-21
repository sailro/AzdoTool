using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Spectre.Console;

namespace AzdTool.Visitors;

internal class Branch(Branches branches, GitBranchStats gitBranchStats) : UnitVisitorNode<GitBranchStats>
{
	public override GitBranchStats Item => gitBranchStats;
	public override string Title => gitBranchStats.Name.EscapeMarkup();
	public override VisitorNode Parent => branches;

	public override List<INode> Children => [
		new DeleteBranches(this),
		new GoBack(branches),
		new Exit()
	];
}
