using System;
using System.Collections.Generic;
using System.Data.Entity; // Cần cho .Include()
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyStoreApp.Models;
using MyStoreApp.ViewModels;

namespace MyStoreApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private MyStoreDBEntities db = new MyStoreDBEntities();

        // GET: Orders (Hiển thị danh sách đơn hàng)
        public ActionResult Index()
        {
            // Lấy danh sách tất cả các đơn hàng từ database
            // Bao gồm thông tin của Đại lý (Agent) liên quan đến mỗi đơn hàng
            // Sắp xếp theo ngày đặt hàng giảm dần (đơn hàng mới nhất lên đầu)
            var orders = db.Orders.Include(o => o.Agent).OrderByDescending(o => o.OrderDate);
            return View(orders.ToList()); // Truyền danh sách List<Order> vào View "Index.cshtml"
        }

        // GET: Orders/Create (Hiển thị form để tạo đơn hàng mới)
        public ActionResult Create()
        {
            var model = new OrderViewModel(); // Khởi tạo ViewModel cho form tạo đơn hàng
            model.OrderDate = DateTime.Now; // Có thể gán ngày hiện tại làm mặc định

            // Chuẩn bị dữ liệu cho DropDownList Đại lý
            ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName");

            // Chuẩn bị danh sách sản phẩm để sử dụng trong JavaScript (thêm chi tiết đơn hàng động)
            ViewBag.Products = db.Products
                                 .Where(p => p.StockQuantity > 0) // Chỉ lấy sản phẩm còn hàng
                                 .Select(p => new { p.ProductId, p.ProductName, p.UnitPrice })
                                 .ToList();
            return View(model); // Truyền OrderViewModel vào View "Create.cshtml"
        }

        // POST: Orders/Create (Xử lý việc tạo đơn hàng mới từ form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel model) // Nhận OrderViewModel từ form
        {
            if (model.OrderDetails == null || !model.OrderDetails.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm vào đơn hàng.");
            }

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Order newOrder = new Order
                        {
                            AgentId = model.AgentId,
                            OrderDate = model.OrderDate, // Lấy ngày từ ViewModel (có thể là DateTime.Now nếu bạn muốn)
                            Status = "Pending",
                            // TotalAmount sẽ được tính toán
                        };
                        db.Orders.Add(newOrder);
                        db.SaveChanges(); // Lưu để lấy OrderId cho OrderDetails

                        decimal totalOrderAmount = 0;
                        foreach (var itemVM in model.OrderDetails)
                        {
                            if (itemVM.ProductId > 0 && itemVM.Quantity > 0)
                            {
                                var product = db.Products.Find(itemVM.ProductId);
                                if (product == null)
                                {
                                    ModelState.AddModelError("", $"Sản phẩm với ID {itemVM.ProductId} không tồn tại.");
                                    // Nếu có lỗi, cần chuẩn bị lại ViewBag và trả về View Create
                                    ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName", model.AgentId);
                                    ViewBag.Products = db.Products.Where(p => p.StockQuantity > 0).Select(p => new { p.ProductId, p.ProductName, p.UnitPrice }).ToList();
                                    transaction.Rollback(); // Quan trọng: Hủy bỏ transaction nếu có lỗi
                                    return View(model); // Trả về View "Create"
                                }

                                OrderDetail detail = new OrderDetail
                                {
                                    OrderId = newOrder.OrderId,
                                    ProductId = itemVM.ProductId,
                                    Quantity = itemVM.Quantity,
                                    PriceAtPurchase = product.UnitPrice // Lấy giá hiện tại của sản phẩm
                                };
                                db.OrderDetails.Add(detail);
                                totalOrderAmount += (itemVM.Quantity * product.UnitPrice);

                                // Tùy chọn: Cập nhật số lượng tồn kho sản phẩm
                                // product.StockQuantity -= itemVM.Quantity;
                                // if (product.StockQuantity < 0)
                                // {
                                //     ModelState.AddModelError("", $"Sản phẩm {product.ProductName} không đủ số lượng tồn kho.");
                                //     ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName", model.AgentId);
                                //     ViewBag.Products = db.Products.Where(p => p.StockQuantity > 0).Select(p => new { p.ProductId, p.ProductName, p.UnitPrice }).ToList();
                                //     transaction.Rollback();
                                //     return View(model);
                                // }
                            }
                        }
                        newOrder.TotalAmount = totalOrderAmount;
                        // Entity Framework sẽ tự động theo dõi thay đổi của newOrder.TotalAmount
                        // không cần db.Entry(newOrder).State = EntityState.Modified; ở đây.
                        db.SaveChanges(); // Lưu thay đổi TotalAmount và cập nhật số lượng tồn kho (nếu có)

                        transaction.Commit(); // Hoàn tất transaction nếu mọi thứ thành công
                        TempData["SuccessMessage"] = $"Đơn hàng #{newOrder.OrderId} đã được tạo thành công!";
                        return RedirectToAction("Details", new { id = newOrder.OrderId });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Hủy bỏ transaction nếu có lỗi trong khối try
                        ModelState.AddModelError("", "Lỗi khi tạo đơn hàng: " + ex.Message);
                        // Log lỗi ex ở đây
                    }
                }
            }

            // Nếu ModelState không hợp lệ hoặc có lỗi xảy ra, hiển thị lại form Create với dữ liệu đã nhập
            ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName", model.AgentId);
            ViewBag.Products = db.Products.Where(p => p.StockQuantity > 0).Select(p => new { p.ProductId, p.ProductName, p.UnitPrice }).ToList();
            return View(model); // Trả về View "Create" (vì action này tên là Create)
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Include(o => o.Agent)
                                   .Include(o => o.OrderDetails.Select(od => od.Product)) // Eager load chi tiết và sản phẩm của chi tiết
                                   .FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order); // Truyền đối tượng Order vào View "Details.cshtml"
        }

        // Thêm các actions Edit, Delete nếu cần theo yêu cầu bài tập

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Giải phóng DbContext
            }
            base.Dispose(disposing);
        }
    }
}
