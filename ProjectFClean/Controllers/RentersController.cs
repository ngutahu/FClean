using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectFClean.Models;

namespace ProjectFClean.Controllers
{
    public class RentersController : Controller
    {
        private ProjectFClean1Entities db = new ProjectFClean1Entities();

        // GET: Renters
        public ActionResult Index()
        {
            var account = Session["Account"] as ProjectFClean.Models.Account;
            var renters = db.Renters.ToList();
            if (account != null && renters != null)
            {
                var renter = renters.SingleOrDefault(r => r.AccountID == account.AccountID);
                if (renter != null)
                {
                    return View(renter);
                }
            }
            return RedirectToAction("Login", "Accounts");
        }

        // GET: Renters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Renter renter = db.Renters.Find(id);
            if (renter == null)
            {
                return HttpNotFound();
            }
            return View(renter);
        }

        // GET: Renters/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name");
            return View();
        }

        // POST: Renters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RID,Money,Address,AccountID")] Renter renter)
        {
            if (ModelState.IsValid)
            {
                db.Renters.Add(renter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", renter.AccountID);
            return View(renter);
        }

        // GET: Renters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Renter renter = db.Renters.Find(id);
            if (renter == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", renter.AccountID);
            return View(renter);
        }


        [HttpPost, ActionName("UpdateProfile")]
        public ActionResult UpdateProfile(FormCollection form)
        {
            //Get link imge
            HttpPostedFileBase file = Request.Files["file"];
            string filename = "";
            if (file != null && file.ContentLength > 0)
            {
                filename = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/img"), filename);
                file.SaveAs(path);
            }
            //end get link image

            var idStr = form["Id"];
            string[] parts = idStr.Split(',');

            int a, b;
            int.TryParse(parts[0].Trim(), out a);
            int.TryParse(parts[1].Trim(), out b);

            var name = form["Name"];
            var email = form["Email"];
            var phone = form["Phone"];


            var account = db.Accounts.FirstOrDefault(p => p.AccountID == a);

            if (account != null)
            {
                if (filename != null && filename.Length > 0)
                {
                    //add link image to db
                    account.ImgAvatarID = filename;
                }
                account.Email = email;
                account.Phone = phone;
                account.Name = name;

                db.Entry(account).State = EntityState.Modified;
                Session["Account"] = account;
            }

            db.SaveChanges();

            // Extracting values from the form
            // Form chứa một khối các data => cách lấy data của từng đối tượng là form["tên đối tượng] nhưng dạng nguyên bảng của nó ( kiểu dữ liệu ) -> string




            var address = form["Address"];

            var renter = db.Renters.FirstOrDefault(p => p.RID == b);

            if (renter != null)
            {
                renter.Address = address;

                db.Entry(renter).State = EntityState.Modified;
                Session["Renter"] = renter;
            }


            db.SaveChanges();
            return RedirectToAction("Index");


        }





        // POST: Renters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RID,Money,Address,AccountID")] Renter renter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(renter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", renter.AccountID);
            return View(renter);
        }

        // GET: Renters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Renter renter = db.Renters.Find(id);
            if (renter == null)
            {
                return HttpNotFound();
            }
            return View(renter);
        }

        // POST: Renters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Renter renter = db.Renters.Find(id);
            db.Renters.Remove(renter);
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
