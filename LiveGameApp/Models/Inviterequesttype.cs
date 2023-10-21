using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Inviterequesttype : IIntKeyModel
    {
        public Inviterequesttype()
        {
            Invitation = new HashSet<Invitation>();
            Participationrequest = new HashSet<Participationrequest>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Invitation> Invitation { get; set; }
        public virtual ICollection<Participationrequest> Participationrequest { get; set; }
    }
}
