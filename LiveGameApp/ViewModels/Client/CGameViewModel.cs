using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class CGameViewModel: GameViewModel
    {

        public bool IsOwned { get; set; }
        public bool IsAuthor { get; set; }
        public int[] GenreIds { get; set; }
        public int[] AuthorIds { get; set; }

        public string[] Authors { get; set; }
        public string[] Genres { get; set; }

    }
}
