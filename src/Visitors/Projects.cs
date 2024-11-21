using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.Core.WebApi;

namespace AzdTool.Visitors;

internal class Projects(Organization organization) : BatchVisitorNode<TeamProjectReference>
{
	public override List<TeamProjectReference> AllItems { get; } = [];
	public override VisitorNode Parent => organization;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(project => new Project(this, project)));
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
			var buffer = new List<TeamProjectReference>();

			await organization.ExecuteClientAsync<ProjectHttpClient>(async client =>
			{
				string? continuationToken = null;
				do
				{
					var projects = await client.GetProjects(continuationToken: continuationToken);
					buffer.AddRange(projects);
					continuationToken = projects.ContinuationToken;

				} while (!string.IsNullOrEmpty(continuationToken));
			});

			AllItems.Clear();
			AllItems.AddRange(buffer
				.Where(project => !project.Name.Contains("disabled", StringComparison.OrdinalIgnoreCase))
				.OrderBy(project => project.Name));

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
