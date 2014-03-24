using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace champ.Map
{
    public class PageNode : Node
    {
        public FileInfo PageFile { get; private set; }
        public String PageName { get; private set; }
        public string Title { get; private set; }
        public string Summary { get; private set; }
        public string Template { get; set; }
        public dynamic Properties { get; set; }
        public DateTime DateStamp { get; private set; }
        public string ListName { get; set; }

        private string _rawContent;

        public PageNode(string pageContents, dynamic globalSettings = null)
            : base(string.Empty)
        {
            if (globalSettings == null)
            {
                globalSettings = new BetterExpando();
            }
            _rawContent = pageContents;
            PageFile = null;
            PageName = String.Empty;
            Properties = pageContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).GetProperties().Augment(globalSettings);
            InitialisePageNode();
        }

        public PageNode(FileInfo file, dynamic globalSettings)
            : base(Path.ChangeExtension(file.Name, "html"))
        {
            PageFile = file;
            PageName = Path.ChangeExtension(file.Name, "html");
            Properties = file.GetProperties().Augment(globalSettings);
            InitialisePageNode();
        }

        public string GetRawContent()
        {
            if (String.IsNullOrEmpty(_rawContent))
            {
                return File.ReadAllText(PageFile.FullName);
            }
            else
            {
                return _rawContent;
            }
        }

        public string GetOutputFileName()
        {
            return Path.ChangeExtension(PageFile.FullName, "html");
        }

        private void InitialisePageNode()
        {
            Template = Properties.HasProperty("template") ? Properties.template : null;
            Summary = Properties.HasProperty("summary") ? Properties.summary : null;
            Title = Properties.title;
            ListName = Properties.HasProperty("list") ? Properties.list : null;

            if (PageFile != null)
            {
                if (Properties.HasProperty("Date"))
                {
                    string rawDate;
                    if (Properties.Date.ToLower() == "folder")
                    {
                        rawDate = PageFile.Directory.Name;
                    }
                    else
                    {
                        rawDate = Properties.Date;
                    }
                    DateStamp = DateTime.Parse(rawDate);
                }
                else
                {
                    DateStamp = PageFile.LastWriteTime;
                }
            }
            else
            {
                DateStamp = DateTime.Now;
            }

        }
    }
}
