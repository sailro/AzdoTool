using AzdoTool.Actions;
using AzdoTool.Nodes;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Spectre.Console;

namespace AzdoTool.Visitors;

internal class Repository(Repositories repositories, GitRepository teamRepository) : UnitVisitorNode<GitRepository>
{
	public override GitRepository Item => teamRepository;
	public override VisitorNode Parent => repositories;

	public override string Title => teamRepository.Name.EscapeMarkup();

	public override List<INode> Children => [
		new Branches(this),
		new PullRequests(this),
		new Tags(this),
		new GoBack(repositories),
		new Exit()
	];

	public async Task UpdateGitRefsAsync(IEnumerable<GitRefUpdate> items)
	{
		var project = Ancestor<Project>();
		var organization = project.Ancestor<Organization>();

		await organization.ExecuteClientAsync<GitHttpClient>(async client =>
		{
			await client.UpdateRefsAsync(items, project.Item.Id, Item.Id);
		});
	}
}
