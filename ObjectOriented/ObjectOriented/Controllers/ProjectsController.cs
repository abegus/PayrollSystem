using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ObjectOriented.Models;
using ObjectOriented.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace ObjectOriented.Controllers
{
    public class ProjectsController : Controller
    {
        private PayrollDbConnection db = new PayrollDbConnection();

        // GET: Projects
        public ActionResult Index()
        {
            AspNetUsers self = db.AspNetUsers.Find(User.Identity.GetUserId());

            var projs = db.Projects.ToList();

            if (self.Level == 2)
            {
                projs = (from utp in db.UserToProjs where utp.UserId == self.Id select utp.Project).ToList();
            }

            return View(projs);
        }

        // GET: Projects/Details/5
        public ActionResult Details(string id)
        {
            ProjectModule pm = new ProjectModule();
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            pm.Proj = project;
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(pm);
        }

        // Post: Projects/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(ProjectModule pm)
        {

            var month = pm.Month;
            //Project project = db.Projects.Find(id);

            //chek to see if it is in a valid time range
            if(pm.Proj.StartDate > pm.Month || pm.Proj.EndDate < pm.Month)
            {
                ViewBag.Month = "Project is not active during this month.";
                return View(pm);
            }

            Project project = pm.Proj;

            var depsInvolved = (from dtp in db.DepToProjs where dtp.ProjId == project.ProjId select dtp).ToArray();

            //store the departments in here
            Department[] allDeps = new Department[depsInvolved.Count()];
            AspNetUsers[][] usersInDeps = new AspNetUsers[depsInvolved.Count()][];
            decimal[][] userPercentages = new decimal[depsInvolved.Count()][];
            decimal[] depTotals = new decimal[depsInvolved.Count()];
            decimal totalCost = 0;

            //loop through each department
            for(int i = 0; i < depsInvolved.Length; i++)
            {
                var x = depsInvolved[i].DepId;
                //select all utp's that are in the given department and project (where percentage isnt 0)    AND USER ISNT FIRED
                var uinvolved = (from utp in db.UserToProjs where utp.ProjId == project.ProjId && utp.Percentage > 0
                                 && (from w in db.Works where w.DepId == x &&
                                     w.UserId == utp.UserId &&
                                     !(utp.AspNetUser.EndDate != null && utp.AspNetUser.EndDate <= pm.Month) //All users who arent fired..?
                                     select w).Count() > 0
                                 select utp).ToArray();
                usersInDeps[i] = new AspNetUsers[uinvolved.Length];
                userPercentages[i] = new decimal[uinvolved.Length];
                //set the departments array
                allDeps[i] = depsInvolved[i].Department;

                //loop through and add all users found to the array
                decimal depCost = 0;
    // CURRENTLY I just add the whole month pay regardless of the days after fire date
    // CURRENTLY I DO NOT REMOVE PEOPLE FROM THE LIST IF THEY ARE FIRED, BUT I DO CHANGE THE TOTAL
                for(int j = 0; j < uinvolved.Length; j++)
                {
                    usersInDeps[i][j] = uinvolved[j].AspNetUser;

                    var tempid = usersInDeps[i][j].Id;

                    //get the percentage
                    userPercentages[i][j] = (from utp in db.UserToProjs
                                            where utp.UserId == tempid &&
                 utp.ProjId == project.ProjId
                                            select utp.Percentage).FirstOrDefault();


                    int isFired = -2;
                    //check if they are fired...
                    if (usersInDeps[i][j].EndDate != null)
                        isFired = DateTime.Compare(pm.Month, usersInDeps[i][j].EndDate??DateTime.Now); //??
                    //lessthan 0, t1 < t2
                    if(isFired < 0)
                        depCost += (usersInDeps[i][j].Salary / 12) * (userPercentages[i][j] / 100);

                    //their salary per month
                    //depCost += usersInDeps[i][j].Salary / 12;
                }
                depTotals[i] = Math.Round(depCost,2);
                totalCost += depCost;
            }
            pm.UsersPercentage = userPercentages;
            pm.Proj.Total = Math.Round(totalCost + pm.Proj.OtherCosts,2);
            pm.DepsArray = allDeps;
            pm.UsersArray = usersInDeps;
            pm.DepCosts = depTotals;

            
            return View(pm);
        }

        // Send in a ProjectModule Object, includes Proj, UTP arr, DTP arr, Dep
        // GET: Projects/Create
        public ActionResult Create()
        {
            ProjectModule project = new ProjectModule();

            //create contents of project
            var uniqueId = Guid.NewGuid().ToString();
            Project newProj = new Models.Project();
            newProj.ProjId = uniqueId;
            project.Proj = newProj;

            var allDeps = from dep in db.Departments select dep;
            bool[] whichDeps = new bool[allDeps.Count()];
            for(int i = 0; i < whichDeps.Length; i++)
            {
                whichDeps[i] = false;
            }
           // IEnumerable<AdvDepartment> advDeps;
           
            project.AllDeps = allDeps;
            project.whichDeps = whichDeps;


            return View(project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectModule project)
        //public ActionResult Create([Bind(Include = "ProjId,Name,OtherCosts,Total,StartDate,EndDate")] Project project)
        {
            bool existsOne = false;
            foreach(var b in project.whichDeps)
            {
                if (b)
                {
                    existsOne = true;
                }
            }
            if (!existsOne)
            {
                return RedirectToAction("Index");
            }

            var allDeps = from dep in db.Departments select dep;
            project.AllDeps = allDeps;
            List<Department> addToList = new List<Department>();
            //get all of the arrays
            var depArray = project.AllDeps.ToArray();
            for (int i = 0; i < depArray.Length; i++)
            {
                if (project.whichDeps[i])
                {//if this is a department to include...
                    var x = project.AllDeps.ToArray()[i];
                    addToList.Add( x ); //long but maybe it works?
                }
            }
            project.Deps = addToList.AsQueryable();

            //doesnt work
            IQueryable<AspNetUsers> users = null;
            foreach (var dep in project.Deps) {
               var some = from u in db.Works where u.DepId == dep.DepId select u.AspNetUser;
                if (some.ToArray().Length > 0)
                {
                    if (users == null)
                    {
                        users = some;
                    }
                    else
                        users = users.Concat(some);
                }
            }

            /* CHECK MODEL STATE ABOVE..... THEN ADD USERS TO UTP, DEPS TO DTP */

            if (ModelState.IsValid)
            {
                //add project object to database
                db.Projects.Add(project.Proj);

                //add all the departments involved to DTP
                foreach(var dep in project.Deps)
                {
                    DepToProj newDTPEntry = new DepToProj();
                    newDTPEntry.DepId = dep.DepId;
                    newDTPEntry.ProjId = project.Proj.ProjId;
                    newDTPEntry.Temp = "nothing";
                    db.DepToProjs.Add(newDTPEntry);
                }

                //loop through and add entries for all of the users now being involved
                var allUsersInProjects = from user in db.UserToProjs select user;
                foreach(var user in users)
                {
                    //create new entry and set fields.     do id too??
                    UserToProj newUTPEntry = new UserToProj();
                    newUTPEntry.ProjId = project.Proj.ProjId;
                    newUTPEntry.UserId = user.Id;
                    newUTPEntry.Temp = "nothing";

                    var allInvolvedIn = user.UserToProjs.ToArray();
                    //set percentages
                    if(allInvolvedIn.Length < 1)
                    {
                        newUTPEntry.Percentage = (decimal)100.0;
                    }
                    else
                        newUTPEntry.Percentage = 0;

                    //add new UTP
                    db.UserToProjs.Add(newUTPEntry);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ProjectModule pm = new ProjectModule();
            var allDeps = from dep in db.Departments select dep;
            bool[] whichDeps = new bool[allDeps.Count()];
            for (int i = 0; i < whichDeps.Length; i++)
            {
                whichDeps[i] = false;
            }
            // IEnumerable<AdvDepartment> advDeps;

            pm.AllDeps = allDeps;
            pm.whichDeps = whichDeps;
            pm.Proj = project;

            return View(pm);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjId,Name,OtherCosts,Total,StartDate,EndDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // GET: Projects/Adjust/5
        public ActionResult Adjust(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectModule projectModule = new ProjectModule();
            Project project = db.Projects.Find(id);
            projectModule.Proj = project;

            var allDTPs = from dtp in db.DepToProjs where dtp.ProjId == id select dtp;
            projectModule.DTP = allDTPs;
            var allUTPs = from utp in db.UserToProjs where utp.ProjId == id &&
                          (from u in db.UserToProjs where u.UserId == utp.UserId && u.ProjId != utp.ProjId select u).Count() > 0
                          select utp;
            var specialUTPs = from utp in db.UserToProjs where utp.ProjId == id &&
                               (from u in db.UserToProjs where u.UserId == utp.UserId && u.ProjId != utp.ProjId select u).Count() == 0
                              select utp;
            projectModule.UTP = specialUTPs; //all of the utps that only have this as a project...

            ///projectModule.UTP = allUTPs;
            projectModule.UTPS = allUTPs.ToArray();
            projectModule.NestedUtps = new UserToProj[projectModule.UTPS.Length][];
            for(int i = 0; i < projectModule.UTPS.Length; i++)
            {
                var x = projectModule.UTPS[i];
                var temp =  from utp in db.UserToProjs where utp.UserId == x.UserId && utp.ProjId != x.ProjId select utp;
                var y = temp.Count();
                projectModule.NestedUtps[i] = new UserToProj[y];
                projectModule.NestedUtps[i] = temp.ToArray();
            }

            foreach(var x in allUTPs)
            {
                
            }
          
            if (project == null)
            {
                return HttpNotFound();
            }
            //viewbag placeholders
            ViewBag.Percentage = null;

            return View(projectModule);
        }

        // POST: Projects/Adjust/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Adjust(ProjectModule pj)
        {
            if(pj.UTPS == null)
            {
                return RedirectToAction("Index");
            }
            //check the fields are valid
            for(int i = 0; i < pj.UTPS.Length; i++)
            {
                decimal sum = pj.UTPS[i].Percentage;
                for(int j = 0; j < pj.NestedUtps[i].Length; j++)
                {
                    sum += pj.NestedUtps[i][j].Percentage;
                }
                if(sum <= 99 || sum >= 101)
                {
                    ViewBag.Percentage = sum + " is not a valid Percentage";
                    //Reset all the info because I'm shit at programming

                    ProjectModule projectModule = new ProjectModule();
                    Project project = db.Projects.Find(pj.Proj.ProjId);
                    var id = pj.Proj.ProjId;
                    projectModule.Proj = project;

                    var allDTPs = from dtp in db.DepToProjs where dtp.ProjId == id select dtp;
                    projectModule.DTP = allDTPs;
                    var allUTPs = from utp in db.UserToProjs
                                  where utp.ProjId == id &&
       (from u in db.UserToProjs where u.UserId == utp.UserId && u.ProjId != utp.ProjId select u).Count() > 0
                                  select utp;
                    var specialUTPs = from utp in db.UserToProjs
                                      where utp.ProjId == id &&
            (from u in db.UserToProjs where u.UserId == utp.UserId && u.ProjId != utp.ProjId select u).Count() == 0
                                      select utp;
                    projectModule.UTP = specialUTPs; //all of the utps that only have this as a project...

                    ///projectModule.UTP = allUTPs;
                    projectModule.UTPS = allUTPs.ToArray();
                    projectModule.NestedUtps = new UserToProj[projectModule.UTPS.Length][];
                    for (int ui = 0; ui < projectModule.UTPS.Length; ui++)
                    {
                        var x = projectModule.UTPS[ui];
                        var temp = from utp in db.UserToProjs where utp.UserId == x.UserId && utp.ProjId != x.ProjId select utp;
                        var y = temp.Count();
                        projectModule.NestedUtps[ui] = new UserToProj[y];
                        projectModule.NestedUtps[ui] = temp.ToArray();
                    }

                    //End reset
                    return View(projectModule);
                }
            }

            //fields are valid. update changes
            for(int i = 0; i < pj.UTPS.Length; i++)
            {
                var u = pj.UTPS[i];
                var v = (from uts in db.UserToProjs where uts.ProjId == u.ProjId && uts.UserId == u.UserId select uts).FirstOrDefault();
                if(u.Percentage != v.Percentage)
                {
                    v.Percentage = u.Percentage;
                }
                for (int j = 0; j < pj.NestedUtps[i].Length; j++)
                {
                    var newUtps = pj.NestedUtps[i][j];
                    var actual = (from uts in db.UserToProjs where uts.ProjId == newUtps.ProjId && uts.UserId == newUtps.UserId select uts).FirstOrDefault();
                    if (newUtps.Percentage != actual.Percentage)
                    {
                        actual.Percentage = newUtps.Percentage;
                        db.SaveChanges();
                    }
                }
            }
            db.SaveChanges();

            /*if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);*/
            return RedirectToAction("Index");
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Project project = db.Projects.Find(id);
            var dtps = from dtp in db.DepToProjs where dtp.ProjId == id select dtp;
            var utps = from utp in db.UserToProjs where utp.ProjId == id select utp;
            foreach(var d in dtps)
            {
                db.DepToProjs.Remove(d);
            }
            foreach(var u in utps)
            {
                db.UserToProjs.Remove(u);
            }
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
