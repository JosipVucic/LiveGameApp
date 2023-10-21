using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class ReviewViewModel : IStringKeyViewModel
    {

        public string id { get; set; }
        public int UserId { get; set; }
        public int ReviewableId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
    }
}
