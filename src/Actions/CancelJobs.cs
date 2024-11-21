using System.Text.RegularExpressions;
using AzdTool.Extensions;
using AzdTool.Visitors;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using AZBuild = Microsoft.TeamFoundation.Build.WebApi.Build;

namespace AzdTool.Actions;

internal class CancelJobs(VisitorNode node) : BatchOrUnitAction<TaskAgentJobRequest>(node, "job", "cancel")
{
	protected override bool Filter(TaskAgentJobRequest item, string filter) => Regex.IsMatch(item.GetFormattedTitle(), filter, RegexOptions.IgnoreCase);

	protected override async Task ActionAsync(IEnumerable<TaskAgentJobRequest> items)
	{
		var organization = Node.Ancestor<Organization>();

		// Cancelling a job is the same as cancelling the underlying build
		await CancelBuilds.CancelBuildsAsync(organization, items.Select(taskAgentJobRequest =>
			new AZBuild
			{
				Id = taskAgentJobRequest.Owner.Id,
				Project = new TeamProjectReference { Id = taskAgentJobRequest.ScopeId } // Scope of the pipeline; matches the project ID
			}
		));
	}
}
