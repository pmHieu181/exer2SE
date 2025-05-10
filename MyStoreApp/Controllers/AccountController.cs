using System.Linq;
using System.Web.Mvc;
using System.Web.Security; // Cho FormsAuthentication
using MyStoreApp.Models; // Namespace chứa model của bạn
using MyStoreApp.ViewModels;

public class AccountController : Controller
{
    private MyStoreDBEntities db = new MyStoreDBEntities(); // Context DB của bạn

    // GET: Account/Login
    [AllowAnonymous] // Cho phép truy cập không cần đăng nhập
    public ActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            // QUAN TRỌNG: Trong ứng dụng thực tế, mã hóa mật khẩu nhập vào và so sánh với hash đã lưu.
            // Để đơn giản ở đây, chúng ta giả sử mật khẩu dạng text thường (RẤT KHÔNG AN TOÀN).
            var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.PasswordHash == model.Password);
            // Cần thay thế bằng phương thức xác thực mật khẩu đúng đắn

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                Session["UserID"] = user.UserId;
                Session["Username"] = user.Username;
                Session["FullName"] = user.FullName;

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Chuyển hướng đến form chính/dashboard
                }
            }
            else
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            }
        }
        return View(model);
    }

    // GET: Account/Logout
    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();
        Session.Clear();
        Session.Abandon();
        return RedirectToAction("Login", "Account");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}