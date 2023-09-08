using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.N8.Models;
using Shop.Repository;
using System.Diagnostics;
using System.Security.Claims;

namespace Shop.N8.Areas.Customer.Controllers
{
    [Area("Customer")]   
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int? productId)
        {
            Cart cart = new Cart()
            {
                Product = _unitOfWork.Product.GetT(x => x.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = (int)productId,
            };

            return View(cart);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(Cart cart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claims.Value;

                var product = _unitOfWork.Product.GetT(x => x.Id == cart.ProductId);
                if (product == null)
                {
                    return NotFound();
                }
                TempData["ErrorMessage"] = null;
                if (cart.Count > product.Quatity)
                {
                    TempData["ErrorMessage"] = "The selected quantity is not available.";
                    return RedirectToAction("Details", new { productId = cart.ProductId });
                }

                var cartItem = _unitOfWork.Cart.GetT(x => x.ProductId == cart.ProductId
                    && x.ApplicationUserId == claims.Value);
                if (cartItem == null)
                {
                    cart.SelectedQuantity = cart.Count;
                    _unitOfWork.Cart.Add(cart);
                    _unitOfWork.Save();
                    /*                    HttpContext.Session.SetInt32("Session Cart", _unitOfWork
                                            .Cart.GetAll(x => x.ApplicationUserId == claims.Value).ToList().Count);*/
                }
                else
                {
                    int newCount = cartItem.Count + cart.Count;
                    if (newCount > product.Quatity)
                    {
                        TempData["ErrorMessage"] = "The selected quantity is not available.";
                        return RedirectToAction("Details", new { productId = cart.ProductId });
                    }
                    cart.SelectedQuantity = newCount;
                    _unitOfWork.Cart.IncrementCartItem(cartItem, cart.Count);
                    _unitOfWork.Save();

                }

                TempData.Remove("Count");
                int cartItemCount = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32("SessionCart", cartItemCount);
                /*                    cart.QuantitySelected -= cart.Count;
                                    _unitOfWork.Cart.Update(cart);
                                    _unitOfWork.Save();*/

            }
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}