using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LanguageResourceMaker.Utils
{
    public class DebugUtils
    {
        public static String GetSourceCodeFolder()
        {
            return new DirectoryInfo("../../../../").FullName;
        }
    }
}
