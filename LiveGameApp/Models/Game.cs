using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Game : IIntKeyModel
    {
        public Game()
        {
            Author = new HashSet<Author>();
            Isgenre = new HashSet<Isgenre>();
            Owns = new HashSet<Owns>();
            Plan = new HashSet<Plan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string Description { get; set; }
        public string Rules { get; set; }
        public string ImageUrl { get; set; }

        public virtual Reviewable IdNavigation { get; set; }
        public virtual ICollection<Author> Author { get; set; }
        public virtual ICollection<Isgenre> Isgenre { get; set; }
        public virtual ICollection<Owns> Owns { get; set; }
        public virtual ICollection<Plan> Plan { get; set; }
    }
}
