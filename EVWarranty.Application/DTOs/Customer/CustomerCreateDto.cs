namespace EVWarranty.Application.DTOs.Customer
{
    public class CustomerCreateDto
    {
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Idnumber { get; set; }
    }
}