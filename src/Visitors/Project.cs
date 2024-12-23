﻿using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Spectre.Console;

namespace AzdTool.Visitors;

internal class Project(Projects projects, TeamProjectReference teamProject) : UnitVisitorNode<TeamProjectReference>
{
	public override TeamProjectReference Item => teamProject;
	public override string Title => teamProject.Name.EscapeMarkup();
	public override VisitorNode Parent => projects;

	public override List<INode> Children => [
		new Pipelines(this),
		new Repositories(this),
		new GoBack(projects),
		new Exit()
	];
}
