using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ObjectOriented.Models
{
    public class Manager
    {
        public IEnumerable<Department> Departments { get; set; }
        public AspNetUsers UserAccount { get; set; }
    }
}
