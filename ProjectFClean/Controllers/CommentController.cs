using ProjectFClean.Models;
using ProjectFClean.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFClean.Controllers
{
    public class CommentController : Controller
    {
        private readonly ProjectFClean1Entities db;
        public CommentController()
        {
            db = new ProjectFClean1Entities();
        }
        // GET: Comment
        public ActionResult Index(int Id)
        {
            var comment = db.Comments.Where(c => c.HID == Id).ToList();
            return View(comment);

        }
        public ActionResult Indexx()
        {

            return View();
        }

        // GET: Comment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Comment/Create
        public ActionResult Create(int Id)
        {
            Comment comment = new Comment();
            comment.HID = Id;
            return View(comment);
        }

        // POST: Comment/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "HID,Content,Name")] Comment comment)
        {
            var loggedInUser = (Account)Session["Account"];
            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            if (ModelState.IsValid)
            {


                comment.CDate = DateTime.Now;
                comment.AccountID = loggedInUser.AccountID;

                db.Comments.Add(comment);

                db.SaveChanges();
                return RedirectToAction("Details", "Home", new { HID = comment.HID });
            }

            return View(comment);
        }



        [HttpPost]
        public ActionResult CreateReply(RepliCommentVM RVM)
        {
            var loggedInUser = (Account)Session["Account"];
            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            if (ModelState.IsValid)
            {
                RepliComment reply = new RepliComment();
                reply.Content = RVM.Reply;
                reply.CId = RVM.CId;
                reply.HID = RVM.HID;
                reply.CDate = DateTime.Now;
                reply.AccountID = loggedInUser.AccountID;
                db.RepliComments.Add(reply);
                db.SaveChanges();
                return RedirectToAction("Details", "Home", new { HID = reply.HID });
            }
            return View(RVM);
        }


        // GET: Comment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Comment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Comment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
