using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.View
{
    public interface IViewRender
    {
        String Render(String viewName, IDictionary<String, Object> viewData);
    }
}
