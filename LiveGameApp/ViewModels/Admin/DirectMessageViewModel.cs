using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class DirectMessageViewModel: IIntKeyViewModel
    {

        public int? id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime Datetime { get; set; }
        public string Content { get; set; }
        
    }
}
