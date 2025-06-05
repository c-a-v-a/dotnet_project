namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/customers/{customerId}/vehicles")]
public class CustomerVehiclesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomerVehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleModel>> AddVehicleToCustomer(int customerId, VehicleModel vehicleModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
        {
            return NotFound($"Customer with id {customerId} not found.");
        }

        var vehicle = new Vehicle
        {
            Make = vehicleModel.Make,
            Model = vehicleModel.Model,
            Year = vehicleModel.Year,
            Color = vehicleModel.Color,
            VIN = vehicleModel.VIN,
            LicensePlate = vehicleModel.LicensePlate,
            CustomerId = customerId
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        vehicleModel.Id = vehicle.Id;
        return CreatedAtAction(nameof(GetVehicle), new { customerId, id = vehicleModel.Id }, vehicleModel);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleModel>> GetVehicle(int customerId, int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && v.CustomerId == customerId);

        if (vehicle == null)
        {
            return NotFound();
        }

        return new VehicleModel
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            VIN = vehicle.VIN,
            LicensePlate = vehicle.LicensePlate,
            CustomerId = vehicle.CustomerId
        };
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleModel>>> GetCustomerVehicles(int customerId)
    {
        var vehicles = await _context.Vehicles
            .Where(v => v.CustomerId == customerId)
            .Select(v => new VehicleModel
            {
                Id = v.Id,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Color = v.Color,
                VIN = v.VIN,
                LicensePlate = v.LicensePlate,
                CustomerId = v.CustomerId
            })
            .ToListAsync();

        return vehicles;
    }
}
