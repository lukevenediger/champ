using champ.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  public class PageListCollection : KeyedCollection<string, PageList>
  {
    public PageListCollection()
      : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    protected override string GetKeyForItem(PageList item)
    {
      return item.Name;
    }

    internal void AddPage(PageNode pageNode)
    {
      if (!this.Items.Any(p => p.Name == pageNode.ListName))
      {
        this.Add(new PageList(pageNode.ListName));
      }
      // We assume that a page only gets processed once,
      // so no need to check for duplicates
      this[pageNode.ListName].Add(pageNode);
    }
  }
}
