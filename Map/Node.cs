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
    public Node Root { get { return GetRootNode(); } }
    public PageListCollection PageLists
    {
      get { return GetPageLists(); } 
      set { SetPageLists(value); } 
    }

    private string _sitePath;
    private PageListCollection _pageLists;

    public Node(string sitePath)
    {
      Children = new List<Node>();
      _sitePath = sitePath;
    }

    public void AddChild(Node node)
    {
      node.Parent = this;
      Children.Add(node);
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

    public virtual int GetDepth()
    {
      if (this.Parent != null)
      {
        return this.Parent.GetDepth();
      }
      else
      {
        return 0;
      }
    }

    private Node GetRootNode()
    {
      if (this.Parent != null)
      {
        return this.Parent.GetRootNode();
      }
      else
      {
        // I am the root node
        return this;
      }
    }

    private PageListCollection GetPageLists()
    {
      if (this.Parent != null)
      {
        return this.Parent.GetPageLists();
      }
      else
      {
        if (_pageLists == null)
        {
          _pageLists = new PageListCollection();
        }
        return _pageLists;
      }
    }

    private void SetPageLists(PageListCollection pageLists)
    {
      if (this.Parent != null)
      {
        throw new Exception("Page lists can only be updated on the root node.");
      }
      _pageLists = pageLists;
    }
  }
}
