using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class ParticipantViewModel : IStringKeyViewModel
    {

        public string id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
