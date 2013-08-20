using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
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
        if (options.Help)
        {
          Console.WriteLine(Options.HelpText);
          return;
        }
        if (!options.Bootstrap && String.IsNullOrEmpty(options.Source) && String.IsNullOrEmpty(options.Destination))
        {
          Console.WriteLine("Nothing to do.");
          Console.WriteLine(Options.HelpText);
          return;
        }
        if (!options.Bootstrap &&
          (String.IsNullOrEmpty(options.Source) || !Directory.Exists(options.Source)))
        {
          Console.WriteLine("Path not found: " + options.Source);
          return;
        }

        if (options.Bootstrap)
        {
          var source = String.IsNullOrEmpty(options.Source) ? "." : options.Source;
          new Bootstrap(source).Run();
          return;
        }

        // Last but not least, just run the tool
        if (options.Watch)
        {
          var watcher = new Watcher(options.Source, options.Destination, options.DefaultTemplate);
          watcher.Watch();
        }
        else
        {
          new SiteBuilder(options.Source, options.Destination, options.DefaultTemplate).Run();
        }
      }
    }
  }
}
