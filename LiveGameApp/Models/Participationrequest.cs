using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Participationrequest : IIntKeyModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int SenderId { get; set; }
        public int TypeId { get; set; }
        public int PlanId { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual Appuser Sender { get; set; }
        public virtual Inviterequesttype Type { get; set; }
    }
}
