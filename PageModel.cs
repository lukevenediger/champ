using champ.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  /// <summary>
  /// Holds information that's passed to the page template
  /// during the render step.
  /// </summary>
  public class PageModel
  {
    public PageNode Page { get; private set; }
    public string Content { get; private set; }
    public PageListCollection Lists { get; private set; }

    public PageModel(PageNode page, string content, PageListCollection lists)
    {
      Page = page;
      Content = content;
      Lists = lists;
    }
  }
}
