using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Books
{
    public class BookCreateValidator : AbstractValidator<BookCreateRequest>
    {
        public BookCreateValidator()
        {

            RuleFor(x => x.HallId).NotEqual(0).WithMessage("Vui lòng chọn 1 sảnh");
            RuleFor(x => x.MenuId).NotEqual(0).WithMessage("Vui lòng chọn 1 thực đơn");
            RuleFor(x => x.NumberTables).GreaterThanOrEqualTo(100).LessThanOrEqualTo(1000).WithMessage("Số bàn không phù hợp");
            RuleFor(x => x.OrganizationDate).NotEmpty().WithMessage("Vui lòng chọn ngày tổ chức");
            RuleFor(x => x.OrganizationDate).LessThanOrEqualTo(DateTime.Now.AddDays(30)).WithMessage("Ngày tổ chức chỉ được đặt trước trong vòng 1 tháng");
            RuleFor(x => x.CustomerCreateRequest.FirstName).MaximumLength(100).WithMessage("Họ tên không được nhập quá 200 ký tự");

            RuleFor(x => x.CustomerCreateRequest.LastName).MaximumLength(100).WithMessage("Họ tên không được nhập quá 200 ký tự");

            RuleFor(x => x.CustomerCreateRequest.DOB).GreaterThanOrEqualTo(DateTime.Now.AddYears(-100)).WithMessage("Ngày sinh không được vượt quá 100 năm");
            RuleFor(x => x.CustomerCreateRequest.DOB).LessThanOrEqualTo(DateTime.Now.AddYears(-18)).WithMessage("Bạn chưa đủ tuổi");

            RuleFor(x => x.CustomerCreateRequest.Email).Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
                .WithMessage("Email không hợp lệ");

            RuleFor(x => x.CustomerCreateRequest.PhoneNumber).MaximumLength(10).WithMessage("Số điện thoại không hợp lệ").MinimumLength(10).WithMessage("Số điện thoại không hợp lệ");
            RuleFor(x => x).Custom((request, context) =>
            {
                if (!checkPhone(request.CustomerCreateRequest.PhoneNumber))
                {
                    context.AddFailure("Số điện thoại không hợp lệ");
                }
            });
        }

        public static bool checkPhone(string phone)
        {
            int number = 0;
            return int.TryParse(phone, out number);
        }
    }
}
