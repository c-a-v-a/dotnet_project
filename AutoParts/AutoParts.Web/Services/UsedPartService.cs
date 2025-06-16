namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class UsedPartService
{
    private readonly ApplicationDbContext _context;
    private readonly UsedPartMapper _mapper;

    public UsedPartService(ApplicationDbContext context, UsedPartMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UsedPartModel>> GetAllAsync()
    {
        List<UsedPart> entities = await _context.UsedParts.Include(usedPart => usedPart.Part).ToListAsync();

        return entities.Select(entity => _mapper.ToViewModel(entity)).ToList();
    }

    public async Task<UsedPartModel?> GetAsync(int id)
    {
        UsedPart? entity = await _context.UsedParts.Include(usedPart => usedPart.Part).FirstOrDefaultAsync(usedPart => usedPart.Id == id);

        if (entity == null)
        {
            return null;
        }

        return _mapper.ToViewModel(entity);
    }

    public async Task<UsedPartModel> CreateAsync(UsedPartModel model)
    {
        UsedPart entity = _mapper.ToEntity(model);

        _context.UsedParts.Add(entity);
        await _context.SaveChangesAsync();

        await _context.Entry(entity).Reference(usedPart => usedPart.Part).LoadAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<UsedPartModel?> UpdateAsync(UsedPartModel model)
    {
        UsedPart? entity = await _context.UsedParts.Include(usedPart => usedPart.Part).FirstOrDefaultAsync(usedPart => usedPart.Id == model.Id);

        if (entity == null)
        {
            return null;
        }

        _mapper.ToEntity(model, entity);
        _context.UsedParts.Update(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        UsedPart? entity = await _context.UsedParts.FindAsync(id);

        if (entity == null)
        {
            return -1;
        }

        _context.UsedParts.Remove(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }
}
