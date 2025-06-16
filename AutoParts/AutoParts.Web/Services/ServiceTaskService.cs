namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class ServiceTaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceTaskMapper _mapper;

    public ServiceTaskService(ApplicationDbContext context, ServiceTaskMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ServiceTaskModel>> GetAllAsync()
    {
        List<ServiceTask> entities = await _context.ServiceTasks
            .Include(task => task.UsedParts)
            .ThenInclude(usedPart => usedPart.Part)
            .ToListAsync();

        return entities.Select(entity => _mapper.ToViewModel(entity)).ToList();
    }

    public async Task<ServiceTaskModel?> GetAsync(int id)
    {
        ServiceTask? entity = await _context.ServiceTasks
            .Include(task => task.UsedParts)
            .ThenInclude(usedPart => usedPart.Part)
            .FirstOrDefaultAsync(task => task.Id == id);

        if (entity == null)
        {
            return null;
        }

        return _mapper.ToViewModel(entity);
    }

    public async Task<ServiceTaskModel> CreateAsync(ServiceTaskModel model)
    {
        ServiceTask entity = _mapper.ToEntity(model);

        _context.ServiceTasks.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.Id;

        return model;
    }

    public async Task<ServiceTaskModel?> UpdateAsync(ServiceTaskModel model)
    {
        ServiceTask? entity = await _context.ServiceTasks
            .Include(task => task.UsedParts)
            .ThenInclude(usedPart => usedPart.Part)
            .FirstOrDefaultAsync(task => task.Id == model.Id);

        if (entity == null)
        {
            return null;
        }

        _mapper.ToEntity(model, entity);
        _context.ServiceTasks.Update(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        ServiceTask? entity = await _context.ServiceTasks.FindAsync(id);

        if (entity == null)
        {
            return -1;
        }

        _context.ServiceTasks.Remove(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }
}
