using EVWarranty.Application.DTOs.Customer;
using EVWarranty.Application.Interfaces;
using EVWarranty.Domain.Entities;
using EVWarranty.Domain.Entities.Interfaces;

namespace EVWarranty.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        
        // Manual Mapping: Chuyển danh sách Entity sang danh sách DTO
        return customers.Select(c => new CustomerDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
            Idnumber = c.Idnumber
        });
    }

    public async Task<CustomerDto> RegisterCustomerAsync(CustomerCreateDto dto)
    {
        // Manual Mapping: Chuyển DTO sang Entity để lưu vào DB
        var customer = new Customer
        {
            FullName = dto.FullName,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            Idnumber = dto.Idnumber
        };

        await _unitOfWork.Repository<Customer>().AddAsync(customer);
        await _unitOfWork.CompleteAsync();

        // Trả về DTO sau khi đã có CustomerId từ DB
        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            Idnumber = customer.Idnumber
        };
    }
}