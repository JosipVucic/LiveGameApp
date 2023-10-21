using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class MessageViewModel : IIntKeyViewModel
    {

        public int? id { get; set; }
        public string Content { get; set; }
        public DateTime Datetime { get; set; }
        public int? UserId { get; set; }
        public int RoomId { get; set; }
        
        
        
    }
}
