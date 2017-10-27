using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ObjectOriented.Models;

namespace ObjectOriented.ViewModels
{
    public class ChangeModule
    {
        public AspNetUsers curUser { get; set; }
        public string managerEmail { get; set; }
        public string departmentName { get; set; }
        public DateTime PromotionDate { get; set; }
        public IEnumerable<AspNetUsers> Users { get; set; }
        public IEnumerable<Work> WorksIn { get; set; }
        public IEnumerable<Mng> Mngs { get; set; }

        /* FOR THE BULLET POINTS */
        public IEnumerable<Department> AllDeps { get; set; }
        public bool[] whichDeps { get; set; }

    }
}