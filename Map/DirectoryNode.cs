using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ.Map
{
  public class DirectoryNode : Node
  {
    public DirectoryInfo Path { get; private set; }
    private string _sitePath;

    public DirectoryNode(DirectoryInfo path, string sitePath)
      :base(sitePath)
    {
      Path = path;
      _sitePath = sitePath;
    }

    public override string ToString()
    {
      return Parent == null ? _sitePath : (Parent.ToString() + _sitePath + "/");
    }

    public override int GetDepth()
    {
      if (this.Parent != null)
      {
        return this.Parent.GetDepth() + 1;
      }
      else
      {
        return 0;
      }
    }
  }
}
