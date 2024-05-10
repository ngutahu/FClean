using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectFClean.Models;
using static ProjectFClean.Controllers.HomeController;

namespace ProjectFClean.Controllers
{
    public class PostsController : Controller
    {
        private readonly ProjectFClean1Entities db = new ProjectFClean1Entities();

        public class PostIndexViewModel
        {
            public List<Post> ListPost { get; set; }
            public List<Service> ListService { get; set; }
            public List<Apply> ListApply { get; set; }
            public int? SortOption { get; set; }
        }


        // GET: Posts
        public ActionResult Index()
        {

            var viewModelPost = new PostIndexViewModel
            {
                ListPost = db.Posts.ToList(),
                ListService = db.Services.ToList(),
            };

            // Lấy thông tin về người đăng nhập từ Session
            var loggedInUser = (Account)Session["Account"];

            // Kiểm tra xem người đăng nhập có vai trò là Housekeeper không
            if (loggedInUser != null)
            {
                if (loggedInUser.Role == "Housekeeper")
                {
                    // Lấy tên của người đăng bài từ bảng Account và gán vào mỗi bài đăng
                    foreach (var post in viewModelPost.ListPost)
                    {
                        post.Account = db.Accounts.FirstOrDefault(a => a.AccountID == post.Account.AccountID);
                    }
                }
                else if (loggedInUser.Role == "Renter")
                {
                    // Lấy tên của người đăng bài từ bảng Account và gán vào mỗi bài đăng
                    foreach (var post in viewModelPost.ListPost)
                    {
                        post.Account = db.Accounts.FirstOrDefault(a => a.AccountID == post.Account.AccountID);
                    }
                }
            }
            return View(viewModelPost);
        }

        // GET: Posts/Details/5
        public ActionResult DetailsPost(int pid = 0)
        {
            if (pid == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(pid);
            if (post == null)
            {
                return HttpNotFound();
            }

            // Lấy thông tin về người đăng nhập từ Session
            var loggedInUser = (Account)Session["Account"];
            // Kiểm tra xem người đăng nhập có phải là chủ bài đăng không
            bool isOwner = false; // Biến này sẽ kiểm tra xem người dùng hiện tại có phải là chủ bài đăng không

            if (loggedInUser != null)
            {
                // Nếu người dùng hiện tại là chủ bài đăng
                if (/*loggedInUser.AccountID != null &&*/ loggedInUser.AccountID == post.AccountID)
                {
                    isOwner = true;
                }
            }
            else
            {
                isOwner = false;
            }
            // Lấy danh sách các đơn apply cho bài viết này
            var applies = db.Applies.Where(a => a.PID == pid).ToList();

            // Truyền thông tin bài viết và danh sách đơn apply vào ViewBag để sử dụng trong View
            ViewBag.Post = post;
            ViewBag.Applies = applies;
            ViewBag.IsOwner = isOwner; // Gửi biến bool này vào View để sử dụng trong quyết định hiển thị các nút

            return View("Details", post);
        }


        public ActionResult Create()
        {
            var loggedInUser = (Account)Session["Account"];
            if (loggedInUser == null)
            {
                TempData["ErrorMessage"] = "You need to login to create a post.";
                return RedirectToAction("Login", "Accounts");
            }

            var viewModel = new PostIndexViewModel
            {
                ListService = db.Services.ToList(),
                ListPost = new List<Post>()
            };

            // Tạo danh sách các mục để chọn từ ListService và gán cho "ServiceID"
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name");
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "Name_of_service");

            // Truyền một đối tượng Post vào View
            return View(new Post());
        }


        // POST: Posts/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceID,Price,Location,Gender,Age,Experience,Description")] Post post)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin tài khoản người dùng đăng nhập từ Session
                var loggedInUser = (Account)Session["Account"];


                // Lấy thông tin Housekeeper từ cơ sở dữ liệu
                var account = db.Accounts.FirstOrDefault(h => h.AccountID == loggedInUser.AccountID);
                if (account != null)
                {

                    post.AccountID = account.AccountID;
                }


                post.DatePost = DateTime.Now;
                try
                {
                    // Thêm bài đăng vào cơ sở dữ liệu
                    db.Posts.Add(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    ModelState.AddModelError("", "An error occurred while saving the post: " + ex.Message);
                    return View(post);
                };
            }

            // Nếu ModelState không hợp lệ, trả về View với dữ liệu post để người dùng nhập lại
            return View(post);
        }



        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách dịch vụ từ cơ sở dữ liệu
            var services = db.Services.ToList();
            var accounts = db.Accounts.ToList();

            // Tạo một SelectList từ danh sách tài khoản
            ViewBag.AccountID = new SelectList(accounts, "AccountID", "Name", post.AccountID);

            // Tạo một SelectList từ danh sách dịch vụ
            ViewBag.ServiceID = new SelectList(services, "ServiceID", "Name_of_service", post.ServiceID);
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PID,ServiceID,Price,Location,Gender,Age,Experience,Description,RID,HID,DatePost,ApplyID,AccountID")] Post post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem bản ghi cần cập nhật vẫn tồn tại trong cơ sở dữ liệu
                    var existingPost = db.Posts.Find(post.PID);
                    if (existingPost == null)
                    {
                        // Nếu không tìm thấy bản ghi, trả về một lỗi hoặc thực hiện một hành động phù hợp
                        return HttpNotFound();
                    }

