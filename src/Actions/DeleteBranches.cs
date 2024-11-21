using System.Text.RegularExpressions;
using AzdoTool.Visitors;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdoTool.Actions;

internal class DeleteBranches(VisitorNode node) : BatchOrUnitAction<GitBranchStats>(node, "branch", "delete")
{
	protected override bool Filter(GitBranchStats item, string filter) => Regex.IsMatch(item.Name, filter, RegexOptions.IgnoreCase);

	protected override async Task ActionAsync(IEnumerable<GitBranchStats> items)
	{
		var repository = Node.Ancestor<Repository>();
		await repository.UpdateGitRefsAsync(items.Select(item => new GitRefUpdate
		{
			Name = $"refs/heads/{item.Name}",
			OldObjectId = item.Commit.CommitId,
			NewObjectId = new string('0', 40)
		}));
	}
}
