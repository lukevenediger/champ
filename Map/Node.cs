using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ.Map
{
  public abstract class Node
  {
    public List<Node> Children { get; private set; }
    public Node Parent { get; private set; }
    public int Depth { get; private set; }
    private string _sitePath;

    public Node(string sitePath)
    {
      Children = new List<Node>();
      _sitePath = sitePath;
      Depth = 0;
    }

    public void AddChild(Node node)
    {
      node.Parent = this;
      Children.Add(node);
      node.Depth = this.Depth + 1;
    }
    
    public override string ToString()
    {
      if (Parent != null)
      {
        return Parent.ToString() + _sitePath;
      }
      else
      {
        return _sitePath;
      }
    }
  }
}
