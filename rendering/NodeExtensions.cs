using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Leafshade;

public static class NodeExtensions
{

    public static IEnumerable<Node> AllChildren(this Node root)
    {
        Stack<Node> stack = new();
        stack.Push(root);

        while (stack.Count > 0)
        {
            Node current = stack.Pop();
            yield return current;
            foreach (Node child in current.GetChildren())
            {
                stack.Push(child);
            }
        }
    }

    public static IEnumerable<Node> GetSiblings(this Node root)
    {
        return root.GetParent().GetChildren().Where(x => x != root);
    }

    public static void QueueFreeChildren(this Node root)
    {
        foreach (Node n in root.GetChildren())
        {
            n.QueueFree();
        }
    }

    public static void AddChildren(this Node root, IEnumerable<Node> children)
    {
        foreach (Node n in children)
        {
            root.AddChild(n);
        }
    }

}
