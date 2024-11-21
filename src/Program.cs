using AzdoTool;
using Spectre.Console.Cli;

var app = new CommandApp<BrowseCommand>();

app.Configure(config =>
{
	config
		.AddCommand<BrowseCommand>("browse")
		.WithDescription("Browse Organization");
});

return app.Run(args);
