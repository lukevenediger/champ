using champ.DebugHelper;
using champ.Map;
using MarkdownSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xipton.Razor;
using Xipton.Razor.Core;

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
    private int _brokenPageCount;

    public SiteBuilder(string sourcePath, string outputPath, string defaultTemplate)
    {
      _sourcePath = new DirectoryInfo(sourcePath);
      _outputPath = string.IsNullOrEmpty(outputPath) ? new DirectoryInfo(Path.Combine(sourcePath, Constants.OUTPUT)) : new DirectoryInfo(outputPath);
      _defaultTemplate = defaultTemplate;
      Log.Debug("[SiteBuilder] SourcePath: " + _sourcePath.FullName);
      Log.Debug("[SiteBuilder] OutputPath: " + _outputPath.FullName);
      _markdown = new Markdown(new MarkdownOptions() { AutoHyperlink = true });
    }

    public int Run()
    {
      Log.Debug("[SiteBuilder] Run(): " + _sourcePath.FullName);
      _brokenPageCount = 0;
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
        Log.Debug("[SiteBuilder] Creating output path because it doesn't exist.");
        _outputPath.Create();
      }
      // Delete what's in the output folder
      Log.Debug("[SiteBuilder] Delete everything in the output folder.");
      _outputPath.RecursiveDelete();
      // Build the site map
      var rootNode = SiteMapFactory.BuildSiteMap(_sourcePath, globalSettings);
      // Copy static content
      Log.Debug("[SiteBuilder] Copying all static content");
      var staticContentPath = _sourcePath.Subdirectory(Constants.STATIC_CONTENT);
      if (staticContentPath.Exists)
      {
        Log.Debug("[SiteBuilder] Copying all static content from " + staticContentPath.FullName);
        _sourcePath
          .Subdirectory(Constants.STATIC_CONTENT)
          .CopyTo(_outputPath.Subdirectory(Constants.STATIC_CONTENT, true));
      }
      // Copy files that aren't .md files
      Log.Debug("[SiteBuilder] Copying non-markdown files");
      _sourcePath
        .Subdirectory(Constants.PAGES)
        .CopyTo(_outputPath, excludedExtensions : new string[] { ".md" });
      // Load templates
      LoadTemplates();
      // Build out pages
      ProcessPages(rootNode);
      return _brokenPageCount;
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
      Log.Debug("[SiteBuilder] Loading templates");
      _sourcePath.Subdirectory(Constants.TEMPLATES)
        .GetFiles("*.cshtml", SearchOption.AllDirectories)
        .ToList()
        .ForEach(file =>
          {
            Log.Debug("[SiteBuilder] Registering template " + file.Name);
            // Register each template
            Razor.RegisterTemplate(file.Name.ToLower(), file.ReadAllText());
          });
      // Register the default template
      Razor.RegisterTemplate("no_template_defined", "<h1>No template defined!</h1> @Model.Content");
    }

    private void ProcessPages(Node node)
    {
      Log.Debug("[SiteBuilder] Processing pages");
      if (node is DirectoryNode)
      {
        Log.Debug("[SiteBuilder] Directory node: " + node.ToString());
        var directory = node as DirectoryNode;
        var destinationPath = directory.Path.FullName.Replace(_sourcePath.Subdirectory(Constants.PAGES).FullName, _outputPath.FullName);
        if (!Directory.Exists(destinationPath))
        {
          Log.Debug("[SiteBuilder] Creating directory " + destinationPath);
          Directory.CreateDirectory(destinationPath);
        }
      }
      else if (node is PageNode)
      {
        if (!ProcessPageNode(node as PageNode))
        {
          _brokenPageCount += 1;
        }
      }
      Log.Debug("[SiteBuilder] Node has " + node.Children.Count() + " children");
      foreach (var child in node.Children)
      {
        Log.Debug("[SiteBuilder] Processing child " + child.ToString());
        ProcessPages(child);                
      }
    }

    private bool ProcessPageNode(PageNode page)
    {
      Log.Debug("[SiteBuilder] Page Node: " + page.ToString());
      var succeeded = false;
      var rawContent = _markdown.Transform(page.GetRawContent());
      var templateFile = "~/" + page.Template + ".cshtml";
      var template = Razor.GetTemplate(templateFile);
      var pageModel = new PageModel(page, rawContent, page.PageLists);
      string renderedContent = null;
      try
      {
        renderedContent = Razor.ExecuteContent(template,
          model: pageModel,
          viewbag: page.Properties
        ).Result;
        succeeded = true;
      } 
      catch (TemplateCompileException e)
      {
        Log.Error("[SiteBuilder] Error while generating {0}: {1}", page.ToString(), e.Message);
        Log.Error(e.ToString());
        renderedContent = ErrorPageFactory.BuildCompilationError(template, templateFile, page, e);
      }
      var outputPath = page.GetOutputFileName().Replace(_sourcePath.Subdirectory(Constants.PAGES).FullName, _outputPath.FullName);
      Log.Debug("[SiteBuilder] Writing to " + outputPath);
      File.WriteAllText(outputPath, renderedContent);
      return succeeded;
    }
  }
}
