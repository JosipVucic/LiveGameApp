using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class AuthorViewModel : IStringKeyViewModel
    {

        public string id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
    }
}
