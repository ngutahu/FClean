using ProjectFClean.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFClean.Controllers
{
    public class FeedbackController : Controller
    {
        ProjectFClean1Entities db = new ProjectFClean1Entities();

        [HttpGet]
        public ActionResult Feedback()
        {
            var account = Session["Account"] as Account;
            if (account == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var feedback = new Feedback
            {
                DateTimeCreated = DateTime.Now,
                IsCompleted = false
            };

           

            return View(feedback);
        }


        [HttpPost]
 
        public ActionResult SaveFeedback(Feedback model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var account = Session["Account"] as Account;
                if (account == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                var feedback = new Feedback
                {
                    DateTimeCreated = DateTime.Now,
                    IsCompleted = false,
                    FeedbackType = model.FeedbackType
                };

                // Lưu ID của người đăng phản hồi
                if (account.Role == "Housekeeper")
                {
                    feedback.HID = account.AccountID;
                }
                else if (account.Role == "Renter")
                {
                    feedback.RID = account.AccountID;
                }

                // Lưu CID đã được nhập từ bàn phím
                feedback.CID = model.CID;
                

                feedback.Comment = model.Comment;

                if (file != null && file.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        feedback.ImageURL = binaryReader.ReadBytes(file.ContentLength);
                    }
                }

                if (model.FeedbackType == "Complaint")
                {
                    feedback.Rate = model.Rate;
                }

                db.Feedbacks.Add(feedback);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


    }
}
