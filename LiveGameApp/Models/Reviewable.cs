using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Reviewable : IIntKeyModel
    {
        public Reviewable()
        {
            Reviews = new HashSet<Reviews>();
        }

        public int Id { get; set; }
        public double? AverageRating { get; set; }

        public virtual Game Game { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
