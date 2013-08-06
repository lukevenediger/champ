using champ.Map;
using MarkdownSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xipton.Razor;

namespace champ
{
  public class SiteBuilder
  {
    private DirectoryInfo _sourcePath;
    private DirectoryInfo _outputPath; 
    private Markdown _markdown;
    private RazorMachine _razor;

    public SiteBuilder(string sourcePath, string outputPath)
    {
      _sourcePath = new DirectoryInfo(sourcePath);
      _outputPath = string.IsNullOrEmpty(outputPath) ? new DirectoryInfo(Path.Combine(sourcePath, Constants.OUTPUT)) : _outputPath;
      _markdown = new Markdown();
      _razor = new RazorMachine(baseType: typeof(ChampTemplateBase), htmlEncode: false);
    }

    public void Run()
    {
      /*
       * 1. delete the contents of the Output folder
       * 2. copy all static content to the Output folder
       * 3. generate pages 
      */
     
      // Load global settings
      var globalSettings = LoadGlobalSettings();
      // Delete what's in the output folder
      _outputPath.RecursiveDelete();
      var rootNode = SiteMapFactory.BuildSiteMap(_sourcePath, globalSettings);
      // Copy static content
      _sourcePath
        .Subdirectory(Constants.STATIC_CONTENT)
        .CopyTo(_outputPath.Subdirectory(Constants.STATIC_CONTENT, true));
      // Load templates
      LoadTemplates();
      // Build out pages
      ProcessPages(rootNode);
    }

    private dynamic LoadGlobalSettings()
    {
      FileInfo settingsFile;
      if (_sourcePath.TryGetFile("settings.*", out settingsFile))
      {
        return settingsFile.GetProperties();
      }
      return new BetterExpando();
    }

    private void LoadTemplates()
    {
      _sourcePath.Subdirectory(Constants.TEMPLATES)
        .GetFiles("*.cshtml", SearchOption.AllDirectories)
        .ToList()
        .ForEach(file =>
          {
            // Register each template
            _razor.RegisterTemplate(file.Name, file.ReadAllText());
          });
    }

    private void ProcessPages(Node node)
    {
      if (node is DirectoryNode)
      {
        var directory = node as DirectoryNode;
        var destinationPath = directory.Path.FullName.Replace(_sourcePath.Subdirectory(Constants.PAGES).FullName, _outputPath.FullName);
        if (!Directory.Exists(destinationPath)) { Directory.CreateDirectory(destinationPath); }
      }
      else if (node is PageNode)
      {
        var page = node as PageNode;
        var raw = _markdown.Transform(page.GetRawContent());
        var template = _razor.GetTemplate("~/" + page.Template + ".cshtml");
        var output = _razor.ExecuteContent(template, model: new { page = page, Content = raw });
        var outputPath = page.GetOutputFileName().Replace(_sourcePath.Subdirectory(Constants.PAGES).FullName, _outputPath.FullName);
        File.WriteAllText(outputPath, output.Result);
      }
      foreach (var child in node.Children)
      {
        ProcessPages(child);                
      }
    }
  }
}
