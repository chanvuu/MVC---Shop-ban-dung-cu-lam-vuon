using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.N8.Models;
using Shop.Repository;
using Shop.Repository.Respositories;
using Shop.Utility.DbInitalizer;
using System.Security.Claims;

namespace Shop.N8.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CartVM vm { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            var product = _unitOfWork.Product.GetT(x => x.Id == cart.ProductId);
            TempData["ErrorMessage"] = null;
            if (cart.Count+1 > product.Quatity)
            {
                TempData["ErrorMessage"] = "The selected quantity is not available.";
                return RedirectToAction("Index");
            }
            _unitOfWork.Cart.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            if (cart.Count <= 1)
            {
                _unitOfWork.Cart.Delete(cart);
                int cartItemCount = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32("SessionCart", cartItemCount);
            }
            else
            {
                _unitOfWork.Cart.DecrementCartItem(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();
            int cartItemCount = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart", cartItemCount);
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claims.Value);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.Phone = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser.Address;
            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser.City;
            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.PostalCode = vm.OrderHeader.ApplicationUser.PinCode;

            foreach(var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            var cartItems = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product");

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(OrderHeader orderHeader)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cartItems = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product");

            if (cartItems == null || cartItems.Count() == 0)
            {
                ModelState.AddModelError("", "Không có sản phẩm trong giỏ hàng.");
            }

            if (!ModelState.IsValid)
            {
                orderHeader.ApplicationUserId = claims.Value;
                orderHeader.DateOfOrder = DateTime.Now;
                orderHeader.OrderTotal = 0;

                _unitOfWork.OrderHeader.Add(orderHeader);
                _unitOfWork.Save();

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderHeaderId = orderHeader.Id,
                        Price = item.Product.Price,
                        Count = item.Count
                    };

                    orderHeader.OrderTotal += (orderDetail.Price * orderDetail.Count);

                    var product = _unitOfWork.Product.GetT(x=> x.Id == item.ProductId);
                    product.Quatity -= item.Count;

                    _unitOfWork.Product.Update(product);
                    _unitOfWork.OrderDetail.Add(orderDetail);
                }
                
                foreach (var item in cartItems)
                {
                    _unitOfWork.Cart.Delete(item);
                }
                _unitOfWork.Save();                

                int cartItemCount = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product").ToList().Count;
                HttpContext.Session.SetInt32("SessionCart", cartItemCount);


                return RedirectToAction("Index", "Home");
            }

            vm = new CartVM()
            {
                ListOfCart = cartItems,
                OrderHeader = orderHeader
            };

            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claims.Value);

            return View(vm);
        }


    }
}
