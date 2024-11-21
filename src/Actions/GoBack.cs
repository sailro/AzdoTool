using AzdoTool.Nodes;

namespace AzdoTool.Actions;

internal class GoBack(INode parent) : BaseGoToNode("[grey]>> Go back[/]", parent);
