using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Directmessage: IIntKeyModel
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public virtual Appuser Recipient { get; set; }
        public virtual Appuser Sender { get; set; }
    }
}
