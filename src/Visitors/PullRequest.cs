using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Spectre.Console;

namespace AzdTool.Visitors;

internal class PullRequest(PullRequests pullRequests, GitPullRequest gitPullRequest) : UnitVisitorNode<GitPullRequest>
{
	public override GitPullRequest Item => gitPullRequest;
	public override string Title => gitPullRequest.Title.EscapeMarkup();
	public override VisitorNode Parent => pullRequests;

	public override List<INode> Children => [
		new AbandonPullRequests(this),
		new GoBack(pullRequests),
		new Exit()
	];
}
