using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms.VisualStyles;
using ProjectFClean.Models;

namespace ProjectFClean.Controllers
{
    public class HousekeepersController : Controller
    {
        private ProjectFClean1Entities db = new ProjectFClean1Entities();

        // GET: Housekeepers
        public ActionResult Index()
        {
            var account = Session["Account"] as ProjectFClean.Models.Account;
            var housekeepers = db.Housekeepers.ToList();
            if (account != null && housekeepers != null)
            {
                var housekeeperFound = housekeepers.SingleOrDefault(h => h.AccountID == account.AccountID);
                if (housekeeperFound != null)
                {
                    return View(housekeeperFound);
                }
            }
            return RedirectToAction("Login", "Accounts");
        }

        // GET: Housekeepers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Housekeeper housekeeper = db.Housekeepers.Find(id);
            if (housekeeper == null)
            {
                return HttpNotFound();
            }
            return View(housekeeper);
        }

        // GET: Housekeepers/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name");
            return View();
        }

        // POST: Housekeepers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HID,Age,Gender,Price,Skill,Experiment,Description,Money,Address,AccountID")] Housekeeper housekeeper)
        {
            if (ModelState.IsValid)
            {
                db.Housekeepers.Add(housekeeper);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", housekeeper.AccountID);
            return View(housekeeper);
        }


        //[HttpPost, ActionName("UpdateProfile")]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdateProfile(FormCollection form)
        //{

        //    // Extracting values from the form
        //    // Form chứa một khối các data => cách lấy data của từng đối tượng là form["tên đối tượng] nhưng dạng nguyên bảng của nó ( kiểu dữ liệu ) -> string

        //    return RedirectToAction("Index");


        //}
        [HttpPost, ActionName("UpdateProfile")]
        [ValidateAntiForgeryToken]
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

            string idStr = form["Id"];
            string[] parts = idStr.Split(',');

            int a, b;
            int.TryParse(parts[0].Trim(), out a);
            int.TryParse(parts[1].Trim(), out b);

            var name = form["Name"];
            var email = form["Email"];
            var phone = form["Phone"];


            int idInt;
            if (int.TryParse(idStr, out idInt))
            {
                // Chuyển đổi thành công, idInt chứa giá trị số nguyên
            }
            else
            {
                // Không thể chuyển đổi, xử lý tùy ý
            }
            //if (!int.TryParse(idStr, out id))
            //{
            //    return RedirectToAction("Error");
            //}
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
            // ươới
            var hidStr = form["HId"];
            var ageStr = form["Age"];
            var priceStr = form["Price"];
            var experimentStr = form["Experiment"];
            int hid;
            int age;
            int price;
            int experiment;
            // Vì nguyên bản là string nên tuổi nó đang là string -> cần parse qua int => khi parse sẽ có khả năng lỗi ( ví dụ a -> Int.parse(a) -> lỗi nên cần check lỗi như dưới
            if (!int.TryParse(hidStr, out hid) || !int.TryParse(ageStr, out age))
            {
                return RedirectToAction("Error");
            }

            if (!int.TryParse(priceStr, out price))
            {
                price = 0;
            }
            if (!int.TryParse(experimentStr, out experiment))
            {
                experiment = 0;
            }
            var gender = form["Gender"];
            var skill = form["Skill"];

            var description = form["Description"];
            var address = form["Address"];

            var houseKeeper = db.Housekeepers.FirstOrDefault(p => p.HID == hid);

            if (houseKeeper != null)
            {
                houseKeeper.Age = age;
                houseKeeper.Price = price;
                houseKeeper.Gender = gender;
                houseKeeper.Skill = skill;
                houseKeeper.Experiment = experiment;
                houseKeeper.Description = description;
                houseKeeper.Address = address;
                db.Entry(houseKeeper).State = EntityState.Modified;
                Session["Housekeeper"] = houseKeeper;
            }
            else
            {
                houseKeeper = new Housekeeper()
                {
                    Age = age,
                    Price = price,
                    Gender = gender,
                    Skill = skill,
                    Experiment = experiment,
                    Description = description,
                    Address = address
                };
                Session["Housekeeper"] = houseKeeper;
                db.Housekeepers.Add(houseKeeper);
            }
            db.SaveChanges();
            return RedirectToAction("Index");



        }

        // GET: Housekeepers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Housekeeper housekeeper = db.Housekeepers.Find(id);
            if (housekeeper == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", housekeeper.AccountID);
            return View(housekeeper);
        }

        // POST: Housekeepers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HID,Age,Gender,Price,Skill,Experiment,Description,Money,Address,AccountID")] Housekeeper housekeeper)
        {
            if (ModelState.IsValid)
            {
                db.Entry(housekeeper).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", housekeeper.AccountID);
            return View(housekeeper);
        }

        // GET: Housekeepers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Housekeeper housekeeper = db.Housekeepers.Find(id);
            if (housekeeper == null)
            {
                return HttpNotFound();
            }
            return View(housekeeper);
        }

        // POST: Housekeepers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Housekeeper housekeeper = db.Housekeepers.Find(id);
            db.Housekeepers.Remove(housekeeper);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ChangeAvt(HttpPostedFileBase newAvatar)
        {
            if (newAvatar != null && newAvatar.ContentLength > 0)
            {
                if (newAvatar.ContentType == "image/jpeg" || newAvatar.ContentType == "image/png" || newAvatar.ContentType == "image/gif")
                {
                    var account = Session["Account"] as ProjectFClean.Models.Account;
                    if (account != null)
                    {
                        string imgHPath = Server.MapPath("~/img/");
                        if (!Directory.Exists(imgHPath))
                        {
                            Directory.CreateDirectory(imgHPath);
                        }
                        // Xóa ảnh cũ trước khi cập nhật ảnh mới
                        string oldImagePath = Server.MapPath("~/img/" + account.ImgAvatarID.ToString() + ".jpg");
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        // Tạo tên tệp mới dựa trên ID của tài khoản và phần mở rộng của tệp
                        string fileName = account.ImgAvatarID.ToString() + ".jpg";
                        string filePath = Path.Combine(imgHPath, fileName);
                        newAvatar.SaveAs(filePath);

                        // Cập nhật đường dẫn ảnh trong cơ sở dữ liệu
                        using (var db = new ProjectFClean1Entities())
                        {
                            account.ImgAvatarID = account.ImgAvatarID; // Giữ nguyên ID ảnh
                            db.Entry(account).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        // Xử lý khi không tìm thấy tài khoản
                    }
                }
                else
                {
                    // Xử lý khi tệp không phải là ảnh
                }
            }
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






        //// POST: Housekeepers/Delete/5
        //[HttpPost, ActionName("ChangeAvt")]
        //public ActionResult ChangeAvt(HttpPostedFileBase newAvatar) 
        //{
        //    if (newAvatar != null)
        //    {
        //        // Đường dẫn đến thư mục imgH
        //        string imgHPath = Server.MapPath("~/img/");

        //        // Tạo thư mục nếu chưa tồn tại
        //        if (!Directory.Exists(imgHPath))
        //        {
        //            Directory.CreateDirectory(imgHPath);
        //        }
        //        var account = Session["Account"] as ProjectFClean.Models.Account;

        //        string fileName = account.Name ;

        //        string fileExtension = Path.GetExtension(newAvatar.FileName);

        //        string filePath = Path.Combine(imgHPath, fileName + fileExtension);

        //        using (FileStream fileToSave = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //        {
        //            newAvatar.InputStream.CopyTo(fileToSave);
        //        }
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}
