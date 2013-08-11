using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  class Program
  {
    static void Main(string[] args)
    {
      var options = new Options();
      if (Parser.Default.ParseArguments(args, options))
      {
        if (options.Build)
        {
          new SiteBuilder(options.Source, options.Destination, options.DefaultTemplate).Run();
        }
      }
    }
  }
}
