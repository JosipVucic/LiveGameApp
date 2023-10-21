using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class CPlanViewModel: PlanViewModel
    {

        public bool IsHost { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsSpectator { get; set; }
        public string Host { get; set; }
        public string Game { get; set; }
        public string Type { get; set; }
        public string Rating { get; set; }
        public string PrivacyType { get; set; }
        public string[] Players { get; set; }
        public string[] Spectators { get; set; }
    }
}
