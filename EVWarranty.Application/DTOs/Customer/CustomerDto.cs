namespace EVWarranty.Application.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Idnumber { get; set; }
    }
}