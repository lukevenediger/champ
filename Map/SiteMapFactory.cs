using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ.Map
{
  public static class SiteMapFactory
  {
    public static Node BuildSiteMap(DirectoryInfo rootPath, dynamic globalSettings)
    {
      // Process Pages
      return ProcessPages(rootPath.Subdirectory(Constants.PAGES), globalSettings);
    }

    private static Node ProcessPages(DirectoryInfo path, dynamic globalSettings, string sitePath = "Home", Node parentNode = null)
    {
      var node = new DirectoryNode(path, sitePath);
      // Process each file
      foreach (var file in path.GetFiles("*.md"))
      {
        node.AddChild(new PageNode(file, globalSettings));
      }
      // Check for a syndication file
      FileInfo syndicationFile;
      if (path.TryGetFile("rss.xml", out syndicationFile))
      {
        node.AddChild(new SyndicationNode());
      }
      // Process each subdirectory
      foreach (var dir in path.GetDirectories())
      {
        var subNode = ProcessPages(dir, globalSettings, dir.Name);
        node.AddChild(subNode);
      }
      return node;
    }
  }
}
