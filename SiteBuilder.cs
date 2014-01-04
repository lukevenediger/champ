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
  /// <summary>
  /// Builds a website using templates, static files and content
  /// </summary>
  public class SiteBuilder
  {
    private static readonly Lazy<RazorMachine> _razor = new Lazy<RazorMachine>(() =>
      new RazorMachine(baseType: typeof(ChampTemplateBase), htmlEncode: false));
    public static RazorMachine Razor { get { return _razor.Value; } }
    private DirectoryInfo _sourcePath;
    private DirectoryInfo _outputPath; 
    private Markdown _markdown;
    private string _defaultTemplate;

    public SiteBuilder(string sourcePath, string outputPath, string defaultTemplate)
    {
      _sourcePath = new DirectoryInfo(sourcePath);
      _outputPath = string.IsNullOrEmpty(outputPath) ? new DirectoryInfo(Path.Combine(sourcePath, Constants.OUTPUT)) : new DirectoryInfo(outputPath);
      _defaultTemplate = defaultTemplate;
      _markdown = new Markdown(new MarkdownOptions() { AutoHyperlink = true });
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
      // Create the output folder if it doesn't exist
      if (!_outputPath.Exists)
      {
        _outputPath.Create();
      }
      // Delete what's in the output folder
      _outputPath.RecursiveDelete();
      // Build the site map
      var rootNode = SiteMapFactory.BuildSiteMap(_sourcePath, globalSettings);
      // Copy static content
      _sourcePath
        .Subdirectory(Constants.STATIC_CONTENT)
        .CopyTo(_outputPath.Subdirectory(Constants.STATIC_CONTENT, true));
      // Copy files that aren't .md files
      _sourcePath
        .Subdirectory(Constants.PAGES)
        .CopyTo(_outputPath, excludedExtensions : new string[] { ".md" });
      // Load templates
      LoadTemplates();
      // Build out pages
      ProcessPages(rootNode);
    }

    private dynamic LoadGlobalSettings()
    {
      dynamic globalSettings = null;
      globalSettings = new BetterExpando();
      // Set the default template if required
      if (!String.IsNullOrEmpty(_defaultTemplate))
      {
        globalSettings.template = _defaultTemplate;
      }
      else
      {
        globalSettings.template = "no_template_defined";
      }
      globalSettings.title = "Title Not Set";
      return globalSettings;
    }

    private void LoadTemplates()
    {
      _sourcePath.Subdirectory(Constants.TEMPLATES)
        .GetFiles("*.cshtml", SearchOption.AllDirectories)
        .ToList()
        .ForEach(file =>
          {
            // Register each template
            Razor.RegisterTemplate(file.Name.ToLower(), file.ReadAllText());
          });
      // Register the default template
      Razor.RegisterTemplate("no_template_defined", "<h1>No template defined!</h1> @Model.Content");
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
        ProcessPageNode(node as PageNode);
      }
      foreach (var child in node.Children)
      {
        ProcessPages(child);                
      }
    }

    private void ProcessPageNode(PageNode page)
    {
      var rawContent = _markdown.Transform(page.GetRawContent());
      var template = Razor.GetTemplate("~/" + page.Template + ".cshtml");
      var pageModel = new PageModel(page, rawContent, page.PageLists);
      var output = Razor.ExecuteContent(template,
        model: pageModel,
        viewbag: page.Properties
      );
      var outputPath = page.GetOutputFileName().Replace(_sourcePath.Subdirectory(Constants.PAGES).FullName, _outputPath.FullName);
      File.WriteAllText(outputPath, output.Result);
    }
  }
}
