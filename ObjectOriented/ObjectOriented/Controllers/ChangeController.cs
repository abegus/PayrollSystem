using ObjectOriented.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ObjectOriented.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace ObjectOriented.Controllers
{
    public class ChangeController : Controller { 
        private PayrollDbConnection db = new PayrollDbConnection();

        // GET: User
        public ActionResult Index()
        {
            
            var viewModel = new ChangeModule();

            AspNetUsers self = db.AspNetUsers.Find(User.Identity.GetUserId());

            //query to grab all users
            var users = from usr in db.AspNetUsers select usr;
            if(self.Level == 2)
            {
                users = from usr in db.AspNetUsers where (db.Mngs.Any(o => o.Mid == self.Id && o.EmpId == usr.Id)) select usr;
            }
            //query User -> Department
            var works = from wrk in db.Works select wrk;
            //query to User -> Manager
            var manages = from mng in db.Mngs select mng;

            //I hope this works?
            viewModel.Users = users;
            viewModel.WorksIn = works;
            viewModel.Mngs = manages;

            return View(viewModel);

           // return View(db.AspNetUsers.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // GET: User/Create 
        public ActionResult Create()
        {
            ChangeModule changeInstance = new ChangeModule();

            var mngs = new Mng();
            var works = new Work();

            AspNetUsers newUser = new AspNetUsers();
            newUser.Level = 1;

            /*changeInstance.Mngs = mngs;
            changeInstance.WorksIn = works;*/

            changeInstance.curUser = newUser;

            var allDeps = from dep in db.Departments select dep;
            bool[] whichDeps = new bool[allDeps.Count()];
            for (int i = 0; i < whichDeps.Length; i++)
            {
                whichDeps[i] = false;
            }
            // IEnumerable<AdvDepartment> advDeps;

            changeInstance.AllDeps = allDeps;
            changeInstance.whichDeps = whichDeps;


            //viewbag placeholders
            ViewBag.Name = null;
            ViewBag.Email = null;
            ViewBag.Password = null;
            ViewBag.Position = null;
            ViewBag.Level = null;
            ViewBag.Salary = null;
            ViewBag.StartDate = null;
            ViewBag.Witholdings = null;
            ViewBag.BirthDate = null;
            ViewBag.ManagerEmail = null;
            ViewBag.DepartmentName = null;

            return View(changeInstance);
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChangeModule changeInstance)
        //public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Position,Level,Salary,StartDate,EndDate,Witholdings,BirthDate")] AspNetUsers aspNetUsers)
        {
            AspNetUsers self = db.AspNetUsers.Find(User.Identity.GetUserId());
            /* check if the contents are valid */
            bool anythingWrong = false;
            if(changeInstance.curUser.Name == null)
            {
                ViewBag.Name = "Name must not be null";
                anythingWrong = true;
            }
            if(changeInstance.curUser.Email == null)
            {
                ViewBag.Email = "Name must not be null";
                anythingWrong = true;
            }
            else if(db.AspNetUsers.Any(o => o.Email == changeInstance.curUser.Email)) {
                ViewBag.Email = "Email Already Exists";
                anythingWrong = true;
            }
            if(changeInstance.curUser.PasswordHash == null || changeInstance.curUser.PasswordHash.Length < 6)
            {
                ViewBag.Password = "Password must be more than 5 characters";
                anythingWrong = true;
            }
            //Level stuff
            if (changeInstance.curUser.Level < 1 || changeInstance.curUser.Level > self.Level)
            {
                ViewBag.Level = "Invalid Level to set";
                anythingWrong = true;
            }
            if(changeInstance.curUser.Level == 1 && (changeInstance.curUser.Salary < 60000 || changeInstance.curUser.Salary > 90000) )
            {
                ViewBag.Salary = "Invalid price range for Developer";
                anythingWrong = true;
            }
            else if (changeInstance.curUser.Level == 2 && (changeInstance.curUser.Salary < 80000 || changeInstance.curUser.Salary > 120000))
            {
                ViewBag.Salary = "Invalid price range for Manager";
                anythingWrong = true;
            }
            else if (changeInstance.curUser.Level == 3 && (changeInstance.curUser.Salary < 100000 || changeInstance.curUser.Salary > 300000))
            {
                ViewBag.Salary = "Invalid price range for Admin";
                anythingWrong = true;
            }
            if(changeInstance.curUser.Witholdings < 0 || changeInstance.curUser.Witholdings > 10)
            {
                ViewBag.Witholdings = "Invalid withholding value";
                anythingWrong = true;
            }
            if(changeInstance.curUser.Level == 1 && !(db.AspNetUsers.Any(o => o.Email == changeInstance.managerEmail)))
            {
                ViewBag.ManagerEmail = "No such Manager exists";
                anythingWrong = true;
            }
            /* if ( (changeInstance.curUser.Level == 1 || changeInstance.curUser.Level == 2)  && !(db.Departments.Any(o => o.Name == changeInstance.departmentName)))
             {
                 ViewBag.DepartmentName = "No such Department exists";
                 anythingWrong = true;
             }*/

            bool containsDeps = false;
            //check if there are any departments that are TRUE
            for (int i = 0; i < changeInstance.whichDeps.Length; i++)
            {
                if (changeInstance.whichDeps[i])
                    containsDeps = true;
            }

            if ((changeInstance.curUser.Level == 1 || changeInstance.curUser.Level == 2) && !containsDeps)
            {
                ViewBag.DepartmentName = "No Departments Selected";
                anythingWrong = true;
            }

            /* check if the contents are valid, if not return back to view) */
            /* if (db.AspNetUsers.Any(o => o.Email == changeInstance.curUser.Email) &&
                 !(db.Departments.Any(o => o.Name == changeInstance.departmentName)) &&
                 !(db.AspNetUsers.Any(o => o.Email == changeInstance.managerEmail)) )
             {
                 changeInstance.curUser.Email = "this email/username already exists";
                 return View(changeInstance);
             }*/
            if (anythingWrong)
            {
                var ad = from dep in db.Departments select dep;
                bool[] whichDeps = new bool[ad.Count()];
                for (int i = 0; i < whichDeps.Length; i++)
                {
                    whichDeps[i] = false;
                }
                // IEnumerable<AdvDepartment> advDeps;

                changeInstance.AllDeps = ad;
                changeInstance.whichDeps = whichDeps;
                return View(changeInstance);
            }

/*  handle departments */
            var allDeps = from dep in db.Departments select dep;
            changeInstance.AllDeps = allDeps;
            List<Department> addToList = new List<Department>();
            String depString = "";
            //get all of the arrays
            var depArray = changeInstance.AllDeps.ToArray();
            for (int i = 0; i < depArray.Length; i++)
            {
                if (changeInstance.whichDeps[i])
                {//if this is a department to include...
                    var x = changeInstance.AllDeps.ToArray()[i];
                    addToList.Add(x); //long but maybe it works?
                    depString += x;
                }
            }
            var usersDeps = addToList.AsQueryable();


            /*create the new user*/
            var pass = changeInstance.curUser.PasswordHash;
            var user = new ApplicationUser
            {
                //UserName = "user1@gmail.com",
                //Email = "user1@gmail.com"
                UserName = changeInstance.curUser.Email,
                Email = changeInstance.curUser.Email
            };
            //string password = "51145@Gus";
            string password = pass;
            using (var d = new ApplicationDbContext())
            {
                var store = new UserStore<ApplicationUser>(d);
                var manager = new UserManager<ApplicationUser, string>(store);
                var result = manager.Create(user, password);
                if (!result.Succeeded)
                    throw new ApplicationException("Unable to create a user.");
                //result = manager.AddToRole(user.Id, "Administrator");
                if (!result.Succeeded)
                    throw new ApplicationException("Unable to add user to a role.");
            }

            /* change the fields in the user that we couldnt change before, as well as some mngs / works relationships */
            var newUsers = from usr in db.AspNetUsers where usr.Email.Equals(changeInstance.curUser.Email) select usr ;
            foreach (var newUser in newUsers)
            {
                AspNetUsers @usr = db.AspNetUsers.Find(newUser.Id);
                @usr.Level = changeInstance.curUser.Level;
                @usr.BirthDate = changeInstance.curUser.BirthDate;
                @usr.StartDate = changeInstance.curUser.StartDate;
                @usr.Witholdings = changeInstance.curUser.Witholdings;
                @usr.Position = changeInstance.curUser.Position;
                @usr.Salary = changeInstance.curUser.Salary;
                @usr.Name = changeInstance.curUser.Name;

                /* add a History Field for the new user */
                var uniqueId = Guid.NewGuid().ToString();
                var hist = new History();
                hist.Hid = uniqueId;
                hist.StartDate = @usr.StartDate;
                hist.UserId = @usr.Id;
                hist.Level = @usr.Level;
                hist.Pay = @usr.Salary;
                hist.Position = @usr.Position;
                hist.EndDate = @usr.EndDate;
                hist.Witholdings = @usr.Witholdings;

                if (@usr.Level == 1 || @usr.Level == 2)
                {

                    //ADD ALL THE WORK RELATIONSHIPS FOR EACH DEPARTMENT IN USERDEPS
                    //ALSO ADD THE UTP RELATIONSHIPS
                    List<String> allAdded = new List<String>();
                    foreach (var d in usersDeps)
                    {
                        Work w = new Work();
                        if (@usr.Level == 1)
                            w.IsEmployee = 1;
                        if (@usr.Level == 2)
                            w.IsManager = 1;
                        /*string departId = (from dep in db.Departments
                                           where dep.Name == changeInstance.departmentName
                                           select dep.DepId).First().ToString();*/
                        w.DepId = d.DepId;//departId;
                        w.UserId = @usr.Id;
                        db.Works.Add(w);

                        //ADD THE UTP STUFF
                        //list of all DTPS with current department (sealteam6 -> proj1, sealteam6 -> proj2)
                        var allDtpsInDep = from dtp in db.DepToProjs where dtp.DepId == d.DepId select dtp;
                        foreach(var dtp in allDtpsInDep)
                        {
                            var zero = from e in db.UserToProjs where e.UserId == @usr.Id && e.ProjId == dtp.ProjId select e;
                            //shoould be empty, getting duplicates
                            if (zero.Count() == 0)
                            {
                                UserToProj u = new UserToProj();
                                u.Percentage = 0;
                                u.UserId = @usr.Id;
                                u.ProjId = dtp.ProjId;
                                u.Project = dtp.Project;
                                u.AspNetUser = @usr;
                                if (allAdded == null || !allAdded.Contains(u.ProjId))
                                {
                                    db.UserToProjs.Add(u);
                                    allAdded.Add(u.ProjId);
                                }
                            }
                        }
                    }
                    //history
                    hist.DepartmentName = changeInstance.departmentName;
                }

                if (@usr.Level == 1)
                {
                    Mng m = new Mng();
                    string manId = (from u in db.AspNetUsers where u.Email == changeInstance.managerEmail select u.Id).First().ToString();
                    m.Mid = manId;
                    m.EmpId = @usr.Id;
                    db.Mngs.Add(m);

                    //history
                    hist.Mid = manId;
                }


               // db.Historys.Add(hist);
            }

            


            db.SaveChanges();

            return RedirectToAction("Index", "Change");
        }

        // GET: User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            ///aspNetUsers.
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }

            ChangeModule changeInstance = new ChangeModule();
           // changeInstance.AllDeps = new IEnumerable<Department>();

            var mngs = new Mng();
            var works = new Work();

            var allDeps = from dep in db.Departments select dep;
            bool[] whichDeps = new bool[allDeps.Count()];
            for (int i = 0; i < whichDeps.Length; i++)
            {
                whichDeps[i] = false;
            }
            // IEnumerable<AdvDepartment> advDeps;
            changeInstance.AllDeps = allDeps;
            changeInstance.whichDeps = whichDeps;

            changeInstance.curUser = aspNetUsers;

            return View(changeInstance);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChangeModule changeInstance) 
        //public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Position,Level,Salary,StartDate,EndDate,Witholdings,BirthDate")] AspNetUsers aspNetUsers)
        {
            AspNetUsers self = db.AspNetUsers.Find(User.Identity.GetUserId());
            /* check if the contents are valid */
            bool anythingWrong = false;
            
            //Level stuff
            if (changeInstance.curUser.Level < 1 || changeInstance.curUser.Level > self.Level)
            {
                ViewBag.Level = "Invalid Level to set";
                anythingWrong = true;
            }
            if (changeInstance.curUser.Level == 1 && (changeInstance.curUser.Salary < 60000 || changeInstance.curUser.Salary > 90000))
            {
                ViewBag.Salary = "Invalid price range for Developer";
                anythingWrong = true;
            }
            else if (changeInstance.curUser.Level == 2 && (changeInstance.curUser.Salary < 80000 || changeInstance.curUser.Salary > 120000))
            {
                ViewBag.Salary = "Invalid price range for Manager";
                anythingWrong = true;
            }
            else if (changeInstance.curUser.Level == 3 && (changeInstance.curUser.Salary < 100000 || changeInstance.curUser.Salary > 300000))
            {
                ViewBag.Salary = "Invalid price range for Admin";
                anythingWrong = true;
            }
            if (changeInstance.curUser.Witholdings < 0 || changeInstance.curUser.Witholdings > 10)
            {
                ViewBag.Witholdings = "Invalid withholding value";
                anythingWrong = true;
            }
            if (changeInstance.curUser.Level == 1 && !(db.AspNetUsers.Any(o => o.Email == changeInstance.managerEmail)))
            {
                ViewBag.ManagerEmail = "No such Manager exists";
                anythingWrong = true;
            }/*
            if ((changeInstance.curUser.Level == 1 || changeInstance.curUser.Level == 2) && !(db.Departments.Any(o => o.Name == changeInstance.departmentName)))
            {
                ViewBag.DepartmentName = "No such Department exists";
                anythingWrong = true;
            }*/

            bool containsDeps = false;
            //check if there are any departments that are TRUE
            for(int i = 0; i < changeInstance.whichDeps.Length; i++)
            {
                if (changeInstance.whichDeps[i])
                    containsDeps = true;
            }

            if( (changeInstance.curUser.Level == 1 || changeInstance.curUser.Level == 2) && !containsDeps)
            {
                ViewBag.DepartmentName = "No Departments Selected";
                anythingWrong = true;
            }
        // STILL HAVE TO RUN AGAINST OTHER ENTRIES IN HISTORY
            if ( DateTime.Compare(changeInstance.curUser.StartDate??DateTime.Now,changeInstance.PromotionDate) >= 0 || changeInstance.PromotionDate == null)
            {
                ViewBag.date = "Not a valid promotion date. (null or before hire)";
                anythingWrong = true;
            }

            /* check if the contents are valid, if not return back to view) */
            if ( anythingWrong)
            {
                return RedirectToAction("Edit", "Change", new { id = changeInstance.curUser.Id });
                // return View(changeInstance);
            }


            /*  handle departments */
            var allDeps = from dep in db.Departments select dep;
            changeInstance.AllDeps = allDeps;
            List<Department> addToList = new List<Department>();
            String depString = "";
            //get all of the arrays
            var depArray = changeInstance.AllDeps.ToArray();
            for (int i = 0; i < depArray.Length; i++)
            {
                if (changeInstance.whichDeps[i])
                {//if this is a department to include...
                    var x = changeInstance.AllDeps.ToArray()[i];
                    addToList.Add(x); //long but maybe it works?
                    depString += x;
                }
            }
            var usersDeps = addToList.AsQueryable();


            /* change the fields in the user that we couldnt change before, as well as some mngs / works relationships */
            var newUsers = from usr in db.AspNetUsers where usr.Email.Equals(changeInstance.curUser.Email) select usr;
            foreach (var newUser in newUsers)
            {
                AspNetUsers @usr = db.AspNetUsers.Find(newUser.Id);
                @usr.Level = changeInstance.curUser.Level;
                @usr.Witholdings = changeInstance.curUser.Witholdings;
                @usr.Position = changeInstance.curUser.Position;
                @usr.Salary = changeInstance.curUser.Salary;

                /* add a History Field for the new user */
                var hist = new History();
                var uniqueId = Guid.NewGuid().ToString();
                hist.Hid = uniqueId;
                hist.UserId = @usr.Id;
                hist.Level = @usr.Level;
                hist.Pay = @usr.Salary;
                hist.Position = @usr.Position;
                hist.StartDate = changeInstance.PromotionDate;
                hist.EndDate = @usr.EndDate; //should be null
                hist.Witholdings = @usr.Witholdings;


        //REMOVE ALL UTPS FOR THIS USER
                var utps = from utp in db.UserToProjs where utp.UserId == @usr.Id select utp;
                foreach(var u in utps)
                {
                    db.UserToProjs.Remove(u);
                }

                //update other history fields for the promoted user
                var HistToClose = from hst in db.Historys where hst.UserId == @usr.Id && hist.EndDate == null select hst;
                foreach(var hst in HistToClose)
                {
                    //remove and re-add;
                    //db.Historys.Remove(hst);
                    hst.EndDate = hist.StartDate;
                   // db.Historys.Add(hst);
                }

                if (@usr.Level == 1 || @usr.Level == 2)
                {
     //Delete all works (even though I do it below)
                    var allOldWorks = from pastWork in db.Works where (pastWork.UserId == @usr.Id) select pastWork;
                    foreach(var pastWork in allOldWorks)
                    {
                        db.Works.Remove(pastWork);
                    }

                    //ADD ALL THE WORK RELATIONSHIPS FOR EACH DEPARTMENT IN USERDEPS
                    //ALSO ADD THE UTP RELATIONSHIPS
                    List < String > allAdded = new List<String>();
                    foreach (var d in usersDeps)
                    {
                        Work w = new Work();
                        if (@usr.Level == 1)
                            w.IsEmployee = 1;
                        if (@usr.Level == 2)
                            w.IsManager = 1;
                        /*string departId = (from dep in db.Departments
                                           where dep.Name == changeInstance.departmentName
                                           select dep.DepId).First().ToString();*/
                        w.DepId = d.DepId;//departId;
                        w.UserId = @usr.Id;
                        //check for duplicate row, delete it if there
                        if (db.Works.Any(o => o.DepId == w.DepId && o.UserId == w.UserId))
                        {
                            var oldEntry = (from de in db.Works
                                            where de.DepId == w.DepId && de.UserId == w.UserId
                                            select de).First();
                            db.Works.Remove(oldEntry);
                        }
                        db.Works.Add(w);

                        //ADD THE UTP STUFF
                        //list of all DTPS with current department (sealteam6 -> proj1, sealteam6 -> proj2)
                        var allDtpsInDep = from dtp in db.DepToProjs where dtp.DepId == d.DepId select dtp;
                        foreach (var dtp in allDtpsInDep)
                        {
                            var zero = from e in db.UserToProjs where e.UserId == @usr.Id && e.ProjId == dtp.ProjId select e;
                            //shoould be empty, getting duplicates
                            if (zero.Count() == 0)
                            {
                                UserToProj u = new UserToProj();
                                u.Percentage = 0;
                                u.UserId = @usr.Id;
                                u.ProjId = dtp.ProjId;
                                u.Project = dtp.Project;
                                u.AspNetUser = @usr;
                                if (allAdded == null || !allAdded.Contains(u.ProjId))
                                {
                                    db.UserToProjs.Add(u);
                                    allAdded.Add(u.ProjId);
                                }
                            }
                        }
                    }


                    /* =========================================
                    Work w = new Work();
                    if (@usr.Level == 1)
                        w.IsEmployee = 1;
                    if (@usr.Level == 2)
                        w.IsManager = 1;
                    string departId = (from dep in db.Departments
                                       where dep.Name == changeInstance.departmentName
                                       select dep.DepId).First().ToString();
                    w.DepId = departId;
                    w.UserId = @usr.Id;

                    //check for duplicate row, delete it if there
                    if (db.Works.Any(o => o.DepId == w.DepId && o.UserId == w.UserId))
                    {
                        var oldEntry = (from d in db.Works
                                        where d.DepId == w.DepId && d.UserId == w.UserId
                                        select d).First();
                        db.Works.Remove(oldEntry);
                    }

                    db.Works.Add(w);*/
                    //history
                    hist.DepartmentName = changeInstance.departmentName;
                }
                //and checking that this doesnt already exist
                if (@usr.Level == 1)
                {
                    Mng m = new Mng();
                    string manId = (from u in db.AspNetUsers where u.Email == changeInstance.managerEmail select u.Id).First().ToString();
                    if (!(db.Mngs.Any(o => o.Mid == manId && o.EmpId == @usr.Id)))
                    {
                        m.Mid = manId;
                        m.EmpId = @usr.Id;
                        db.Mngs.Add(m);
                    }
                    hist.Mid = manId;
                }
                //db.Historys.Add(hist);
            }
            db.SaveChanges();

            return RedirectToAction("Index", "Change");


            /* if (ModelState.IsValid)
             {
                 db.Entry(aspNetUsers).State = EntityState.Modified;
                 db.SaveChanges();
                 return RedirectToAction("Index");
             }
             return View(aspNetUsers);*/

        }

        // GET: User/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: User/Delete/5
        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)   
        public ActionResult Delete(AspNetUsers user)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(user.Id);
            aspNetUsers.EndDate = user.EndDate;

            //find and remove rows in Works and Mng
            var departments = from dep in db.Works where dep.UserId == aspNetUsers.Id select dep;
           /* foreach(var dep in departments)
            {
                db.Works.Remove(dep);
            }
            var employees = from emp in db.Mngs where emp.EmpId == aspNetUsers.Id select emp;
            var managers = from emp in db.Mngs where emp.Mid == aspNetUsers.Id select emp;
            foreach(var emp in employees)
            {
                db.Mngs.Remove(emp);
            }
            foreach (var emp in managers)
            {
                db.Mngs.Remove(emp);
            }*/
            

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