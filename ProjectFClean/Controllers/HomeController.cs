using ProjectFClean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFClean.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectFClean1Entities db;
        public HomeController()
        {
            db = new ProjectFClean1Entities();
        }
        public class HomeIndexViewModel
        {
            public List<Account> ListAccounts { get; set; }//
            public List<Notification> ListNotifications { get; set; }//

            public List<Housekeeper> ListHousekeeper { get; set; }
            public List<Service> ListService { get; set; }
            public List<Post> ListPost { get; set; }
            public int TotalPages { get; set; }
            public int CurrentPage { get; set; }
            public int? SortOption { get; set; } // Thêm thuộc tính SortOption

        }
        //public ActionResult Index(int? page)
        //{
        //    int pageSize = 6; // Số housekeeper muốn hiển thị trên mỗi trang
        //    int pageNumber = page ?? 1;

        //    var viewModel = new HomeIndexViewModel
        //    {
        //        ListHousekeeper = db.Housekeeper
        //                             .OrderBy(h => h.HID) // Sắp xếp theo một trường nhất định
        //                             .Skip((pageNumber - 1) * pageSize)
        //                             .Take(pageSize)
        //                             .ToList(),
        //        ListService = db.Service.ToList(),
        //        ListPost = db.Post.ToList(),
        //        CurrentPage = pageNumber // Gán giá trị cho thuộc tính CurrentPage
        //    };

        //    // Tính tổng số housekeeper
        //    int totalHousekeepers = db.Housekeeper.Count();
        //    // Tính tổng số trang bằng cách chia tổng số housekeeper cho kích thước trang
        //    viewModel.TotalPages = (int)Math.Ceiling((double)totalHousekeepers / pageSize);

        //    return View(viewModel);
        //}

        public ActionResult Index(int? page)
        {
            int pageSize = 3; // Số housekeeper muốn hiển thị trên mỗi trang
            int pageNumber = page ?? 1;

            // Lấy các tham số từ session
            string gender = Session["Gender"] as string;
            string location = Session["Location"] as string;
            string name = Session["Name"] as string;
            string service = Session["Service"] as string;
            int? sortOption = Session["SortOption"] as int?;

            // Lấy danh sách housekeepers từ cơ sở dữ liệu
            IQueryable<Housekeeper> housekeepersQuery = db.Housekeepers;

            // Áp dụng các tham số search từ session
            if (!string.IsNullOrEmpty(gender))
            {
                housekeepersQuery = housekeepersQuery.Where(h => h.Gender == gender);
            }

            if (!string.IsNullOrEmpty(location))
            {
                housekeepersQuery = housekeepersQuery.Where(h => h.Address == location);
            }

            if (!string.IsNullOrEmpty(name))
            {
                housekeepersQuery = housekeepersQuery.Where(h => h.Account.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(service))
            {
                housekeepersQuery = housekeepersQuery.Where(h => h.Skill.Contains(service));
            }

            // Áp dụng các tham số sort từ session
            if (sortOption != null)
            {
                switch (sortOption)
                {
                    case 1: // Giá tăng dần
                        housekeepersQuery = housekeepersQuery.OrderBy(h => h.Price);
                        break;
                    case 2: // Giá giảm dần
                        housekeepersQuery = housekeepersQuery.OrderByDescending(h => h.Price);
                        break;
                    case 3: // Kinh nghiệm giảm dần
                        housekeepersQuery = housekeepersQuery.OrderByDescending(h => h.Experiment);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Sắp xếp mặc định nếu không có tùy chọn
                housekeepersQuery = housekeepersQuery.OrderBy(h => h.HID);
            }

            // Lấy số lượng housekeepers trong trang hiện tại
            List<Housekeeper> housekeepersInPage = housekeepersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tạo view model
            var viewModel = new HomeIndexViewModel
            {
                ListAccounts = db.Accounts.ToList(),
                ListNotifications = db.Notifications.ToList(),
                ListHousekeeper = housekeepersInPage,
                ListService = db.Services.ToList(),
                CurrentPage = pageNumber,
                SortOption = sortOption,
                ListPost = db.Posts.ToList()
            };

            // Tính tổng số housekeeper
            int totalHousekeepers = db.Housekeepers.Count();
            // Tính tổng số trang bằng cách chia tổng số housekeeper cho kích thước trang
            viewModel.TotalPages = (int)Math.Ceiling((double)totalHousekeepers / pageSize);

            return View(viewModel);
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Logout()
        {
            Session.Clear(); // Xóa tất cả các giá trị trong session
            Session.Abandon(); // Hủy session hiện tại
            return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chủ
        }
        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult Details(int HID = 0)
        {
            Housekeeper housekeeper = db.Housekeepers.Find(HID);
            if (housekeeper == null)
            {
                return HttpNotFound();
            }
            return View("DetailsHouseKeeper", housekeeper);
        }

        public ActionResult DetailsRenter1(int RID = 0)
        {
            Renter renter = db.Renters.Find(RID);
            if (renter == null)
            {
                return HttpNotFound();
            }
            return View("DetailsRenter", renter);
        }

        [HttpGet]
        public PartialViewResult Search(string gender, string location, string name, string service)
        {
            // Lọc danh sách HouseKeeper dựa trên các tham số tìm kiếm
            IQueryable<Housekeeper> housekeepers = db.Housekeepers;

            if (!string.IsNullOrEmpty(gender))
            {
                housekeepers = housekeepers.Where(h => h.Gender == gender);
            }

            if (!string.IsNullOrEmpty(location))
            {
                housekeepers = housekeepers.Where(h => h.Address == location);
            }

            if (!string.IsNullOrEmpty(name))
            {
                housekeepers = housekeepers.Where(h => h.Account.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(service))
            {
                // Kiểm tra xem housekeeper có chứa kỹ năng service trong danh sách kỹ năng của mình hay không
                housekeepers = housekeepers.Where(h => h.Skill.Contains(service));
            }

            List<Housekeeper> searchResult = housekeepers.ToList();

            return PartialView("_HousekeeperList", searchResult);

        }


        [HttpGet]
        public PartialViewResult Sort(int sortOption, int? page)
        {
            ViewBag.SortOption = sortOption;
            int pageSize = 3; // Số housekeeper muốn hiển thị trên mỗi trang
            int pageNumber = page ?? 1;

            // Lưu giá trị sortOption vào Session
            Session["SortOption"] = sortOption;

            // Lấy danh sách housekeepers đã được sắp xếp theo option được chọn
            var sortedHousekeepers = SortHousekeepers(sortOption);

            // Skip và Take dữ liệu theo trang
            var housekeepersOnPage = sortedHousekeepers
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

            // Return the sorted and paginated list as a partial view
            return PartialView("_HousekeeperList", housekeepersOnPage);
        }
        private IQueryable<Housekeeper> SortHousekeepers(int sortOption)
        {
            IQueryable<Housekeeper> housekeepers = db.Housekeepers;

            switch (sortOption)
            {
                case 1: // Price Ascending
                    housekeepers = housekeepers.OrderBy(h => h.Price);
                    break;
                case 2: // Price Decreasing
                    housekeepers = housekeepers.OrderByDescending(h => h.Price);
                    break;
                case 3: // Experience Decreasing
                    housekeepers = housekeepers.OrderByDescending(h => h.Experiment);
                    break;
                default:
                    housekeepers = housekeepers.OrderBy(h => h.HID);
                    break;
            }

            return housekeepers;
        }
    }
}