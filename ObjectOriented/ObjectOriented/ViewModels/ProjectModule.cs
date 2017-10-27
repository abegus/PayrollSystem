using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ObjectOriented.Models;

namespace ObjectOriented.ViewModels
{
    public class ProjectModule
    {
        public Project Proj { get; set; }
        public IEnumerable<Department> Deps { get; set; }
        public IEnumerable<Department> AllDeps { get; set; }
        public bool[] whichDeps { get; set; }
        public IEnumerable<DepToProj> DTP { get; set; }
        public IEnumerable<UserToProj> UTP { get; set; }

        public UserToProj[] UTPS { get; set; }
        public UserToProj[][] NestedUtps { get; set; }

        // FOR THE REPORT 
        public DateTime Month { get; set; }
        public Department[] DepsArray { get; set; }
        public AspNetUsers[][] UsersArray { get; set; }
        public decimal[][] UsersPercentage { get; set; }
        public decimal[] DepCosts { get; set; }
    }
}