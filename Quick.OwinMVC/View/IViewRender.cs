using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.View
{
    public interface IViewRender
    {
        void Init(IDictionary<String, String> properties);
        String Render(String viewName, IDictionary<String, Object> viewData);
    }
}
