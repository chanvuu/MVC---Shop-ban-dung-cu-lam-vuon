﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.N8.Models;
using Shop.Repository;
using Shop.Utility;

namespace Shop.N8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.categories = _unitOfWork.Category.GetAll();
            return View(categoryVM);
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM vm = new CategoryVM();
            if (id == null || id == 0)
            {
                return View(vm);
            }
            vm.Category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (vm.Category == null)
            {
                return NotFound();
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult CreateUpdate(CategoryVM vm) 
        {
            if (!ModelState.IsValid)
            {
                if (vm.Category.Id == 0)
                {
                    _unitOfWork.Category.Add(vm.Category);
                    TempData["success"] = "Category Created Done";
                }
                else
                {
                    _unitOfWork.Category.Update(vm.Category);
                    TempData["success"] = "Category Updated Done";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }            
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Done";
            return RedirectToAction("Index");
        }
        
    }
}
