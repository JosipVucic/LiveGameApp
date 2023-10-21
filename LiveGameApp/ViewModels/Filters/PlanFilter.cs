using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels.Filters
{
    public class PlanFilter : PlanViewModel, IBaseFilter
    {
        public string q { get; set; }
        public string Type { get; set; }
        public string PrivacyType { get; set; }
        public bool ShowMine { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
