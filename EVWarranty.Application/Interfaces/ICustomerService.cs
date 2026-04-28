using EVWarranty.Application.DTOs.Customer;

namespace EVWarranty.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto> RegisterCustomerAsync(CustomerCreateDto dto);
}