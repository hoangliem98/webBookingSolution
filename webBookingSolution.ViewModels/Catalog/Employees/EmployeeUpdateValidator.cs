using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Employees
{
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateRequest>
    {
        public EmployeeUpdateValidator()
        {
            RuleFor(x => x.FirstName).MaximumLength(100).WithMessage("Họ tên không được nhập quá 200 ký tự");

            RuleFor(x => x.LastName).MaximumLength(100).WithMessage("Họ tên không được nhập quá 200 ký tự");

            RuleFor(x => x.DOB).GreaterThanOrEqualTo(DateTime.Now.AddYears(-100)).WithMessage("Ngày sinh không được vượt quá 100 năm");
            RuleFor(x => x.DOB).LessThanOrEqualTo(DateTime.Now.AddYears(-18)).WithMessage("Bạn chưa đủ tuổi");

            RuleFor(x => x.Email).Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
                .WithMessage("Email không hợp lệ");

            RuleFor(x => x.PhoneNumber).MaximumLength(10).MinimumLength(10).WithMessage("Số điện thoại không hợp lệ");
            RuleFor(x => x).Custom((request, context) =>
            {
                if (!checkPhone(request.PhoneNumber))
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
