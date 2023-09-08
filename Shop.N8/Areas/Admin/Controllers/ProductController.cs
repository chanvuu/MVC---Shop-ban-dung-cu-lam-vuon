using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Models;
using Shop.N8.Models;
using Shop.Repository;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Shop.N8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _hostingEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }
        #region APICALL
        public IActionResult AllProducts()
        {
            try
            {
                var products = _unitOfWork.Product.GetAll(includeProperties: "Category");
                return Json(new { data = products });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        #endregion
        public IActionResult Index()
        {
            ProductVM productVM = new ProductVM();
            productVM.Products = _unitOfWork.Product.GetAll();
            return View(productVM);
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM vm = new ProductVM()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                })

            };
            if (id == null || id == 0)
            {
                return View(vm);
            }
            vm.Product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (vm.Product == null)
            {
                return NotFound();
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult CreateUpdate(ProductVM vm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (file != null)
                {
                    string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImage");
                    fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    if (vm.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, vm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    vm.Product.ImageUrl = @"\ProductImage\" + fileName;
                }

                if (vm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(vm.Product);
                    TempData["success"] = "Product Created Done";
                }
                else
                {
                    _unitOfWork.Product.Update(vm.Product);
                    TempData["success"] = "Product Updated Done";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        #region DeleteAPICall
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error in Fetching Data" }); ;
            }
            var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();
            Console.WriteLine($"Deleted product with ID {id}");
            return Json(new { success = true, message = "Product Deleted" });
        }
        #endregion
    }
}