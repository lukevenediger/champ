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
    [Option('b', "build", HelpText="Builds the site", Required=false)]
    public bool Build { get; set; }
    [ValueOption(0)]
    public string Source { get; set; }
    [ValueOption(1)]
    public string Destination { get; set; }
  }
}
