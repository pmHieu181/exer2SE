using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStoreApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Message = "Chào mừng đến Dashboard chính!";
            // Bạn có thể lấy thông tin người dùng từ Session nếu cần
            ViewBag.FullName = Session["FullName"] as string;
            return View();
        }
    }
}