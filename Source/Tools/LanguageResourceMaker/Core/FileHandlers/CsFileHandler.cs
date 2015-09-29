using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace LanguageResourceMaker.Core.FileHandlers
{
    public class CsFileHandler : AbstractFileHandler
    {
        //\[Text\s\("*(?'value'.*)"\)]\s*(?'key'[^,}]*),?
        private Regex regex = new Regex("\\[Text\\s\\(\"*(?'value'.*)\"\\)]\\s*(?'key'[^,}]*),?");
        //\[(?'name'.*?)(?'args'\(.*\))*]
        private Regex getAttrubuteNameRegex = new Regex("\\[(?'name'.*?)(?'args'\\(.*\\))*]");

        public override void Handle(FileInfo viewFile, DirectoryInfo projectFolder)
        {
            String text = viewFile.FullName;

            StreamReader reader = new StreamReader(viewFile.FullName);
            CSharpParser parser = new CSharpParser();
            SyntaxTree syntaxTree = parser.Parse(reader);
            reader.Close();
            foreach (EntityDeclaration item in syntaxTree.GetTypes(true))
            {
                if (!(item is TypeDeclaration))
                    continue;
                TypeDeclaration type = (TypeDeclaration)item;
                handle(type, projectFolder);
            }
        }

        private void handle(TypeDeclaration type, DirectoryInfo projectFolder)
        {
            if (type.ClassType != ClassType.Enum)
                return;

            Dictionary<String, String> textDict = new Dictionary<string, string>();
            foreach (AttributeSection attribute in type.Attributes)
            {
                String attributeText = attribute.ToString();
                Match attributeNameMatch = getAttrubuteNameRegex.Match(attribute.ToString());

                Group attributeNameGroup = attributeNameMatch.Groups["name"];
                if (!attributeNameGroup.Success || attributeNameGroup.Value != "TextResource")
                    continue;

                foreach (Match match in regex.Matches(type.ToString()))
                {
                    Group keyGroup = match.Groups["key"];
                    Group valueGroup = match.Groups["value"];
                    if (!keyGroup.Success || !valueGroup.Success)
                        continue;
                    String key = keyGroup.Value.Trim();
                    String value = valueGroup.Value.Trim();
                    textDict.Add(key, value);
                }
            }
            if (textDict.Count == 0)
                return;

            TypeDeclaration currentType = type;
            String typeFullName = null;
            while (true)
            {
                if (typeFullName == null)
                    typeFullName = type.Name;
                else
                    typeFullName = String.Format("{0}+{1}", currentType.Name, typeFullName);
                if (currentType.Parent is TypeDeclaration)
                {
                    currentType = (TypeDeclaration)currentType.Parent;
                    continue;
                }
                else if (currentType.Parent is NamespaceDeclaration)
                {
                    NamespaceDeclaration namesp = (NamespaceDeclaration)currentType.Parent;
                    typeFullName = String.Format("{0}.{1}", namesp.FullName, typeFullName);
                    break;
                }
                else
                    throw new ApplicationException("Type's parent unknown!");
            }
            handle(typeFullName, textDict, projectFolder);
        }

        private void handle(String typeFullName, Dictionary<String, String> textDict, DirectoryInfo projectFolder)
        {
            String outFileName;
            String projectName = projectFolder.Name;
            if (typeFullName.StartsWith(projectName + "."))
                outFileName = typeFullName.Substring((projectName + ".").Length);
            else
                outFileName = typeFullName;

            OutputLanguageFileAction(outFileName, projectFolder, textDict, Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}
