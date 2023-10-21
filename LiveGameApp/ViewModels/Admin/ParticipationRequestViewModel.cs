using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class ParticipationRequestViewModel : IIntKeyViewModel
    {

        public int? id { get; set; }
        public string Message { get; set; }
        public int SenderId { get; set; }
        public int TypeId { get; set; }
        public int PlanId { get; set; }


    }
}
