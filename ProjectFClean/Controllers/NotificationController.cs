using ProjectFClean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFClean.Controllers
{
    public class NotificationController : Controller
    {
        private ProjectFClean1Entities db = new ProjectFClean1Entities();

        // Các hàm khác ở đây...

        // GET: Housekeepers/SendNotification
        public ActionResult SendNotification()
        {
            // Lấy danh sách thông báo từ cơ sở dữ liệu
            var notifications = db.Notifications.ToList();

            // Trả về view với danh sách thông báo
            return View(notifications);
        }

    }
}