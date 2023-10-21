using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class FriendViewModel : IStringKeyViewModel
    {

        public string id { get; set; }
        public int UserLowId { get; set; }
        public int UserHighId { get; set; }
    }
}
