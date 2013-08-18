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

    public Bootstrap(string sourceDirectory)
    {
      _source = new DirectoryInfo(sourceDirectory);
    }

    public void Run()
    {
      // Download the template
      var targetFile = Path.GetTempFileName();
      var client = new WebClient();
      Console.WriteLine("Downloading champ-bootstrap.zip (github.com)");
      client.DownloadFile("https://github.com/lukevenediger/champ-bootstrap/releases/download/v1.0/champ-bootstrap.zip", targetFile);
      Console.WriteLine("Done.");
      // Unzip it to the source directory
      using (var zipFile = ZipFile.Read(targetFile))
      {
        zipFile.ExtractAll(_source.FullName, ExtractExistingFileAction.OverwriteSilently);
      }
    }
  }
}
