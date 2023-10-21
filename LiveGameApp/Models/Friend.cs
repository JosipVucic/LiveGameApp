using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LiveGameApp.Models
{
    public partial class Friend
    {
        public int UserLowId { get; set; }
        public int UserHighId { get; set; }

        public virtual Appuser UserHigh { get; set; }
        public virtual Appuser UserLow { get; set; }
    }
}
