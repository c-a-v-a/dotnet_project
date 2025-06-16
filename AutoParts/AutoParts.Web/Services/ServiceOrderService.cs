namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class ServiceOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceOrderMapper _mapper;

    public ServiceOrderService(ApplicationDbContext context, ServiceOrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ServiceOrderModel>> GetAllAsync()
    {
        List<ServiceOrder> entities = await _context.ServiceOrders
            .Include(order => order.Vehicle)
            .ThenInclude(vehicle => vehicle.Customer)
            .Include(order => order.Mechanic)
            .Include(order => order.Tasks)
            .ThenInclude(task => task.UsedParts)
            .ThenInclude(usedPart => usedPart.Part)
            .Include(order => order.Comments)
            .ThenInclude(comment => comment.Author)
            .ToListAsync();

        return entities.Select(entity =>
            {
                ServiceOrderModel model = _mapper.ToViewModel(entity);
                model.CustomerId = entity.Vehicle.CustomerId;
                model.Customer = _mapper.ToShortDto(entity.Vehicle.Customer!);

                return model;
            }).ToList();
    }

    public async Task<ServiceOrderModel?> GetAsync(int id)
    {
        ServiceOrder? entity = await _context.ServiceOrders
            .Include(order => order.Vehicle)
            .ThenInclude(vehicle => vehicle.Customer)
            .Include(order => order.Mechanic)
            .Include(order => order.Tasks)
            .ThenInclude(task => task.UsedParts)
            .ThenInclude(usedPart => usedPart.Part)
            .Include(order => order.Comments)
            .ThenInclude(comment => comment.Author)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (entity == null)
        {
            return null;
        }

        ServiceOrderModel model = _mapper.ToViewModel(entity);
        model.CustomerId = entity.Vehicle.CustomerId;
        model.Customer = _mapper.ToShortDto(entity.Vehicle.Customer!);

        return model;
    }

    public async Task<ServiceOrderModel> CreateAsync(ServiceOrderModel model)
    {
        ServiceOrder entity = _mapper.ToEntity(model);

        _context.ServiceOrders.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.Id;

        return model;
    }

    public async Task<ServiceOrderModel?> UpdateAsync(ServiceOrderModel model)
    {
        ServiceOrder? entity = await _context.ServiceOrders
            .Include(order => order.Vehicle)
            .ThenInclude(vehicle => vehicle.Customer)
            .Include(order => order.Mechanic)
            .Include(order => order.Tasks)
            .ThenInclude(task => task.UsedParts)
            .ThenInclude(usedPart => usedPart.Part)
            .Include(order => order.Comments)
            .ThenInclude(comment => comment.Author)
            .FirstOrDefaultAsync(order => order.Id == model.Id);

        if (entity == null)
        {
            return null;
        }

        _mapper.ToEntity(model, entity);
        _context.ServiceOrders.Update(entity);
        await _context.SaveChangesAsync();

        ServiceOrderModel updated = _mapper.ToViewModel(entity);
        updated.CustomerId = entity.Vehicle.CustomerId;
        updated.Customer = _mapper.ToShortDto(entity.Vehicle.Customer!);

        return updated;
    }

    public async Task<int> DeleteAsync(int id)
    {
        ServiceOrder? entity = await _context.ServiceOrders.FindAsync(id);

        if (entity == null)
        {
            return -1;
        }

        _context.ServiceOrders.Remove(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }
}
