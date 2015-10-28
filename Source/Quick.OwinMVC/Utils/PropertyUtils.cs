using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Utils
{
    public class PropertyUtils
    {
        public static IDictionary<String, String> Load(String content, String folderPath)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            content = parse(content, folderPath);
            //参考正则表达式: ^\s*(?'key'[^#][^\s]*?)\s*=\s*(?'value'.*)\s*$
            Regex regex = new Regex(@"^\s*(?'key'[^#][^\s]*?)\s*=\s*(?'value'.*)\s*$", RegexOptions.Multiline);
            foreach (Match match in regex.Matches(content))
            {
                String key = match.Groups["key"].Value;
                String value = match.Groups["value"].Value;
                value = value.Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\t", "\t").Replace(@"\f", "\f").Replace(@"\\", "\\");
                dict[key] = value;
            }
            return dict;
        }

        private static String parse(String content, String folderPath)
        {
            content = content.Replace("\\\r\n", "").Replace("\\\r", "").Replace("\\\n", "").Replace("\r", "");

            //解析include指令
            //正则表达式：^\s*include\s*=\s*(?'fileName'.*?)\s*$
            Regex regex = new Regex(@"^\s*include\s*=\s*(?'fileName'.*?)\s*$", RegexOptions.Multiline);
            content = regex.Replace(content, match =>
             {
                 var fileName = match.Groups["fileName"].Value;
                 if (!File.Exists(fileName))
                     fileName = Path.Combine(folderPath, fileName);
                 FileInfo file = new FileInfo(fileName);
                 return parse(File.ReadAllText(file.FullName), file.DirectoryName);
             });
            return content;
        }

        public static IDictionary<String, String> LoadFile(String fileName)
        {
            FileInfo file = new FileInfo(fileName);
            return Load(File.ReadAllText(file.FullName), file.DirectoryName);
        }
    }
}
