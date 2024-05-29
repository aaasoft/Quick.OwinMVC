﻿using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.WebSocket
{
    public class WebSocketManager
    {
        private static WebSocketManager _Instance = new WebSocketManager();
        public static WebSocketManager Instance 
        {
            get{return _Instance;}
        }

        private List<Type> connectionList = new List<Type>();
        
        public Func<Type, string> GetRouteFunc { get; set; } 
        public WebSocketManager()
        {
            GetRouteFunc= type =>
           {
               var assembly = type.Assembly;

               var assemblyName = assembly.GetName().Name;
               var typeName = type.FullName;
               if (typeName.StartsWith(assemblyName))
                   typeName = typeName.Substring(assemblyName.Length + 1);
               var contextPath = Server.Instance.ContextPath;
               if (string.IsNullOrEmpty(contextPath))
                   contextPath = "/";

               if (!contextPath.StartsWith("/"))
                   contextPath = "/" + contextPath;
               if (!contextPath.EndsWith("/"))
                   contextPath = contextPath + "/";
               return string.Format("{0}ws/{1}/{2}", contextPath, assemblyName, typeName);
           };
        }

        public void Register<T>()
            where T : WebSocketConnection
        {
            Register(typeof(T));
        }

        public IDictionary<string, Type> GetConnectionTypeDict()
        {
            return connectionList.ToDictionary(t => GetRouteFunc(t), t => t);
        }

        public void Register(Type connectionType)
        {
            connectionList.Add(connectionType);
        }

        public void Unregister<T>()
            where T : WebSocketConnection
        {
            Unregister(typeof(T));
        }

        public void Unregister(Type connectionType)
        {
            if (connectionList.Contains(connectionType))
                connectionList.Remove(connectionType);
        }
    }
}
