using AzdoTool.Actions;
using AzdoTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Spectre.Console;

namespace AzdoTool.Visitors;

internal class Tag(Tags tags, GitRef gitRef) : UnitVisitorNode<GitRef>
{
	public override GitRef Item => gitRef;
	public override string Title => gitRef.Name.EscapeMarkup();
	public override VisitorNode Parent => tags;

	public override List<INode> Children => [
		new DeleteTags(this),
		new GoBack(tags),
		new Exit()
	];
}
