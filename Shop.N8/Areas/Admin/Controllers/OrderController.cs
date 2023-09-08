using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Shop.Models;
using Shop.N8.Models;
using Shop.Repository;
using Shop.Utility;
using Shop.Utility.DbInitalizer;
using System.Security.Claims;

namespace Shop.N8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region APICALL
        public IActionResult AllOrders(string status)
        {
            IEnumerable<OrderHeader> orderHeader;
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                orderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeader = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claims.Value);

            }
            foreach (var item in orderHeader)
            {
                if (item.OrderStatus == null)
                {
                    item.OrderStatus = OrderStatus.StatusPending;
                    _unitOfWork.OrderHeader.Update(item);
                }
            }
            _unitOfWork.Save();
            switch (status)
            {
                case "Pending":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusPending);
                    break;
                case "Cancelled":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusCancelled);
                    break;
                case "Approved":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeader });
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };
            return View(orderVM);
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        [HttpPost]
        public IActionResult OrderDetails(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id);
            orderHeader.Name = vm.OrderHeader.Name;
            orderHeader.Phone = vm.OrderHeader.Phone;
            orderHeader.Address = vm.OrderHeader.Address;
            orderHeader.City = vm.OrderHeader.City;
            orderHeader.State = vm.OrderHeader.State;
            orderHeader.PostalCode = vm.OrderHeader.PostalCode;
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        [HttpPost]
        public IActionResult Approved(OrderVM vm)
        {
            _unitOfWork.OrderHeader.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusApproved);
            _unitOfWork.Save();
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        [HttpPost]
        public IActionResult Cancelled(OrderVM vm)
        {
            _unitOfWork.OrderHeader.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusCancelled);
            _unitOfWork.Save();
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        [HttpPost]
        public IActionResult ExportToExcel(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(u => u.Id == id, includeProperties: "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id, includeProperties: "Product");

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Order");

                // Add customer information
                worksheet.Cells["A1"].Value = "Customer Name:";
                worksheet.Cells["B1"].Value = orderHeader.ApplicationUser.Name;

                worksheet.Cells["A2"].Value = "Phone:";
                worksheet.Cells["B2"].Value = orderHeader.ApplicationUser.PhoneNumber;

                worksheet.Cells["A3"].Value = "Address:";
                worksheet.Cells["B3"].Value = orderHeader.ApplicationUser.Address;

                // Add order date
                worksheet.Cells["E1"].Value = "Order Date:";
                worksheet.Cells["F1"].Value = orderHeader.DateOfOrder.ToString("dd/MM/yyyy");

                // Add table headers
                worksheet.Cells["A5"].Value = "Product Name";
                worksheet.Cells["B5"].Value = "Quantity";
                worksheet.Cells["C5"].Value = "Price";

                var row = 6;
                foreach (var item in orderDetails)
                {
                    worksheet.Cells[string.Format("A{0}", row)].Value = item.Product.ProductName;
                    worksheet.Cells[string.Format("B{0}", row)].Value = item.Count;
                    worksheet.Cells[string.Format("C{0}", row)].Value = item.Price;
                    row++;
                }

                // Add total price                
                worksheet.Cells[string.Format("A{0}", row)].Value = "Total Price:";
                worksheet.Cells[string.Format("C{0}", row)].Value = orderHeader.OrderTotal;
                row++;

                // Apply formatting to the cells
                var headerRange = worksheet.Cells["A5:C5"];
                headerRange.Style.Font.Bold = true;

                var customerNameCell = worksheet.Cells["B1"];
                customerNameCell.Style.Font.UnderLine = true;
                customerNameCell.Hyperlink = new ExcelHyperLink("tel:" + orderHeader.ApplicationUser.PhoneNumber);

                var totalPriceRange = worksheet.Cells[string.Format("A{0}:C{0}", row)];
                totalPriceRange.Style.Font.Bold = true;

                var dateRange = worksheet.Cells["E1:F1"];
                dateRange.Style.Font.Bold = true;

                var range = worksheet.Cells[string.Format("A5:C{0}", row - 1)];
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;


                worksheet.Cells.AutoFitColumns();
                package.Save();
            }

            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Order_{orderHeader.Id}.xlsx";

            return File(stream, contentType, fileName);
        }
        #region DeleteAPICall
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product");
            
            if (orderHeader == null || orderDetails == null)
            {
                return Json(new { success = false, message = "Error in Fetching Data" }); ;
            }
            _unitOfWork.OrderHeader.Delete(orderHeader);
            foreach (var details in orderDetails)
            {
                if (details.OrderHeaderId == id)
                {
                    if (orderHeader.OrderStatus == "Cancelled" || orderHeader.OrderStatus == "Pending")
                    {
                        // Cập nhật giá trị quantity của sản phẩm
                        var product = _unitOfWork.Product.GetT(x => x.Id == details.ProductId);
                        if (product != null)
                        {
                            product.Quatity += details.Count;
                            _unitOfWork.Product.Update(product);
                        }
                    }
                    _unitOfWork.OrderDetail.Delete(details);
                }
            }
            _unitOfWork.Save();
            Console.WriteLine($"Deleted product with ID {id}");
            return Json(new { success = true, message = "Product Deleted" });
        }
        #endregion

    }
}
