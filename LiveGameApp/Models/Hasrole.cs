using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Hasrole: IdentityUserRole<int>
    {
        //public int UserId { get; set; }
        //public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Appuser User { get; set; }
    }
}
