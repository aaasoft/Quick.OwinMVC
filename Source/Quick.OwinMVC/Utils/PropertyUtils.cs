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
        public static IDictionary<String, String> Load(String content, String folderPath, params String[] notAllowFiles)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            content = parse(content, folderPath, new HashSet<string>(notAllowFiles));
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

        private static String parse(String content, String folderPath, HashSet<String> notAllowFiles)
        {
            content = content.Replace("\\\r\n", "").Replace("\\\r", "").Replace("\\\n", "").Replace("\r", "");

            //解析include指令
            //正则表达式：^\s*include\s*=\s*(?'fileName'.*?)\s*$
            Regex regex = new Regex(@"^\s*include\s*=\s*(?'fileName'.*?)\s*$", RegexOptions.Multiline);
            content = regex.Replace(content, match =>
             {
                 var fileName = match.Groups["fileName"].Value;
                 List<String> list = new List<string>();
                 list.AddRange(PathUtils.SearchFile(fileName));
                 if (list.Count == 0)
                 {
                     fileName = Path.Combine(folderPath, fileName);
                     list.AddRange(PathUtils.SearchFile(fileName));
                 }
                 StringBuilder sb = new StringBuilder();
                 foreach (var currentFileName in list)
                 {
                     FileInfo file = new FileInfo(currentFileName);
                     if (notAllowFiles.Contains(file.FullName))
                         continue;
                     sb.AppendLine(parse(File.ReadAllText(file.FullName), file.DirectoryName, notAllowFiles));
                 }
                 return sb.ToString();
             });
            return content;
        }

        public static IDictionary<String, String> LoadFile(String fileName)
        {
            FileInfo file = new FileInfo(fileName);
            return Load(File.ReadAllText(file.FullName), file.DirectoryName, file.FullName);
        }
    }
}
