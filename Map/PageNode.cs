using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace champ.Map
{
  public class PageNode : Node
  {
    public FileInfo PageFile { get; private set; }
    public String PageName { get; private set; }
    public string Title { get; private set; }
    public string Template { get; set; }
    public dynamic Properties { get; set; }

    public PageNode(FileInfo file, dynamic globalSettings)
      : base(Path.ChangeExtension(file.Name, "html"))
    {
      PageFile = file;
      PageName = Path.ChangeExtension(file.Name, "html");
      Properties = file.GetProperties().Augment(globalSettings);
      Template = Properties.HasProperty("template") ? Properties.template : null;
      Title = Properties.title;
    }

    public string GetRawContent()
    {
      return File.ReadAllText(PageFile.FullName);
    }

    public string GetOutputFileName()
    {
      return Path.ChangeExtension(PageFile.FullName, "html");
    }
  }
}
