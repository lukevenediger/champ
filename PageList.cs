using champ.Map;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  [DebuggerDisplay("{Name} - {this.Count} pages")]
  public class PageList : List<PageNode>
  {
    public string Name { get; private set; }

    public PageList(string name)
    {
      Name = name;
    }

    public IEnumerable<PageNode> TakeNewest(int count = -1)
    {
      PageNode[] pages;
      if (count != -1)
      {
        pages = this.OrderByDescending(p => p.DateStamp).Take(count).ToArray();
      }
      else
      {
        pages = this.OrderByDescending(p => p.DateStamp).ToArray();
      }
      return pages;
    }
  }
}
