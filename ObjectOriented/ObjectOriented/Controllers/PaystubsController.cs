using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ObjectOriented.Models;
using Microsoft.AspNet.Identity;
using ObjectOriented.ViewModels;

namespace ObjectOriented.Controllers
{
    public class PaystubsController : Controller
    {
        private PayrollDbConnection db = new PayrollDbConnection();

        // GET: Paystubs
        public ActionResult Index()
        {
            AspNetUsers self = db.AspNetUsers.Find(User.Identity.GetUserId());

            var paystubs = db.Paystubs.Include(p => p.AspNetUser);
            if (self.Level != 3)
            {
                paystubs = from ps in db.Paystubs where ps.UserId == self.Id select ps;
            }
            foreach(var paystub in paystubs)
            {
                if (paystub.Mid != null)
                {
                    //paystub.Mid = db.AspNetUsers.Find(paystub.Mid).Email;
                }
            }
            
            

            return View(paystubs.ToList());
        }

        // GET: Paystubs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paystub paystub = db.Paystubs.Find(id);
            if(paystub.Mid != null) { }
                //paystub.Mid = db.AspNetUsers.Find(paystub.Mid).Email;
            if (paystub == null)
            {
                return HttpNotFound();
            }
            return View(paystub);
        }

        // GET: Paystubs/Create
        public ActionResult Create()
        {
            Paystub paystub = new Paystub();
            AspNetUsers user = db.AspNetUsers.Find(User.Identity.GetUserId());
           // 
            if (db.Works.Any(o => o.UserId == user.Id))
            {
                paystub.DepartmentName = (from x in db.Works where x.UserId == user.Id select x.Department.Name).First().ToString();
            }
            if( db.Mngs.Any(o => o.EmpId == user.Id))
            {
                paystub.Mid = (from x in db.Mngs where x.EmpId == user.Id select x.Mid).First().ToString();
            }
            paystub.AspNetUser = user;
            paystub.Pay = user.Salary / 12;
            paystub.StartDate = DateTime.Now;
            paystub.Tax = Decimal.Multiply((decimal)user.Salary /12, (decimal)0.25);
            paystub.NetPay = paystub.Pay - paystub.Tax - (user.Witholdings * paystub.Pay / 25);
            paystub.StubId = Guid.NewGuid().ToString();
            paystub.Witholdings = (int)user.Witholdings;
            paystub.UserId = user.Id;

            //HUGE PLAY HERE
            //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.UserId = user.Id;

            PaystubViewmodel stubmodel = new PaystubViewmodel();
            SelectList users = new SelectList(from us in db.AspNetUsers select us.Name);
            stubmodel.users = users;
            stubmodel.stub = paystub;

            // return View(paystub);
            return View(stubmodel);
        }

        // POST: Paystubs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "StubId,StartDate,Pay,NetPay,Tax,Witholdings,UserId,Mid,DepartmentName")] Paystub paystub)
        public ActionResult Create(PaystubViewmodel stubmodel)
        {
            if(stubmodel.selectedUser == null)
            {
                SelectList users = new SelectList(from us in db.AspNetUsers select us.Name);
                stubmodel.users = users;
                return View(stubmodel);
            }

            

            var who = stubmodel.selectedUser;
            AspNetUsers user = (from us in db.AspNetUsers where us.Name.Equals(who) select us).FirstOrDefault();

            if (user.EndDate != null && stubmodel.stub.StartDate >= user.EndDate )
            {
                SelectList users = new SelectList(from us in db.AspNetUsers select us.Name);
                stubmodel.users = users;
                ViewBag.UserId = "User is already fired";
                return View(stubmodel);
            }
            /* if (ModelState.IsValid && ! (db.Paystubs.Any(o => o.StartDate.Year == paystub.StartDate.Year && o.StartDate.Month == paystub.StartDate.Month 
             && o.UserId == paystub.UserId)))
             {

                 db.Paystubs.Add(paystub);
                 db.SaveChanges();
                 return RedirectToAction("Index");
             }*/

            /*ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", paystub.UserId);
            return View(paystub);*/
            

            Paystub paystub = new Paystub();

            /* if (db.Works.Any(o => o.UserId == user.Id))
             {
                 paystub.DepartmentName = (from x in db.Works where x.UserId == user.Id select x.Department.Name).First().ToString();
             }
             if (db.Mngs.Any(o => o.EmpId == user.Id))
             {
                 paystub.Mid = (from x in db.Mngs where x.EmpId == user.Id select x.Mid).First().ToString();
             }*/
            paystub.Mid = null;
            paystub.DepartmentName = null;

            paystub.AspNetUser = user;
            paystub.Pay = user.Salary / 12;
            paystub.StartDate = stubmodel.stub.StartDate;//DateTime.Now;
            paystub.Tax = Decimal.Multiply((decimal)user.Salary / 12, (decimal)0.25);
            paystub.NetPay = paystub.Pay - paystub.Tax - (user.Witholdings * paystub.Pay / 25);
            paystub.StubId = Guid.NewGuid().ToString();
            paystub.Witholdings = (int)user.Witholdings;
            paystub.UserId = user.Id;

            //get the managers and the departments
            var managers = (from mng in db.Mngs where mng.EmpId == user.Id select mng.AspNetUser1.Name);
            var departments = (from dep in db.Works where dep.UserId == user.Id select dep.Department.Name);
            foreach(var man in managers)
            {
                paystub.Mid += man + ", ";
            }
            foreach(var dep in departments)
            {
                paystub.DepartmentName += dep +", ";
            }

            //check to see if the date is after when they were fired.

            db.Paystubs.Add(paystub);
            db.SaveChanges();
            return RedirectToAction("Index");




            
        }

        // GET: Paystubs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paystub paystub = db.Paystubs.Find(id);
            if (paystub == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", paystub.UserId);
            return View(paystub);
        }

        // POST: Paystubs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StubId,StartDate,Pay,NetPay,Tax,Witholdings,UserId,Mid,DepartmentName")] Paystub paystub)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paystub).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", paystub.UserId);
            return View(paystub);
        }

        // GET: Paystubs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paystub paystub = db.Paystubs.Find(id);
            if (paystub == null)
            {
                return HttpNotFound();
            }
            return View(paystub);
        }

        // POST: Paystubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Paystub paystub = db.Paystubs.Find(id);
            db.Paystubs.Remove(paystub);
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
