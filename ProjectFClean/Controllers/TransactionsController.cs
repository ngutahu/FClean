using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectFClean.Models;

namespace ProjectFClean.Controllers
{
    public class TransactionsController : Controller
    {
        private ProjectFClean1Entities db = new ProjectFClean1Entities();

        // GET: Transactions
        public ActionResult Index()
        {
            //var transactions = db.Transactions.Include(t => t.Account).Include(t => t.Account1);
            //return View(transactions.ToList());
            var transactionsFromDb = db.Transactions.ToList(); // Materialize the query first

            List<Transaction> transactions = transactionsFromDb.Select(t => new Transaction
            {
                TransId = t.TransId,
                ReceiveId = t.ReceiveId,
                Note = t.Note,
                Transaction_Money = t.Transaction_Money,
                DateTime = DateTime.Now,
                //TransactionType = t.TransactionType,  
                SenderId = t.SenderId
            }).ToList();

            return View(transactions);
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.ReceiveId = new SelectList(db.Accounts, "AccountID", "Name");
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountID", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransId,ReceiveId,Note,DateTime,Transaction_Money_,SenderId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReceiveId = new SelectList(db.Accounts, "AccountID", "Name", transaction.ReceiveId);
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountID", "Name", transaction.SenderId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReceiveId = new SelectList(db.Accounts, "AccountID", "Name", transaction.ReceiveId);
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountID", "Name", transaction.SenderId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransId,ReceiveId,Note,DateTime,Transaction_Money_,SenderId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReceiveId = new SelectList(db.Accounts, "AccountID", "Name", transaction.ReceiveId);
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountID", "Name", transaction.SenderId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
