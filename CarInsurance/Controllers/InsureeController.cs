using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insuree.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insuree.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Insuree.Add(insuree);
                db.SaveChanges();
                QuoteCalc(insuree.Id);
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insuree.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insuree.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insuree.Find(id);
            db.Insuree.Remove(insuree);
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

        public ActionResult QuoteCalc(int Id)
        {
            using (InsuranceEntities db = new InsuranceEntities())
            {
                var quote = 50.0M;
                var insuree = db.Insuree.Find(Id);
                var dob = insuree.DateOfBirth;
                var CarYear = insuree.CarYear;
                var CarMake = insuree.CarMake;
                var CarModel = insuree.CarModel;
                var SpeedingTickets = insuree.SpeedingTickets;
                bool DUI = insuree.DUI;
                bool CoverageType = insuree.CoverageType;

                DateTime Now = DateTime.Now;
                int Years = new DateTime(DateTime.Now.Subtract(dob).Ticks).Year - 1;
                DateTime PYD = dob.AddYears(Years);
                int Months = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (PYD.AddMonths(i) == Now)
                    {
                        Months = i;
                        break;
                    }
                    else if (PYD.AddMonths(i) >= Now)
                    {
                        Months = i - 1;
                        break;
                    }
                }

                double age = Years + (Months / 12);


                if (age <= 18)
                {
                    quote += 100.00M;

                }
                else if (age > 18 && age < 25)
                {
                    quote += 50.00M;
                }
                else if (age >= 25)
                {
                    quote += 25.00M;
                }

                if (CarYear < 2000)
                {
                    quote += 25.00M;
                }
                else if (CarYear > 2015)
                {
                    quote += 25.00M;
                }

                if (CarMake == "Porsche")
                {
                    quote += 25.00M;
                }

                if (CarMake == "Porsche" && CarModel == "911 Carrera")
                {
                    quote += 25.00M;
                }

                quote += (SpeedingTickets * 10);

                if (DUI)
                {
                    quote += (quote * .25M);
                }

                if (CoverageType)
                {
                    quote += (quote * 0.50M);
                }
                insuree.Quote = quote;
                db.SaveChanges();

            }
            return View("Index");
        }
    }
}
