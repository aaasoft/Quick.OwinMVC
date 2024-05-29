using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Utils
{
    public class MimeUtils
    {
        private static IDictionary<String, String> mimeMap;

        static MimeUtils()
        {
            mimeMap = new Dictionary<String, String>();
            var lines = Quick.OwinMVC.Properties.Resources.MIME_DICT.Split('\r','\n');
            foreach(var line in lines)
            {
                var strs =  line.Split('=');
                if(strs.Length<2)
                    continue;
                var key=strs[0];
                var value=strs[1];
                mimeMap[key]=value;
            }
        }

        public static void AddMime(string key,string value)
        {
            mimeMap[key]=value;
        }
        
        public static void RemoveMime(string key)
        {
            if(mimeMap.ContainsKey(key))
                mimeMap.Remove(key);
        }

        public static String GetMime(String resourceName)
        {
            String key = Path.GetExtension(resourceName.ToLower()).Replace(".", "");
            if (mimeMap.ContainsKey(key))
                return mimeMap[key];
            return "application/octet-stream";
        }
    }
}
