using Microsoft.Owin;
using Quick.OwinMVC;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Node;
using Quick.OwinMVC.Node.ValueFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace Plugin.ApiDoc.Node.Api
{
    [TextResource]
    public enum DocOutput
    {
        [Text("html格式")]
        HTML,
        [Text("xml格式")]
        XML
    }

    public class DocParameter
    {
        [FormFieldInfo(Key = nameof(Tag),
            Name = "标签",
            Description = "可以查询指定标签的接口，如果不传入此参数，则返回全部接口。",
            NotEmpty = false)]
        public String Tag { get; set; }

        [FormFieldInfo(Key = nameof(Output),
            Name = "输出格式",
            Description = "设置输出的格式：html或者xml",
            NotEmpty = false)]
        [ComboboxValueFormat(ValuesEnum = typeof(DocOutput), ValuesEnum_KeyByEnumName = true)]
        public DocOutput Output { get; set; } = DocOutput.HTML;
    }

    public class Utils
    {
        private static XmlDocument getDocument(IOwinContext context, DocParameter input, String xsltPath)
        {
            XmlDocument document = new XmlDocument();
            if (!String.IsNullOrEmpty(xsltPath))
                document.AppendChild(document.CreateProcessingInstruction(
                "xml-stylesheet",
                $"type='text/xsl' href='{xsltPath}'"));
            var rootElement = document.CreateElement("Api");
            if (!String.IsNullOrEmpty(input.Tag))
                rootElement.SetAttribute("Tag", input.Tag);
            document.AppendChild(rootElement);

            writeLevel1Nodes(context, document, rootElement, NodeManager.Instance, input);
            return document;
        }


        public static void OutputXml(IOwinContext context, DocParameter input, String xsltPath, bool outputXml = true)
        {
            var xmlSetting = new XmlWriterSettings() { Encoding = Encoding.UTF8 };

            var uri = context.Request.Uri;
            xsltPath = $"{uri.Scheme}://{uri.Host}:{uri.Port}{xsltPath}";
            //if (!outputXml)
            //    xsltPath = "Plugins" + xsltPath;

            var document = getDocument(context, input, xsltPath);
            var rep = context.Response;

            if (outputXml)
            {
                rep.ContentType = "text/xml";
                using (var writer = XmlWriter.Create(rep.Body, xmlSetting))
                {
                    document.WriteTo(writer);
                }
            }
            else
            {
                rep.ContentType = "text/html";
                var myXslTransform = new XslCompiledTransform();
                myXslTransform.Load(xsltPath);
                myXslTransform.Transform(document, null, rep.Body);
            }
            rep.Body.Flush();
        }

        //第一级节点(插件)
        private static void writeLevel1Nodes(IOwinContext context, XmlDocument document, XmlElement parentElement, INode parentNode, DocParameter input)
        {
            foreach (var childNode in parentNode.GetChildren())
            {
                var childElement = document.CreateElement("Plugin");
                childElement.SetAttribute(nameof(childNode.Id), childNode.Id);
                childElement.SetAttribute(nameof(childNode.Name), childNode.Name);

                writeLevel2Nodes(context, document, childElement, childNode, input);
                if (childElement.GetElementsByTagName("Feature").Count > 0)
                    parentElement.AppendChild(childElement);
            }
        }

        //第二级节点(功能)
        private static void writeLevel2Nodes(IOwinContext context, XmlDocument document, XmlElement parentElement, INode parentNode, DocParameter input)
        {
            foreach (var childNode in parentNode.GetChildren())
            {
                var childElement = document.CreateElement("Feature");
                childElement.SetAttribute(nameof(childNode.Id), childNode.Id);
                childElement.SetAttribute(nameof(childNode.Name), childNode.Name);

                writeMethods(context, document, childElement, childNode, input);
                if (childElement.GetElementsByTagName(typeof(IMethod).Name).Count > 0)
                    parentElement.AppendChild(childElement);
            }
        }

        private static void writeMethods(IOwinContext context, XmlDocument document, XmlElement element, INode node, DocParameter input)
        {
            foreach (var method in node.GetMethods()
                .Where(t =>
                {
                    if (input.Tag == null)
                        return (t.Value.Tags == null
                            || !t.Value.Tags.Contains("hidden"));
                    else
                        return input.Tag == "all" ||
                            (t.Value.Tags != null
                            && t.Value.Tags.Contains(input.Tag));
                }
                ))
            {
                var HttpMethod = method.Key;
                var Method = method.Value;

                var methodElement = document.CreateElement(typeof(IMethod).Name);

                methodElement.SetAttribute(nameof(HttpMethod), HttpMethod);
                methodElement.SetAttribute(nameof(Method.Path), Method.Path);
                methodElement.SetAttribute(nameof(Method.Name), Method.Name);
                methodElement.SetAttribute(nameof(Method.Description), Method.Description);
                methodElement.SetAttribute(nameof(Method.InvokeExample), Method.InvokeExample);
                methodElement.SetAttribute(nameof(Method.ReturnValueExample), Method.ReturnValueExample);
                writePermissions(context, document, methodElement, Method);
                writeTags(document, methodElement, Method);
                writeParameters(document, methodElement, Method);

                element.AppendChild(methodElement);
            }

            foreach (var childNode in node.GetChildren())
            {
                writeMethods(context, document, element, childNode, input);
            }
        }

        private static void writePermissions(IOwinContext context, XmlDocument document, XmlElement element, IMethod method)
        {
            //如果方法不要求权限
            if (!(method is IPermissionRequired))
                return;
            IPermissionRequired permissionRequired = (IPermissionRequired)method;
            if (permissionRequired.Permissions == null
                || permissionRequired.Permissions.Length == 0)
                return;
            //输出权限信息
            var permissionsElement = document.CreateElement("Permissions");
            foreach (var permission in permissionRequired.Permissions)
            {
                var name = context.GetTextManager().GetText(permission);
                var permissionElement = document.CreateElement("Permission");
                permissionElement.SetAttribute("Name", name);
                permissionsElement.AppendChild(permissionElement);
            }
            element.AppendChild(permissionsElement);
        }

        private static void writeTags(XmlDocument document, XmlElement element, IMethod method)
        {
            if (method.Tags == null || method.Tags.Length == 0)
                return;
            var tagsElement = document.CreateElement("Tags");
            foreach (var tag in method.Tags)
            {
                var tagElement = document.CreateElement("Tag");
                tagElement.SetAttribute("Name", tag);
                tagsElement.AppendChild(tagElement);
            }
            element.AppendChild(tagsElement);
        }

        private static void writeParameters(XmlDocument document, XmlElement element, IMethod method)
        {
            if (method.InputType == null
                || method.InputType == typeof(object))
                return;
            var parametersElement = document.CreateElement("Parameters");
            foreach (var parameter in FormFieldInfo.GetAll(TextManager.DefaultInstance, method.InputType))
            {
                var abc = parameter.TypeId;
                var parameterElement = document.CreateElement(typeof(FormFieldInfo).Name);
                parameterElement.SetAttribute(nameof(parameter.Key), parameter.Key);
                parameterElement.SetAttribute(nameof(parameter.Name), parameter.Name);
                parameterElement.SetAttribute(nameof(parameter.Type), parameter.Type);
                parameterElement.SetAttribute(nameof(parameter.Description), parameter.Description);
                parameterElement.SetAttribute(nameof(parameter.NotEmpty), parameter.NotEmpty.ToString());
                parameterElement.SetAttribute(nameof(parameter.ValueFormatType), parameter.ValueFormatType);
                //parameterElement.SetAttribute(nameof(parameter.ValueFormatValue), DataUtils.Serialize(parameter.ValueFormatValue));
                parametersElement.AppendChild(parameterElement);
            }
            element.AppendChild(parametersElement);
        }
    }
}
