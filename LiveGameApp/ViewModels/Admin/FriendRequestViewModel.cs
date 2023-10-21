using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class FriendRequestViewModel : IIntKeyViewModel
    {

        public int? id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Message { get; set; }
    }
}
