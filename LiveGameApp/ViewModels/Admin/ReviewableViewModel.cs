using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveGameApp.ViewModels
{
    public class ReviewableViewModel : IIntKeyViewModel
    {

        public int? id { get; set; }
        public double? AverageRating { get; set; }
    }
}
