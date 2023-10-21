using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class IsGenreViewModel : IStringKeyViewModel
    {

        public string id { get; set; }
        public int GameId { get; set; }
        public int GenreId { get; set; }
    }
}
