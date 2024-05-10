using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using ProjectFClean.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectFClean.Controllers
{
    public class BookingController : Controller
    {
        private ProjectFClean1Entities db = new ProjectFClean1Entities();

        public ActionResult RentView(int housekeeperId)
        {
            var account = Session["Account"] as Account;
            if (account != null)
            {
                var renter = db.Renters.SingleOrDefault(h => h.AccountID == account.AccountID);
                if (renter == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                var housekeeper = db.Housekeepers.FirstOrDefault(h => h.HID == housekeeperId);
                if (housekeeper == null)
                {
                    return HttpNotFound();
                }

                var services = db.Services.ToList();

                var compact = new Compact
                {
                    RID = renter.RID,
                    HID = housekeeper.HID,
                    Signed_Date = DateTime.Now,
                    Status = "Chưa xác nhận"
                };

                var viewModel = new RentViewModel
                {
                    Housekeeper = housekeeper,
                    Renter = renter,
                    Compact = compact,
                    Services = services
                };

                return View(viewModel);
            }
            return RedirectToAction("Login", "Accounts");
        }

        [HttpPost]
        public ActionResult SubmitRent(Compact viewModel)
        {
            // Chỗ này dùng để hiện thị ra data nếu trường hợp code đi vào exception => gán data vào object này để hiện thị ra view.
            RentViewModel rentViewModel = null;
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo một đối tượng Compact từ dữ liệu được truyền từ view model   
                    var compact = new Compact
                    {
                        //RID = viewModel.Compact.RID,
                        //HID = viewModel.Compact.HID,
                        //Signed_Date = viewModel.Compact.Signed_Date,
                        //ServiceID = viewModel.Compact.ServiceID,
                        //Start_Date = viewModel.Compact.Start_Date,
                        //End_Date = viewModel.Compact.End_Date,
                        //Status = viewModel.Compact.Status
                        RID = viewModel.RID,
                        HID = viewModel.HID,
                        Signed_Date = viewModel.Signed_Date,
                        ServiceID = viewModel.ServiceID,
                        Start_Date = viewModel.Start_Date,
                        End_Date = viewModel.End_Date,
                        Status = "Pending",
                        Work_Time = viewModel.Work_Time,
                        Total_Price = viewModel.Total_Price
                    };
                    // Lưu dữ liệu vào cơ sở dữ liệu
                    db.Compacts.Add(compact);
                    //Add notification
                    var receiver = db.Housekeepers.SingleOrDefault(h => h.HID == viewModel.HID);
                    var sender = db.Renters.SingleOrDefault(r => r.RID == viewModel.RID);

                    db.Notifications.Add(new Notification
                    {
                        HID = receiver.Account.AccountID,
                        RID = sender.Account.AccountID,
                        Content = $"User Just Rent You",
                        LinkNoti = "/Booking/ConfirmBooking"
                    });
                    db.SaveChanges();

                    // Chuyển hướng người dùng đến trang chủ
                    return RedirectToAction("Index", "Home");
               
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the rent details.");
                    // Trong trường hợp có lỗi, trả về view "RentView" với dữ liệu của RentViewModel
                    rentViewModel.Services = db.Services.ToList();
                    return View(rentViewModel);
                }
            }

            // Trong trường hợp có lỗi, trả về view "RentView" với dữ liệu của RentViewModel
            rentViewModel.Services = db.Services.ToList();
            return View(rentViewModel);
        }
        public ActionResult ConfirmBooking()
        {
            var account = Session["Account"] as Account;
            var housekeeper = db.Housekeepers.SingleOrDefault(h => h.AccountID == account.AccountID);
            if (housekeeper == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            var compacts = db.Compacts.Where(c => c.HID == housekeeper.HID && c.Status == "pending").ToList();

            if (compacts.Count == 0)
            {
                return RedirectToAction("Index", "Home"); ;
            }

            return View(compacts);


        }
        [HttpPost]
        public ActionResult UpdateStatus(int compactId, string status)
        {
            var compact = db.Compacts.FirstOrDefault(c => c.CID == compactId);

            if (compact == null)
            {
                return HttpNotFound();
            }

            if (status == "confirmed")
            {
                compact.Status = "Confirmed";

                var renter = compact.Renter;
                var housekeeper = compact.Housekeeper;

                renter.Account.Money -= compact.Total_Price;

                //Add Notification
                var account = Session["Account"] as Account;
                var renterId = compact.RID;
                var receiver = db.Renters.SingleOrDefault(r => r.RID == renterId);

                db.Notifications.Add(new Notification
                {
                    HID = receiver.Account.AccountID,
                    RID = account.AccountID,
                    Content = $"User Has Been Accepted Your Compact",
                    LinkNoti = "A"
                });
                db.SaveChanges();

                // Giữ tiền trên trang web
                TempData["HeldMoney"] = compact.Total_Price;
                // Add Transaction
                var houskeeperByAccountID = db.Housekeepers.SingleOrDefault(h => h.AccountID == account.AccountID);
                var renterByAccountID = db.Accounts.SingleOrDefault(h => h.AccountID == receiver.AccountID);
                db.Transactions.Add(new Transaction
                {
                    ReceiveId = account.AccountID,
                    Note = "You Account Subtracted",
                    Transaction_Money = houskeeperByAccountID.Price,
                    DateTime = DateTime.Now,
                    SenderId = receiver.Account.AccountID,
                });

                db.SaveChanges();
            
        }
            else if (status == "cancelled")
            {
                compact.Status = "Cancelled";
                //Add Notification
                var account = Session["Account"] as Account;
                var renterId = compact.RID;
                var receiver = db.Renters.SingleOrDefault(r => r.RID == renterId);

                db.Notifications.Add(new Notification
                {
                    HID = receiver.Account.AccountID,
                    RID = account.AccountID,
                    Content = $"User Has Been Cancel Your Compact",
                    LinkNoti = "A"
                });
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }



    }



}

