using AzdTool.Nodes;

namespace AzdTool.Actions;

internal class GoBack(INode parent) : BaseGoToNode("[grey]>> Go back[/]", parent);