                    // Cập nhật thông tin của bài đăng trong cơ sở dữ liệu
                    db.Entry(existingPost).CurrentValues.SetValues(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    ModelState.AddModelError("", "An error occurred while saving the post: " + ex.Message);
                    return View(post);
                }
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Name", post.AccountID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "Name_of_service", post.ServiceID);
            // Nếu ModelState không hợp lệ, trả về view để hiển thị thông báo lỗi
            return View(post);
        }




        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //POST: Posts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound(); // Trả về HTTP 404 nếu bài đăng không tồn tại
            }
            // Xóa tất cả các đơn apply liên kết với bài viết này trước
            var applies = db.Applies.Where(a => a.PID == id).ToList();
            foreach (var apply in applies)
            {
                db.Applies.Remove(apply);
            }
            db.Posts.Remove(post);
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
        [HttpGet]
        public PartialViewResult Search(string gender, string location, string name, string service)
        {
            try
            {
                // Lọc danh sách Post dựa trên các tham số tìm kiếm
                IQueryable<Post> posts = db.Posts;

                if (!string.IsNullOrEmpty(gender))
                {
                    posts = posts.Where(p => p.Gender == gender);
                }

                if (!string.IsNullOrEmpty(location))
                {
                    posts = posts.Where(p => p.Location == location);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    posts = posts.Where(p => p.Account.Name.Contains(name));
                }

                if (!string.IsNullOrEmpty(service))
                {
                    // Kiểm tra xem Post có chứa dịch vụ service trong danh sách dịch vụ của mình hay không
                    posts = posts.Where(p => p.Service.Name_of_service.Contains(service));
                }

                List<Post> searchResult = posts.ToList();

                var viewModel = new PostIndexViewModel
                {
                    ListPost = searchResult,
                    ListService = db.Services.ToList()
                };

                return PartialView("_PostList", viewModel);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về kết quả phù hợp
                return PartialView("_Error", ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult Sort(int sortOption, int? page)
        {
            ViewBag.SortOption = sortOption;
            int pageSize = 6; // Số bài đăng muốn hiển thị trên mỗi trang
            int pageNumber = page ?? 1;

            // Lưu giá trị sortOption vào Session
            Session["SortOption"] = sortOption;

            // Lấy danh sách bài đăng đã được sắp xếp theo option được chọn
            var sortedPosts = SortPosts(sortOption);

            // Skip và Take dữ liệu cho từng trang
            var PostsOnPage = sortedPosts
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

            // Trả về danh sách bài đăng đã được sắp xếp và phân trang dưới dạng PartialView
            return PartialView("_PostList", PostsOnPage);
        }

        private IQueryable<Post> SortPosts(int sortOption)
        {
            IQueryable<Post> posts = db.Posts;

            switch (sortOption)
            {
                case 1: // Giá tăng dần
                    posts = posts.OrderBy(p => p.Price);
                    break;
                case 2: // Giá giảm dần
                    posts = posts.OrderByDescending(p => p.Price);
                    break;
                case 3: // Ngày đăng mới nhất
                    posts = posts.OrderByDescending(p => p.DatePost);
                    break;
                default:
                    posts = posts.OrderBy(p => p.PID);
                    break;
            }

            return posts;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(int postId)
        {
            // Lấy thông tin về người đăng nhập từ Session
            var loggedInUser = (Account)Session["Account"];

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (loggedInUser == null)
            {
                TempData["ErrorMessage"] = "You need to login to apply for this post.";
                return RedirectToAction("Login", "Accounts");
            }

            try
            {
                // Tạo một đơn apply mới
                var newApply = new Apply
                {
                    PID = postId, // Gán ID của bài post mà đơn apply được gửi đến
                    AccountID = loggedInUser.AccountID, // Gán ID của người dùng gửi đơn apply (nếu cần)
                    Status = "Pending", // Gán trạng thái của đơn apply
                    DateApply = DateTime.Now
                };

                // Thêm đơn apply mới vào danh sách applies của bài post
                var post = db.Posts.Find(postId);

                if (post != null)
                {
                    post.Applies.Add(newApply);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Your application has been submitted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to submit application. Post not found.";
                }

            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = "An error occurred while processing your application: " + ex.Message;
            }
            // Xóa TempData sau khi sử dụng
            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");

            // Sau khi thêm đơn apply, chuyển hướng người dùng đến trang chi tiết bài post
            return RedirectToAction("Index", new { pid = postId });
        }

        [HttpPost]
        public ActionResult ConfirmApply(int applyId)
        {
            try
            {
                // Tìm đơn apply trong cơ sở dữ liệu bằng applyId
                var apply = db.Applies.FirstOrDefault(a => a.ApplyID == applyId);

                if (apply != null)
                {
                    if (apply.Status == "Confirmed")
                    {
                        return Json(new { success = false, message = "This application has already been confirmed." });
                    }
                    // Cập nhật trạng thái của apply và lưu vào cơ sở dữ liệu
                    apply.Status = "Confirmed";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    // Nếu không tìm thấy đơn apply, trả về thông báo lỗi
                    return Json(new { success = false, message = "Apply not found." });
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CancelApply(int applyId)
        {
            try
            {
                // Tìm đơn apply trong cơ sở dữ liệu bằng applyId
                var apply = db.Applies.FirstOrDefault(a => a.ApplyID == applyId);

                if (apply != null)
                {
                    if (apply.Status == "Canceled")
                    {
                        return Json(new { success = false, message = "This application has already been canceled." });
                    }

                    // Cập nhật trạng thái của apply và lưu vào cơ sở dữ liệu
                    apply.Status = "Canceled";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    // Nếu không tìm thấy đơn apply, trả về thông báo lỗi
                    return Json(new { success = false, message = "Apply not found." });
                }
            }
            catch (Exception ex)
            {

                // Xử lý ngoại lệ nếu có
                return Json(new { success = false, message = ex.Message });
            }
        }



    }

}

