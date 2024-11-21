using System.Text.RegularExpressions;
using AzdTool.Extensions;
using AzdTool.Visitors;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using AZBuild = Microsoft.TeamFoundation.Build.WebApi.Build;

namespace AzdTool.Actions;

internal class CancelBuilds(VisitorNode node) : BatchOrUnitAction<AZBuild>(node, "build", "cancel")
{
	protected override bool Filter(AZBuild item, string filter) => Regex.IsMatch(item.GetFormattedTitle(), filter, RegexOptions.IgnoreCase);

	protected override async Task ActionAsync(IEnumerable<AZBuild> items)
	{
		var organization = Node.Ancestor<Organization>();
		await CancelBuildsAsync(organization, items);
	}

	public static async Task CancelBuildsAsync(Organization organization, IEnumerable<AZBuild> items)
	{
		await organization.ExecuteClientAsync<BuildHttpClient>(async client =>
		{
			foreach (var item in items)
			{
				await client.UpdateBuildAsync(new AZBuild
				{
					Id = item.Id,
					Status = BuildStatus.Cancelling,
					Project = new TeamProjectReference { Id = item.Project.Id }
				});
			}
		});
	}
}
