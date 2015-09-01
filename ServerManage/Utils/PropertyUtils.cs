using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerManage.Utils
{
    public class PropertyUtils
    {
        public static IDictionary<String, String> Load(String content)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            foreach (String str in content.Split('\r', '\n'))
            {
                var line = str.Trim();
                if (line.StartsWith("#"))
                    continue;
                var strs = line.Split('=');
                if (strs.Length < 2)
                    continue;
                String key = strs[0].Trim();
                String value = strs[1].Trim();
                dict[key] = value;
            }
            return dict;
        }

        public static IDictionary<String, String> LoadFile(String fileName)
        {
            return Load(File.ReadAllText(fileName));
        }
    }
}
