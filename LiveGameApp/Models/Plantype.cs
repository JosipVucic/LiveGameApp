using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Plantype : IIntKeyModel
    {
        public Plantype()
        {
            Plan = new HashSet<Plan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Plan> Plan { get; set; }
    }
}
