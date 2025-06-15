namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class PartService
{
    private readonly ApplicationDbContext _context;
    private readonly PartMapper _mapper;

    public PartService(ApplicationDbContext context, PartMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PartModel>> GetAllAsync()
    {
        List<Part> entities = await _context.Parts.ToListAsync();

        return entities.Select(entity => _mapper.ToViewModel(entity))
            .OrderBy(part => part.Type)
            .ToList();
    }

    public async Task<PartModel?> GetAsync(int id)
    {
        Part? entity = await _context.Parts.FindAsync(id);

        if (entity == null)
        {
            return null;
        }

        return _mapper.ToViewModel(entity);
    }

    public async Task<PartModel> CreateAsync(PartModel model)
    {
        Part entity = _mapper.ToEntity(model);

        _context.Parts.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<PartModel?> UpdateAsync(PartModel model)
    {
        Part? entity = await _context.Parts.FindAsync(model.Id);

        if (entity == null)
        {
            return null;
        }

        _mapper.ToEntity(model, entity);
        _context.Parts.Update(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        Part? entity = await _context.Parts.FindAsync(id);

        if (entity == null)
        {
            return -1;
        }

        _context.Parts.Remove(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }
}
