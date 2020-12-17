using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Api.Books;
using webBookingSolution.Api.Payments;
using webBookingSolution.ViewModels.Catalog.Books;
using webBookingSolution.ViewModels.Catalog.Payments;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IBookApiClient _bookApiClient;
        private readonly IPaymentApiClient _paymentApiClient;

        public PaymentController(IBookApiClient bookApiClient, IPaymentApiClient paymentApiClient)
        {
            _paymentApiClient = paymentApiClient;
            _bookApiClient = bookApiClient;
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookBigViewModel request)
        {
            var book = await _bookApiClient.GetById(request.bookViewModel.Id);

            var paymentCreate = new PaymentCreateRequest()
            {
                BookId = request.bookViewModel.Id,
                TempPrice = book.Price,
                EmployeeId = int.Parse(User.FindFirst("UserId").Value)
            };
            var result = await _paymentApiClient.Create(paymentCreate);
            if (result == "Lỗi")
            {
                ModelState.AddModelError("", "Thanh toán thất bại");
                return RedirectToAction("List", "Book");
            }
            else if (result == "NotFound")
            {
                ModelState.AddModelError("", "Không tìm thấy đơn hàng");
                return RedirectToAction("List", "Book");
            }
            else
            {
                TempData["result"] = "Thanh toán thành công";
                return RedirectToAction("List", "Payment");
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var data = await _paymentApiClient.GetAll();
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(data);
        }

        public async Task<IActionResult> ExportExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Payments");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Mã đơn hàng";
                worksheet.Cell(currentRow, 2).Value = "Ngày tổ chức";
                worksheet.Cell(currentRow, 3).Value = "Ngày thanh toán";
                worksheet.Cell(currentRow, 4).Value = "Nhân viên thanh toán";
                worksheet.Cell(currentRow, 5).Value = "Khách hàng";
                worksheet.Cell(currentRow, 6).Value = "Tạm tính";
                worksheet.Cell(currentRow, 7).Value = "Phụ phí";
                worksheet.Cell(currentRow, 8).Value = "Tổng tiền";

                var payments = await _paymentApiClient.GetAll();
                foreach (var item in payments)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.BookId;
                    worksheet.Cell(currentRow, 2).Value = item.OrganizationDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 3).Value = item.PaymentDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 4).Value = item.EmployeeName;
                    worksheet.Cell(currentRow, 5).Value = item.CustomerName;
                    worksheet.Cell(currentRow, 6).Value = item.TempPrice;
                    worksheet.Cell(currentRow, 7).Value = item.DelayPrice;
                    worksheet.Cell(currentRow, 8).Value = item.TotalPrice;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payments.xlsx");
                }
            }
        }
    }
}
