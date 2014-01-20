using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  internal class Options
  {
    [ValueOption(0)]
    public string Source { get; set; }
    [ValueOption(1)]
    public string Destination { get; set; }

    [Option('t', "template", HelpText = "Specifies the default template.", Required = false)]
    public string DefaultTemplate { get; set; }
    [Option('b', "bootstrap", HelpText = "Download a bootstrapped site to use with champ.", Required = false, DefaultValue = false)]
    public bool Bootstrap { get; set; }
    [Option('w', "watch", HelpText = "Watch for changes and regenerate the site.", Required = false, DefaultValue = false)]
    public bool Watch {get; set;}
    [Option('h', "help", HelpText = "Prints this help text. More info at https://github.com/lukevenediger/champ", Required = false, DefaultValue = false)]
    public bool Help { get; set; }

    public static string HelpText
    {
      get
      {
        var help = CommandLine.Text.HelpText.AutoBuild(new Options());
        return String.Join("\n",
          help,
          "Examples:",
          @"  champ.exe c:\mysite                    Rebuild and save in c:\mysite\output",
          @"  champ.exe c:\mysite c:\output",
          @"  champ.exe c:\mysite c:\output --watch  Watch c:\mysite for file changes"
          );
      }
    }
  }
}
