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
      Log.Debug("[SiteMapFactory] BuildSiteMap()");
      var pageLists = new PageListCollection();
      // Process Pages
      var rootNode = ProcessPages(rootPath.Subdirectory(Constants.PAGES), 
        globalSettings, 
        pageLists);
      Log.Debug("[SiteMapFactory] Root node is at " + rootNode.Path);
      rootNode.PageLists = pageLists;
      return rootNode;
    }

    private static Node ProcessPages(DirectoryInfo path, 
      dynamic globalSettings, 
      PageListCollection pageLists,
      string sitePath = "", 
      Node parentNode = null)
    {
      var node = new DirectoryNode(path, sitePath);
      // Process each file
      foreach (var file in path.GetFiles("*.md"))
      {
        var pageNode = new PageNode(file, globalSettings);
        if (!String.IsNullOrEmpty(pageNode.ListName))
        {
          pageLists.AddPage(pageNode);
        }
        node.AddChild(pageNode);
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
        var subNode = ProcessPages(dir, 
          globalSettings, 
          pageLists,
          dir.Name);
        node.AddChild(subNode);
      }
      return node;
    }
  }
}
