using ProjectFClean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Web.DynamicData;
using System.Data.Entity.Validation;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using System.Globalization;

namespace ProjectFClean.Controllers
{

    public class AdminController : Controller
    {
        ProjectFClean1Entities db = new ProjectFClean1Entities();

        public List<Account> accounts = new List<Account>();

        private bool IsAdminLoggedIn()
        {
            return Session["Account"] != null && ((Account)Session["Account"]).Role == "Admin";
        }

        // Kiểm tra quyền truy cập trước khi thực hiện một hành động trong AdminController
        private ActionResult CheckAdminAccess()
        {
            if (!IsAdminLoggedIn())
            {
                // Người dùng chưa đăng nhập hoặc không có quyền truy cập, chuyển hướng đến trang đăng nhập hoặc trang không có quyền truy cập
                return RedirectToAction("Login", "Accounts");
            }
            return null; // Người dùng có quyền truy cập
        }

        // Sử dụng trong các phương thức khác
        public ActionResult Index()
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }

            // Người dùng đã đăng nhập và có vai trò là "Admin", cho phép truy cập vào trang admin
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            Account account = db.Accounts.SingleOrDefault(n => n.AccountID == id);
            ViewBag.AccountID = account.AccountID;
            if (account == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {

            var renters = db.Renters.Where(r => r.AccountID == id);
            db.Renters.RemoveRange(renters);


            Account account = db.Accounts.SingleOrDefault(n => n.AccountID == id);
            if (account == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Accounts.Remove(account);
            db.SaveChanges();

            return RedirectToAction("User");
        }

        public ActionResult Edit(int id)
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            Account account = db.Accounts.SingleOrDefault(n => n.AccountID == id);
            if (account == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(account);
        }

        public ActionResult User()
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }


            var approvedAccounts = db.Accounts.Where(a => a.Approve == "Yes").OrderBy(n => n.AccountID);
            return View("User", approvedAccounts);
        }

        public ActionResult Verification()
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            var accountsWithNoApprove = db.Accounts.Where(a => a.Approve == "No").OrderBy(n => n.AccountID).ToList();

