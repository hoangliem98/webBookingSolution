using FluentValidation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace webBookingSolution.ViewModels.System
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName).MaximumLength(100).WithMessage("Họ tên không được nhập quá 200 ký tự");

            RuleFor(x => x.LastName).MaximumLength(100).WithMessage("Họ tên không được nhập quá 200 ký tự");

            RuleFor(x => x.DOB).GreaterThanOrEqualTo(DateTime.Now.AddYears(-100)).WithMessage("Ngày sinh không được vượt quá 100 năm");
            RuleFor(x => x.DOB).LessThanOrEqualTo(DateTime.Now.AddYears(-18)).WithMessage("Bạn chưa đủ tuổi");

            RuleFor(x => x.Email).Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
                .WithMessage("Email không hợp lệ");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.Password != request.ConfirmPassword)
                {
                    context.AddFailure("Mật khẩu không trùng nhau");
                }
                if (!checkPhone(request.PhoneNumber))
                {
                    context.AddFailure("Số điện thoại không hợp lệ");
                }
                if (request.UserName.Contains(" "))
                {
                    context.AddFailure("Tên tài khoản không hợp lệ");
                }
            });

            RuleFor(x => x.PhoneNumber).MaximumLength(10).WithMessage("Số điện thoại không hợp lệ").MinimumLength(10).WithMessage("Số điện thoại không hợp lệ");

            RuleFor(x => x.UserName).Matches(@"^[[A-Z]|[a-z]][[A-Z]|[a-z]|\\d|[_]]{7,29}$").WithMessage("Tên tài khoản không hợp lệ");

            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Mật khẩu không được nhập dưới 8 ký tự");

            RuleFor(x => x.Image).NotNull().WithMessage("Vui lòng chọn ảnh đại diện");
        }

        public static bool checkPhone(string phone)
        {
            int number = 0;
            return int.TryParse(phone, out number);
        }
    }
}
