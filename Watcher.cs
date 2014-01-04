using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace champ
{
  public class Watcher
  {
    private string _source;
    private string _destination;
    private string _defaultTemplate;
    private ManualResetEvent _quitNotification;
    private object _lock;
    private FileSystemWatcher _fsWatcher;
    private DateTime _lastChange;

    public Watcher(string source, string destination, string defaultTemplate)
    {
      _source = source;
      _destination = destination;
      _defaultTemplate = defaultTemplate;
      _quitNotification = new ManualResetEvent(false);
      _lock = new object();
      _fsWatcher = new FileSystemWatcher(_source);
      _lastChange = DateTime.MinValue;
    }

    public void Watch()
    {
      Console.WriteLine("Watching for changes. Press Enter to force regeneration or CTRL^C to quit.");
      Console.CancelKeyPress += (sender, e) =>
        {
          _quitNotification.Set();
        };

      _fsWatcher.IncludeSubdirectories = true;
      _fsWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
      _fsWatcher.Changed += new FileSystemEventHandler(GenerateContentFromChange);
      _fsWatcher.Created += new FileSystemEventHandler(GenerateContentFromChange);
      _fsWatcher.Deleted += new FileSystemEventHandler(GenerateContentFromChange);
      _fsWatcher.Renamed += new RenamedEventHandler(GenerateContentFromRename);
      _fsWatcher.EnableRaisingEvents = true;

      while (true)
      {
        var key = Console.ReadKey(false);
        if (key.Key == ConsoleKey.Enter)
        {
          GenerateContent(true);
        }
        if (_quitNotification.WaitOne(new TimeSpan(1), false))
        {
          break;
        }
      }
      _fsWatcher.EnableRaisingEvents = false;
    }

    private void GenerateContentFromChange(object sender, FileSystemEventArgs e)
    {
      GenerateContent();
    }

    private void GenerateContentFromRename(object sender, RenamedEventArgs e)
    {
      GenerateContent();
    }

    private void GenerateContent(bool isForced = false)
    {
      var changeStamp = DateTime.Now;
      try
      {
        _fsWatcher.EnableRaisingEvents = false;
        lock (_lock)
        {
          if ((changeStamp - _lastChange).TotalMilliseconds < 500)
          {
            return;
          }
          Console.Write("{0} {1} change - regenerating...", changeStamp.ToShortTimeString(), isForced ? "Forcing" : "Detected");
          new SiteBuilder(_source, _destination, _defaultTemplate).Run();
          Console.WriteLine("done.");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("error: {0} - {1}", ex.GetType().Name, ex.Message);
      }
      finally
      {
        _fsWatcher.EnableRaisingEvents = true;
        _lastChange = changeStamp;
      }
    }
  }
}
