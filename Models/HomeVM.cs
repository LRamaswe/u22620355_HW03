using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u22620355_HW03.Models
{
    public class HomeVM
    {
        public List<types> types { get; set; }
        public List<authors> authors { get; set; }
        public List<books> books { get; set; }
        public List<students> students { get; set; }
        public List<borrows> borrows { get; set; }

       
    }
}