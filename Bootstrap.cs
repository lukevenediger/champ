using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  public class Bootstrap
  {
    private DirectoryInfo _source;
    private string _uncLocation;

    public Bootstrap(string sourceDirectory, string bootstrapUNCLocation)
    {
      _source = new DirectoryInfo(sourceDirectory);
      _uncLocation = bootstrapUNCLocation;
    }

    public void Run()
    {
      // Download the template
      var targetFile = Path.GetTempFileName();
      if (String.IsNullOrEmpty(_uncLocation))
      {
        var client = new WebClient();
        Console.WriteLine("Downloading champ-bootstrap.zip (github.com)");
        client.DownloadFile("https://github.com/lukevenediger/champ-bootstrap/releases/download/v1.0/champ-bootstrap.zip", targetFile);
      }
      else
      {
        var file = new FileInfo(_uncLocation);
        Console.WriteLine("Copying " + file.Name + " from " + file.Directory.FullName);
        file.CopyTo(targetFile, true);
      }
      // Unzip it to the source directory
      using (var zipFile = ZipFile.Read(targetFile))
      {
        zipFile.ExtractAll(_source.FullName, ExtractExistingFileAction.OverwriteSilently);
      }
      Console.WriteLine("Done.");
    }
  }
}
