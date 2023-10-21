using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels.Filters
{
    public class DirectMessageFilter : DirectMessageViewModel, IBaseFilter
    {
        public string q { get; set; }

    }
}
