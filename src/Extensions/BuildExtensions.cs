using System.Text;
using Microsoft.TeamFoundation.Build.WebApi;
using Spectre.Console;

namespace AzdoTool.Extensions;

internal static class BuildExtensions
{
	public static string GetFormattedTitle(this Build build)
	{
		var builder = new StringBuilder();
		builder.Append($"Build {build.BuildNumber} ");

		if (build.Result.HasValue)
			builder.Append(build.Result.Value.ToString());
		else if (build.Status.HasValue)
			builder.Append(build.Status.Value.ToString());

		return builder.ToString().Trim().EscapeMarkup();
	}
}
