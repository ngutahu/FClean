using ProjectFClean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ProjectFClean.ViewModel;

namespace ProjectFClean.Controllers
{
    public class ChatRoomController : Controller
    {
        private readonly ProjectFClean1Entities db;
        public ChatRoomController()
        {
            db = new ProjectFClean1Entities();
        }
        // GET: ChatRoom
        public ActionResult Index()
        {
            var chatRoom = db.ChatRooms.Include(c => c.Replies).OrderByDescending(c => c.ChatDate).ToList();
            return View(chatRoom);
        }

        // GET: ChatRoom/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChatRoom/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChatRoom/Create
        [HttpPost]
        public ActionResult Create(ReplyVM RVM)
        {
            var loggedInUser = (Account)Session["Account"];
            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            Reply reply = new Reply();
            reply.Text = RVM.Reply;
            reply.ChatId = RVM.CID;
            reply.ChatDate = DateTime.Now;
            reply.AccountID = loggedInUser.AccountID;
            db.Replies.Add(reply);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult CreateChat(ChatRoom chat)
        {
            var loggedInUser = (Account)Session["Account"];
            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            ChatRoom chatRoom = new ChatRoom();
            chatRoom.ChatDate = DateTime.Now;
            chatRoom.AccountID = loggedInUser.AccountID;
            chatRoom.Text = chat.Text;

            db.ChatRooms.Add(chatRoom);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ChatRoom/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChatRoom/Edit/5
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

        // GET: ChatRoom/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChatRoom/Delete/5
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
