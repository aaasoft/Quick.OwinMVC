﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quick.OwinMVC.Hunter
{
    public interface ITypeHunter
    {
        void Hunt(Type type);
    }
}
