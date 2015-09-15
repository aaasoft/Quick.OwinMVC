using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Hunter
{
    public interface IHungryPropertyHunter
    {
        void Hunt(IDictionary<String, String> properties);
    }
}
