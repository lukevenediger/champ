using champ.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xipton.Razor;

namespace champ
{
  public abstract class ChampTemplateBase : TemplateBase
  {
    public virtual string ResolveUrl(string filename)
    {
      var node = Model.page as PageNode;
      var prefix = "";
      for (var counter = 0; counter < node.Depth; counter++)
      {
        prefix += "../";
      }
      return "<link rel=\"stylesheet\" href=\"" + prefix + filename + "\" />";
    }
  }

  public abstract class ChampTemplateBase<TModel> : ChampTemplateBase, ITemplate<TModel>
  {
    public new TModel Model
    {
      get { return GetOrCreateModel<TModel>(); } 
    }
  }
}
