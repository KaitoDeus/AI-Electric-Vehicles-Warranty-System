using System.ComponentModel.DataAnnotations;

namespace EVWarranty.Application.DTOs.Customer
{
    public class CustomerCreateDto
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Idnumber { get; set; }
    }
}