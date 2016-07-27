using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    /// <summary>
    /// 节点方法已经处理异常，抛出此异常后，则不处理Response
    /// </summary>
    public class NodeMethodHandledException : Exception
    {
        private NodeMethodHandledException()
        {
        }

        public static Exception Instance
        {
            get
            {
                return new AppDomainUnloadedException(null, new NodeMethodHandledException());
            }
        }
    }
}
