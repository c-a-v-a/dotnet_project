namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class VehicleService
{
    private readonly ApplicationDbContext _context;
    private readonly VehicleMapper _mapper;

    public VehicleService(ApplicationDbContext context, VehicleMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<VehicleModel>> GetAllAsync()
    {
        List<Vehicle> entities = await _context.Vehicles.Include(vehicle => vehicle.Customer).ToListAsync();

        return entities.Select(entity => _mapper.ToViewModel(entity)).ToList();
    }

    public async Task<VehicleModel?> GetAsync(int id)
    {
        Vehicle? entity = await _context.Vehicles.Include(vehicle => vehicle.Customer).FirstOrDefaultAsync(vehicle => vehicle.Id == id);

        if (entity == null)
        {
            return null;
        }

        return _mapper.ToViewModel(entity);
    }

    public async Task<VehicleModel> CreateAsync(VehicleModel model)
    {
        Vehicle entity = _mapper.ToEntity(model);

        _context.Vehicles.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<VehicleModel?> UpdateAsync(VehicleModel model)
    {
        Vehicle? entity = await _context.Vehicles.FindAsync(model.Id);

        if (entity == null)
        {
            return null;
        }

        _mapper.ToEntity(model, entity);
        _context.Vehicles.Update(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        Vehicle? entity = await _context.Vehicles.FindAsync(id);

        if (entity == null)
        {
            return -1;
        }

        _context.Vehicles.Remove(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }
}
