using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStoreApp.Models; // Đảm bảo bạn có using này để truy cập MyStoreDBEntities và các model khác
using MyStoreApp.ViewModels; // Namespace cho ViewModel của bạn

namespace MyStoreApp.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private MyStoreDBEntities db = new MyStoreDBEntities();

        public ActionResult BestSellingItems(int topN = 10)
        {
            var bestSelling = db.OrderDetails
                .GroupBy(od => od.Product.ProductName)
                .Select(g => new BestSellingItemViewModel
                {
                    ProductName = g.Key,
                    TotalQuantitySold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(topN)
                .ToList();
            ViewBag.TopN = topN;
            return View(bestSelling);
        }

        // Action
        public ActionResult ItemsByAgent(int? agentId)
        {
            ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName", agentId);
            List<ProductPurchaseByAgentViewModel> items = new List<ProductPurchaseByAgentViewModel>();
            if (agentId != null)
            {
                items = db.OrderDetails
                    .Where(od => od.Order.AgentId == agentId)
                    .GroupBy(od => od.Product.ProductName)
                    .Select(g => new ProductPurchaseByAgentViewModel
                    {
                        ProductName = g.Key,
                        TotalQuantityPurchased = g.Sum(od => od.Quantity)
                    })
                    .OrderBy(p => p.ProductName).ToList();
                ViewBag.SelectedAgentName = db.Agents.Find(agentId)?.AgentName;
            }
            return View(items);
        }
        // Action
        public ActionResult AgentsByItem(int? productId)
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", productId);
            List<AgentPurchaseByItemViewModel> agents = new List<AgentPurchaseByItemViewModel>();
            if (productId != null)
            {
                agents = db.OrderDetails
                    .Where(od => od.ProductId == productId)
                    .GroupBy(od => od.Order.Agent.AgentName)
                    .Select(g => new AgentPurchaseByItemViewModel
                    {
                        AgentName = g.Key,
                        TotalQuantityOfItemPurchased = g.Sum(od => od.Quantity)
                    })
                    .OrderBy(a => a.AgentName).ToList();
                ViewBag.SelectedProductName = db.Products.Find(productId)?.ProductName;
            }
            return View(agents);
        }
    }
}
