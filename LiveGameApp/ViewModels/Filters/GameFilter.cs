using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels.Filters
{
    public class GameFilter : GameViewModel, IBaseFilter
    {
        public string q { get; set; }
        public int[] OwnerIds { get; set; }
        public int[] AuthorIds { get; set; }
        public int[] GenreIds { get; set; }
    }
}
