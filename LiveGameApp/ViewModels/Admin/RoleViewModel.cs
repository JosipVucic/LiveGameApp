using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class RoleViewModel : IIntKeyViewModel
    {

        public int? id { get; set; }
        public string Name { get; set; }
    }
}
