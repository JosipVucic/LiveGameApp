using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Appuser: IdentityUser<int>, IIntKeyModel
    {
        public Appuser()
        {
            Author = new HashSet<Author>();
            DirectmessageRecipient = new HashSet<Directmessage>();
            DirectmessageSender = new HashSet<Directmessage>();
            FriendUserHigh = new HashSet<Friend>();
            FriendUserLow = new HashSet<Friend>();
            FriendrequestRecipient = new HashSet<Friendrequest>();
            FriendrequestSender = new HashSet<Friendrequest>();
            Hasrole = new HashSet<Hasrole>();
            Invitation = new HashSet<Invitation>();
            Message = new HashSet<Message>();
            Owns = new HashSet<Owns>();
            Participant = new HashSet<Participant>();
            Participationrequest = new HashSet<Participationrequest>();
            Plan = new HashSet<Plan>();
            Player = new HashSet<Player>();
            Reviews = new HashSet<Reviews>();
            Spectator = new HashSet<Spectator>();
        }

        [PersonalData]
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Author> Author { get; set; }
        public virtual ICollection<Directmessage> DirectmessageRecipient { get; set; }
        public virtual ICollection<Directmessage> DirectmessageSender { get; set; }
        public virtual ICollection<Friend> FriendUserHigh { get; set; }
        public virtual ICollection<Friend> FriendUserLow { get; set; }
        public virtual ICollection<Friendrequest> FriendrequestRecipient { get; set; }
        public virtual ICollection<Friendrequest> FriendrequestSender { get; set; }
        public virtual ICollection<Hasrole> Hasrole { get; set; }
        public virtual ICollection<Invitation> Invitation { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Owns> Owns { get; set; }
        public virtual ICollection<Participant> Participant { get; set; }
        public virtual ICollection<Participationrequest> Participationrequest { get; set; }
        public virtual ICollection<Plan> Plan { get; set; }
        public virtual ICollection<Player> Player { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
        public virtual ICollection<Spectator> Spectator { get; set; }
    }
}
