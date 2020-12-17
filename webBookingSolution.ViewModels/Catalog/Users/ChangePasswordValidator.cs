using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Users
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x).Custom((request, context) => {
                if (request.Password != request.ConfirmPassword)
                {
                    context.AddFailure("Mật khẩu không trùng nhau");
                }
            });

            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Mật khẩu không được nhập dưới 8 ký tự");
        }
    }
}
