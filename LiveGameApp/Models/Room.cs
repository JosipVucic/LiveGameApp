using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Room : IIntKeyModel
    {
        public Room()
        {
            Message = new HashSet<Message>();
            Participant = new HashSet<Participant>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Participant> Participant { get; set; }
    }
}
