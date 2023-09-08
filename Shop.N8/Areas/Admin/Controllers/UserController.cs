using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.N8.Models;
using Shop.Repository;
using Shop.Utility;

namespace Shop.N8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region APICALL
        public IActionResult AllUsers()
        {
            try
            {
                var users = _unitOfWork.ApplicationUser.GetAll();
                return Json(new { data = users });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        #endregion
        public IActionResult Index()
        {
            UserVM userVM = new UserVM();
            userVM.applicationUsers = _unitOfWork.ApplicationUser.GetAll();
            return View(userVM);
        }
        [HttpGet]
        public IActionResult CreateUpdate(string? id)
        {
            UserVM vm = new UserVM();
            if (id == null || id == 0.ToString())
            {
                return View(vm);
            }
            vm.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == id);
            if (vm.ApplicationUser == null)
            {
                return NotFound();
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult CreateUpdate(UserVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ApplicationUser.Id == 0.ToString())
                {
                    _unitOfWork.ApplicationUser.Add(vm.ApplicationUser);
                    TempData["success"] = "User Created Done";
                }
                else
                {
                    _unitOfWork.ApplicationUser.Update(vm.ApplicationUser);
                    TempData["success"] = "User Updated Done";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        #region DeleteAPICall
        [HttpDelete]
        public IActionResult Delete(string? id)
        {
            var user = _unitOfWork.ApplicationUser.GetT(x => x.Id == id);            
            if (user == null)
            {
                return Json(new { success = false, message = "Error in Fetching Data" }); ;
            }

            _unitOfWork.ApplicationUser.Delete(user);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product Deleted" });
        }
        #endregion

    }
}
