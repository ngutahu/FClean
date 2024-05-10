using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectFClean.Models
{
        public class RentViewModel
        {
            public Renter Renter { get; set; }
            public Housekeeper Housekeeper { get; set; }
            public Compact Compact { get; set; }
            public List<Service> Services { get; set; }
    }
}