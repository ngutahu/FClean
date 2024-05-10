using ProjectFClean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ProjectFClean.Controllers
{
    public class AccountsController : Controller
    {
        ProjectFClean1Entities db = new ProjectFClean1Entities();



        // GET: Accounts
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account account, Renter renter, Housekeeper housekeeper)
        {
            // Kiểm tra xem email đã tồn tại hay chưa
            if (CheckEmail(account.Email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View(account); // Trả về View với dữ liệu đã nhập để người dùng có thể sửa đổi và thử lại
            }

            // Thêm tài khoản vào cơ sở dữ liệu
            db.Accounts.Add(account);
            db.SaveChanges();

            var accId = account.AccountID;

            // Tạo và thêm thông tin người thuê nếu vai trò là "Renter"
            if (account.Role == "Renter")
            {
                Renter renterAdd = new Renter
                {
                    AccountID = accId,
                    Address = renter.Address,
                    Age = renter.Age, // Thêm trường Age
                    Gender = renter.Gender, // Thêm trường Gender
                    Description = renter.Description, // Thêm trường Description
                    CreateDate = DateTime.Now,
                };
                db.Renters.Add(renterAdd);
            }
            // Tạo và thêm thông tin người giúp việc nếu vai trò là "Housekeeper"
            else if (account.Role == "Housekeeper")
            {
                Housekeeper housekeeperAdd = new Housekeeper
                {
                    AccountID = accId,
                    Address = housekeeper.Address,
                    Age = housekeeper.Age,
                    Gender = housekeeper.Gender,
                    Price = housekeeper.Price,
                    Skill = housekeeper.Skill,
                    Experiment = 0,
                    Description = housekeeper.Description,
                    CreateDate = DateTime.Now,
                };
                db.Housekeepers.Add(housekeeperAdd);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful. Please login.";
            return RedirectToAction("Login", "Accounts");
        }

        private bool CheckEmail(string email)
        {
            return db.Accounts.Any(x => x.Email == email);
        }

        public ActionResult Login()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(Account account)
        {
            var Email = account.Email;
            var Password = account.Password;
            var CheckUser = db.Accounts.SingleOrDefault(x => x.Email.Equals(Email) && x.Password.Equals(Password));
            if (CheckUser != null)
            {
                // Kiểm tra trạng thái của người dùng
                string status = CheckUser.Approve;
                if (status != null)
                {
                    if (status == "Yes")
                    {
                        // Kiểm tra role của người dùng
                        string role = CheckUser.Role;
                        if (role != null && (role == "Housekeeper" || role == "Renter"))
                        {
                            Session["Account"] = CheckUser;
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role != null && role == "Admin")
                        {
                            Session["Account"] = CheckUser;
                            return RedirectToAction("Chart", "Admin");
                        }
                        else
                        {
                            // Trường hợp role không hợp lệ
                            ViewBag.LoginFail = "Invalid role. Please contact administrator.";
                            return View("Login");
                        }
                    }
                    else if (status == "No")
                    {
                        ViewBag.LoginFail = "Your account has not been approved.";
                        return View("Login");
                    }
                    else if (status == "Lock")
                    {
                        ViewBag.LoginFail = "Your account has been locked.";
                        return View("Login");
                    }
                }
                else
                {
                    ViewBag.LoginFail = "Invalid account status.";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.LoginFail = "Login failed. Try again.";
                return View("Login");
            }
            ViewBag.LoginFail = "Login failed. Try again.";
            return View("Login");
        }
        public ActionResult AccountDetail(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

    }
}