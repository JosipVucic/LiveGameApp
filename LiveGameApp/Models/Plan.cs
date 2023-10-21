using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Plan: IIntKeyModel
    {
        public Plan()
        {
            Invitation = new HashSet<Invitation>();
            Participationrequest = new HashSet<Participationrequest>();
            Player = new HashSet<Player>();
            Spectator = new HashSet<Spectator>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public string Location { get; set; }
        public int MaxPlayers { get; set; }
        public int MaxSpectators { get; set; }
        public int HostUserId { get; set; }
        public int GameId { get; set; }
        public int TypeId { get; set; }
        public int PrivacyTypeId { get; set; }

        public virtual Game Game { get; set; }
        public virtual Appuser HostUser { get; set; }
        public virtual Reviewable IdNavigation { get; set; }
        public virtual Privacytype PrivacyType { get; set; }
        public virtual Plantype Type { get; set; }
        public virtual ICollection<Invitation> Invitation { get; set; }
        public virtual ICollection<Participationrequest> Participationrequest { get; set; }
        public virtual ICollection<Player> Player { get; set; }
        public virtual ICollection<Spectator> Spectator { get; set; }
    }
}
