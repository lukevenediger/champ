using CommandLine;
using CommandLine.Text;
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
    static int Main(string[] args)
    {
      var options = new Options();
      var exitCode = 0;
      if (!Parser.Default.ParseArguments(args, options))
      {
        Console.WriteLine("Couldn\'t understand that command. For help please try champ.exe --help");
        return 1;
      }

      if (options.Help)
      {
        Console.WriteLine(Options.HelpText);
        return 0;
      }
      Log.Threshold = options.EnableVerboseLogging ? LogLevel.Debug : LogLevel.Error;
      Log.Debug("[Champ] Champ is starting.");
      if (!options.Bootstrap && String.IsNullOrEmpty(options.Source) && String.IsNullOrEmpty(options.Destination))
      {
        Console.WriteLine("Nothing to do.");
        Console.WriteLine(Options.HelpText);
        return 0;
      }
      if (!options.Bootstrap &&
        (String.IsNullOrEmpty(options.Source) || !Directory.Exists(options.Source)))
      {
        Console.WriteLine("Path not found: " + options.Source);
        return 1;
      }

      if (options.Bootstrap)
      {
        Log.Debug("[Champ] Downloading champ bootstrap file.");
        var source = String.IsNullOrEmpty(options.Source) ? "." : options.Source;
        new Bootstrap(source, options.BootstrapSource).Run();
        return 0;
      }

      // Last but not least, just run the tool
      if (options.Watch)
      {
        Log.Debug("[Champ] Starting Watcher.");
        var watcher = new Watcher(options.Source, options.Destination, options.DefaultTemplate);
        watcher.Watch();
      }
      else
      {
        Log.Debug("[Champ] Starting Site Builder.");
        int numBrokenPages = new SiteBuilder(options.Source, options.Destination, options.DefaultTemplate).Run();
        if (numBrokenPages > 0)
        {
          Log.Error("{0} pages did not process correctly.", numBrokenPages.ToString());
          exitCode = 1;
        }
      }
      Log.Debug("[Champ] Done, exiting.");
      return exitCode;
    }
  }
}
