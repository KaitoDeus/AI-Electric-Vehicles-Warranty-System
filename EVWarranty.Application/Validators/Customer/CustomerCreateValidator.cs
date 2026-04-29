using FluentValidation;
using EVWarranty.Application.DTOs.Customer;

namespace EVWarranty.Application.Validators.Customer
{
    public class CustomerCreateValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Tên khách hàng không được để trống")
                .MaximumLength(100).WithMessage("Tên không được quá 100 ký tự");

            RuleFor(x => x.Email)
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Email không đúng định dạng");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^\d{10,11}$").WithMessage("Số điện thoại phải từ 10-11 chữ số");

            RuleFor(x => x.Idnumber)
                .NotEmpty().WithMessage("Số CCCD không được để trống")
                .Matches(@"^\d{12}$").WithMessage("Số CCCD phải bao gồm đúng 12 chữ số (không chứa chữ cái)");

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("Địa chỉ không được quá 255 ký tự");
        }
    }
}
