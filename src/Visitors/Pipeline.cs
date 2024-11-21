using AzdTool.Actions;
using AzdTool.Nodes;
using Spectre.Console;
using AZPipeline = Microsoft.Azure.Pipelines.WebApi.Pipeline;

namespace AzdTool.Visitors;

internal class Pipeline(Pipelines pipelines, AZPipeline pipeline) : UnitVisitorNode<AZPipeline>
{
	public override AZPipeline Item => pipeline;
	public override string Title => pipeline.Name.EscapeMarkup();
	public override VisitorNode Parent => pipelines;

	public override List<INode> Children => [
		new Builds(this),
		new GoBack(pipelines),
		new Exit()
	];
}
