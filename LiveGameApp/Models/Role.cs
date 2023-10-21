using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Role: IdentityRole<int>, IIntKeyModel
    {
        public Role()
        {
            Hasrole = new HashSet<Hasrole>();
        }

        override public int Id { get; set; }
        override public string Name { get; set; }

        public virtual ICollection<Hasrole> Hasrole { get; set; }
    }
}
