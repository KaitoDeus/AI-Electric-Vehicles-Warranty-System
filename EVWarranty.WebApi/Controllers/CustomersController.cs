using EVWarranty.Application.DTOs.Customer;
using EVWarranty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVWarranty.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,SC Staff,SC Technician")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var result = await _customerService.GetAllCustomersAsync();
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<CustomerDto>> Register(CustomerCreateDto dto)
    {
        var result = await _customerService.RegisterCustomerAsync(dto);
        return Ok(result);
    }
}