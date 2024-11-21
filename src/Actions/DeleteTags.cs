using System.Text.RegularExpressions;
using AzdoTool.Visitors;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzdoTool.Actions;

internal class DeleteTags(VisitorNode node) : BatchOrUnitAction<GitRef>(node, "tag", "delete")
{
	protected override bool Filter(GitRef item, string filter) => Regex.IsMatch(item.Name, filter, RegexOptions.IgnoreCase);

	protected override async Task ActionAsync(IEnumerable<GitRef> items)
	{
		var repository = Node.Ancestor<Repository>();
		await repository.UpdateGitRefsAsync(items.Select(item => new GitRefUpdate
		{
			Name = item.Name,
			OldObjectId = item.ObjectId,
			NewObjectId = new string('0', 40)
		}));
	}
}
