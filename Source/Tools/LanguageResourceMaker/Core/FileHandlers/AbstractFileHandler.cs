using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageResourceMaker.Core.FileHandlers
{
    public abstract class AbstractFileHandler : IFileHandler
    {
        public virtual string GetFolderPath()
        {
            return String.Empty;
        }

        public virtual Action<string, System.IO.DirectoryInfo, Dictionary<String, String>, string> OutputLanguageFileAction { get; set; }


        public abstract void Handle(System.IO.FileInfo viewFile, System.IO.DirectoryInfo projectFolder);
    }
}
