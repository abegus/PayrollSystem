using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ObjectOriented.Models;
using System.Web.Mvc;

namespace ObjectOriented.ViewModels
{
    public class PaystubViewmodel
    {
        public AspNetUsers curUser { get; set; }
        public SelectList users { get; set; }
        public String selectedUser { get; set; }
        public Paystub stub { get; set; }

    }
}