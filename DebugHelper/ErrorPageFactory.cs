using champ.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xipton.Razor.Core;

namespace champ.DebugHelper
{
  public static class ErrorPageFactory
  {
    private static string ERROR_PAGE_TEMPLATE =
      @"
<html>
<head>
  <title>{PageName} - Champ Error Report</title>
  <link rel='stylesheet' href='//netdna.bootstrapcdn.com/bootstrap/3.1.0/css/bootstrap.min.css'>
</head>
<body>

  <div class='navbar navbar-inverse navbar-fixed-top' role='navigation'>
    <div class='container'>
    <div class='navbar-header'>
      <button type='button' class='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
      <span class='sr-only'>Toggle navigation</span>
      <span class='icon-bar'></span>
      <span class='icon-bar'></span>
      <span class='icon-bar'></span>
      </button>
      <a class='navbar-brand' href='https://github.com/lukevenediger/champ/'>champ</a>
    </div>
    <div class='collapse navbar-collapse'>
      <ul class='nav navbar-nav'>
      <li class='active'><a href='https://github.com/lukevenediger/champ/'>Project Home</a></li>
      <li><a href='https://github.com/lukevenediger/champ/issues'>Report a Bug</a></li>
      </ul>
    </div>
    </div>
  </div>
    
  <div class='container' style='padding-top:39px;'>
    <h1>Error while building {PageName}</h1>
    <div class='alert alert-danger'>
      <strong>{Title}</strong>
      <br/>
      <p>{ErrorMessage}</p>
    </div>
    <ul>
      {Snippet}
    </ul>
    <h2>Details</h2>
    <ul>
      <li>Content file: {ContentFile}</li>
      <li>Template: {TemplateFile}</li>
      <li>Line Number: {LineNumber}</li>
    </ul> 
  </div>
</body>
</html>
";
    public static string BuildCompilationError(string template, 
      string templateFile,
      PageNode page,
      TemplateCompileException e)
    {
      var lines = e.Message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
      var title = lines[0];
      var errorMessage = lines[2];
      if (title.Contains("~/_MemoryContent/"))
      {
        title = title.Split(':')[0];
      }
      int lineNumber = Int32.Parse(lines[1].Split(':')[0].Replace("line", ""));
      string snippet = GetSnippet(template, lineNumber);

      return ERROR_PAGE_TEMPLATE
        .Replace("{PageName}", page.PageName)
        .Replace("{Title}", title)
        .Replace("{ErrorMessage}", errorMessage)
        .Replace("{LineNumber}", lineNumber.ToString())
        .Replace("{ContentFile}", page.PageFile.FullName)
        .Replace("{TemplateFile}", templateFile)
        .Replace("{Snippet}", snippet);
    }

    private static string GetSnippet(string template, int lineNumber)
    {
      var lines = template.Replace("\r", "").Split('\n');
      int startIndex = Math.Max(lineNumber - 4, 0);
      int endIndex = Math.Min(lineNumber + 4, lines.Length);;
      List<string> outputLines = new List<string>();

      for (var index = startIndex; index < endIndex; index++)
      {
        outputLines.Add("<li " + (index == (lineNumber - 1) ? "style='background: #f2dede;'" : "") + ">" +
                          "<strong>" + (index + 1) + ":</strong><code>" +
                          HttpUtility.HtmlEncode(lines[index]) + "</code></li>");
      }

      return String.Join(Environment.NewLine, outputLines.ToArray());
    }
  }
}
