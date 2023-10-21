using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class SpectatorViewModel: IStringKeyViewModel
    {

        public string id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
    }
}
