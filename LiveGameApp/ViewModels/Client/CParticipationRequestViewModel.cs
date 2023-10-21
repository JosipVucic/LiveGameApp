using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class CParticipationRequestViewModel : ParticipationRequestViewModel
    {

        public bool IsAccepted { get; set; }

        public string Type { get; set; }

        public string Plan { get; set; }
    }
}
