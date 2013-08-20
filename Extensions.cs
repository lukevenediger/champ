using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xipton.Razor;

namespace champ
{
  public static class Extensions
  {
    /// <summary>
    /// Copies the contents of a directory to the destination.
    /// </summary>
    /// <remarks>Taken from http://stackoverflow.com/a/15648301/2363991 </remarks>
    public static void CopyTo(this DirectoryInfo sourcePath, 
      DirectoryInfo destinationPath, 
      bool overwrite = false,
      string[] excludedExtensions = null)
    {
      excludedExtensions = excludedExtensions ?? new string[] { };
      foreach (var sourceSubDirPath in sourcePath.EnumerateDirectories("*", SearchOption.AllDirectories))
      {
        Directory.CreateDirectory(sourceSubDirPath.FullName.Replace(sourcePath.FullName, destinationPath.FullName));
      }

      // Copy all files except those that have been excluded by extension
      foreach (var file in sourcePath.EnumerateFiles("*", SearchOption.AllDirectories).Where(f => !excludedExtensions.Contains(f.Extension)))
      {
        File.Copy(file.FullName, file.FullName.Replace(sourcePath.FullName, destinationPath.FullName), overwrite);
      }
    }

    public static string ReadAllText(this FileInfo fileInfo)
    {
      return File.ReadAllText(fileInfo.FullName);
    }

    public static string GetTemplate(this RazorMachine razor, string templateName)
    {
      try
      {
        return razor.GetRegisteredInMemoryTemplates()[templateName];
      }
      catch (KeyNotFoundException ex)
      {
        throw new KeyNotFoundException("Could not find a template called " + templateName);
      }
    }

    public static void RecursiveDelete(this DirectoryInfo directory)
    {
      var files = directory.GetFiles("*", SearchOption.AllDirectories)
        .Where(p => !p.DirectoryName.Split('\\').Any(n => n.StartsWith(".")))
        .Where(p => !p.Name.StartsWith("."))
        .ToList();
      files.ForEach(p => p.Delete());
    }

    public static DirectoryInfo Subdirectory(this DirectoryInfo directory, string subdirectoryName, bool createIfNotExists = false)
    {
      var subdirectory = new DirectoryInfo(Path.Combine(directory.FullName, subdirectoryName));
      if (createIfNotExists && !subdirectory.Exists)
      {
        subdirectory.Create();
      }
      return subdirectory;
    }

    public static void BuildOut(this DirectoryInfo directory, DirectoryInfo sourcePath, DirectoryInfo destinationPath)
    {
      var relativeParts = directory.RelativePath(sourcePath);
    }

    public static string RelativePath(this DirectoryInfo directory, DirectoryInfo rootPath)
    {
      var parts = new List<string>();
      while (rootPath.FullName != directory.FullName)
      {
        parts.Add(directory.Name);
        directory = directory.Parent;
      }
      parts.Reverse();
      return parts.ToString();
    }

    public static bool TryGetFile(this DirectoryInfo directory, string filename, out FileInfo file)
    {
      var hits = directory.GetFiles(filename);
      if (hits.Length == 0)
      {
        file = null;
        return false;
      }
      file = hits[0];
      return true;
    }

    public static IEnumerable<T> TakeUpTo<T>(this IEnumerable<T> items, Predicate<T> predicate)
    {
      foreach (var item in items)
      {
        if (predicate(item))
        {
          yield break;
        }
        else
        {
          yield return item;
        }
      }
    }

    public static IEnumerable<T> TakeAfter<T>(this IEnumerable<T> items, Predicate<T> predicate)
    {
      var canSendItems = false;
      foreach (var item in items)
      {
        if (canSendItems)
        {
          yield return item;
          continue;
        }
        else if (predicate(item))
        {
          canSendItems = true;
        }
      }
    }

    public static dynamic GetProperties(this FileInfo file)
    {
      dynamic properties = new BetterExpando(ignoreCase: true, returnEmptyStringForMissingProperties: true);
      File.ReadAllLines(file.FullName)
        .TakeUpTo(line => line.Contains("-->"))
        .Where(line => !String.IsNullOrEmpty(line))
        .ForEach(line =>
          {
            var parts = line.Split('=')
              .Select(part => part.Trim())
              .Where(part => part.Length > 0)
              .ToArray();
            if (parts.Length == 2)
            {
              properties[parts[0]] = parts[1];
            }
          });
      return properties; 
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
      foreach (T item in items)
      {
        action(item);
      }
    }
  }
}