            return View(accountsWithNoApprove);
        }

        public ActionResult VeriConfirm(int id = 0)
        {
            var account = db.Accounts.SingleOrDefault(n => n.AccountID == id);

            if (account == null)
            {
                return HttpNotFound();
            }

            account.Approve = "Yes";
            db.SaveChanges();
            return RedirectToAction("Verification");
        }


        public ActionResult AccountDetail(int id = 0)
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            var account = db.Accounts.SingleOrDefault(n => n.AccountID == id);

            if (account == null)
            {
                return HttpNotFound();
            }

            var renterDetail = db.Renters.FirstOrDefault(r => r.AccountID == id);
            var housekeeperDetail = db.Housekeepers.FirstOrDefault(h => h.AccountID == id);

            ViewBag.RenterDetail = renterDetail;
            ViewBag.HousekeeperDetail = housekeeperDetail;

            return View("AccountDetail", account);
        }

        public ActionResult LockAcc()
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            var accountsWithNoApprove = db.Accounts.Where(a => a.Approve == "Lock").OrderBy(n => n.AccountID).ToList();

            return View(accountsWithNoApprove);
        }

        public ActionResult Lock(int id = 0)
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            var account = db.Accounts.SingleOrDefault(n => n.AccountID == id);

            if (account == null)
            {
                return HttpNotFound();
            }

            account.Approve = "Lock";
            db.SaveChanges();
            return RedirectToAction("User");
        }

        public ActionResult UnLock(int id = 0)
        {
            var account = db.Accounts.SingleOrDefault(n => n.AccountID == id);

            if (account == null)
            {
                return HttpNotFound();
            }

            account.Approve = "Yes";
            db.SaveChanges();
            return RedirectToAction("LockAcc");
        }
        public ActionResult Chart()
        {
            ActionResult accessResult = CheckAdminAccess();
            if (accessResult != null)
            {
                return accessResult;
            }
            ViewBag.Months = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "January", Value = "1" },
            new SelectListItem() { Text = "February", Value = "2" },
            new SelectListItem() { Text = "February", Value = "3" },
            new SelectListItem() { Text = "February", Value = "4" },
            new SelectListItem() { Text = "February", Value = "5" },
            new SelectListItem() { Text = "February", Value = "6" },
            new SelectListItem() { Text = "February", Value = "7" },
            new SelectListItem() { Text = "February", Value = "8" },
            new SelectListItem() { Text = "February", Value = "9" },
            new SelectListItem() { Text = "February", Value = "10" },
            new SelectListItem() { Text = "February", Value = "11" },
            new SelectListItem() { Text = "December", Value = "12" }
        };
            ViewBag.Year = DateTime.Now.Year;
            return View();

        }

        public ActionResult GetTotalAccounts()
        {
            var totalAccounts = db.Accounts.Count();
            return Json(totalAccounts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTotalHousekeepers()
        {
            var totalHousekeepers = db.Housekeepers.Count();
            return Json(totalHousekeepers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTotalRenters()
        {
            var totalRenters = db.Renters.Count();
            return Json(totalRenters, JsonRequestBehavior.AllowGet);
        }
       

        //[HttpGet]
        //public JsonResult GetChartData(int year, int month)
        //{
        //    try
        //    {
        //        var housekeeperData = db.Housekeepers
        //            .Where(x => x.CreateDate.Month == month && x.CreateDate.Year == year)
        //            .GroupBy(x => x.CreateDate.Day)
        //            .Select(x => new { Date = new DateTime(x.Key, month, year), Count = x.Count() })
        //            .ToList();

        //        var renterData = db.Renters
        //            .Where(x => x.CreateDate.Month == month && x.CreateDate.Year == year)
        //            .GroupBy(x => x.CreateDate.Day)
        //            .Select(x => new { Date = new DateTime(x.Key, month, year), Count = x.Count() })
        //            .ToList();

        //        var labels = new List<string>();
        //        for (var i = 1; i <= DateTime.DaysInMonth(year, month); i++)
        //        {
        //            labels.Add(new DateTime(year, month - 1, i).ToString("dd")); // Use "dd" for day only

        //        }

        //        var renterCounts = new List<int>();
        //        foreach (var item in renterData)
        //        {
        //            renterCounts.Add(item.Count);
        //        }

        //        var housekeeperCounts = new List<int>();
        //        foreach (var item in housekeeperData)
        //        {
        //            housekeeperCounts.Add(item.Count);
        //        }

        //        return Json(new { labels, renterCounts, housekeeperCounts }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Lỗi truy vấn dữ liệu:", ex);
        //        return Json(new { error = "Error occurred while fetching data" }); // Handle error with a JSON response
        //    }
        //}

        public ActionResult FeedbackHub()
        {
            var pendingFeedbacks = db.Feedbacks.Where(f => f.IsCompleted == false).ToList();
            return View(pendingFeedbacks);
        }
        
        [HttpPost]
       
        public ActionResult ApproveFeedback(int feedbackId)
        {
            var feedback = db.Feedbacks.Find(feedbackId);
            if (feedback != null)
            {
                feedback.IsCompleted = true;
              

                // Lấy thông tin compact tương ứng với feedback
                var compact = db.Compacts.FirstOrDefault(c => c.CID == feedback.CID);

                if (compact != null)
                {
                    // Cộng số tiền đã giữ vào tài khoản của Housekeeper
                    var housekeeper = db.Housekeepers.Find(compact.HID);
                    if (housekeeper != null && housekeeper.Account != null)
                    {
                        housekeeper.Account.Money += compact.Total_Price;
                        db.SaveChanges();
                    }
                }
               
            }

            return RedirectToAction("FeedbackHub");

        }



    }
}
