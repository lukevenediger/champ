using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  public static class Log
  {
    public static LogLevel Threshold { get; set; }

    public static void Debug(string message, params string[] format)
    {
      LogMessage(LogLevel.Debug, message, format);
    }

    public static void Error(string message, params string[] format)
    {
      LogMessage(LogLevel.Error, message, format);
    }

    private static void LogMessage(LogLevel level, string message, params string[] format)
    {
      if (level < Threshold)
      {
        return;
      }
      if (format.Length == 0)
      {
        Console.WriteLine(level.ToString() + ": " + message);
      }
      else
      {
        Console.WriteLine(level.ToString() + ": " + String.Format(message, format));
      }
    }
  }

  public enum LogLevel : int
  {
    Debug = 0,
    Error = 1,
    Off = 2
  }
}
