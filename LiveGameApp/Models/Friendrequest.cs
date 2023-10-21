using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Friendrequest : IIntKeyModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public virtual Appuser Recipient { get; set; }
        public virtual Appuser Sender { get; set; }
    }
}
