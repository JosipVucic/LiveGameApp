using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels.Filters
{
    public class AppUserFilter : AppUserViewModel, IBaseFilter
    {
        public string q { get; set; }
        public bool FriendsOnly { get; set; }
    }
}
