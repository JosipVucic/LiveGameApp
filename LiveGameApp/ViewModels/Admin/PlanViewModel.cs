using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class PlanViewModel : IIntKeyViewModel
    {

        public int? id { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public string Location { get; set; }
        public int MaxPlayers { get; set; }
        public int MaxSpectators { get; set; }
        public int HostUserId { get; set; }
        public int GameId { get; set; }
        public int TypeId { get; set; }
        public int PrivacyTypeId { get; set; }
        
        
    }
}
