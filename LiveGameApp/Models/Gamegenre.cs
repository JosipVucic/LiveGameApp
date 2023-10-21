using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Gamegenre : IIntKeyModel
    {
        public Gamegenre()
        {
            Isgenre = new HashSet<Isgenre>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Isgenre> Isgenre { get; set; }
    }
}
