using System.Text;
using AzdoTool.Nodes;

namespace AzdoTool.Visitors;

internal abstract class VisitorNode : Node
{
	public abstract VisitorNode? Parent { get; }

	public T Ancestor<T>() where T : VisitorNode
	{
		var current = Parent;
		while (current != null)
		{
			if (current is T parent)
				return parent;

			current = current.Parent;
		}

		throw new ArgumentException($"No parent of type {typeof(T).Name} found");
	}

	protected override string GetSelectionTitle()
	{
		var stack = new Stack<string>();

		var current = this;
		while (current != null)
		{
			stack.Push(current.Title);
			current = current.Parent;
		}

		var builder = new StringBuilder();
		while (stack.TryPop(out var title))
		{
			builder.Append(title);
			builder.Append(" > ");
		}

		return builder.ToString();
	}
}
