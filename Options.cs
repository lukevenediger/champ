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
    [Option('h', "help", HelpText = "Prints this help text. More info at https://github.com/lukevenediger/champ", Required = false, DefaultValue = false)]
    public bool Help { get; set; }

    public static string HelpText
    {
      get
      {
        return "Usage: champ.exe [--template TEMPLATE] [--bootstrap] SOURCE-DIR DESTINATION-DIR";
      }
    }
  }
}
