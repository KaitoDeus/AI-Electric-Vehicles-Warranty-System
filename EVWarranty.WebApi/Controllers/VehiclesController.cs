using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EVWarranty.Domain.Entities.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize] // <--- Khóa Controller này lại, chỉ ai có Token mới vào được
public class VehiclesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    public VehiclesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
        return Ok(vehicles);
    }
}
