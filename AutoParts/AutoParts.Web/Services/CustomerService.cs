namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerMapper _mapper;

    public CustomerService(ApplicationDbContext context, CustomerMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CustomerModel>> GetAllAsync()
    {
        List<Customer> entities = await _context.Customers.ToListAsync();

        return entities.Select(entity => _mapper.ToViewModel(entity)).ToList();
    }

    public async Task<CustomerModel?> GetAsync(int id)
    {
        Customer? entity = await _context.Customers
            .Include(customer => customer.Vehicles)
            .FirstOrDefaultAsync(customer => customer.Id == id);

        if (entity == null)
        {
            return null;
        }

        return _mapper.ToViewModel(entity);
    }

    public async Task<CustomerModel> CreateAsync(CustomerModel model)
    {
        Customer entity = _mapper.ToEntity(model);

        _context.Customers.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<CustomerModel?> UpdateAsync(CustomerModel model)
    {
        Customer? entity = await _context.Customers.FindAsync(model.Id);

        if (entity == null)
        {
            return null;
        }

        _mapper.ToEntity(model, entity);
        _context.Customers.Update(entity);
        await _context.SaveChangesAsync();

        return _mapper.ToViewModel(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        Customer? entity = await _context.Customers.FindAsync(id);

        if (entity == null)
        {
            return -1;
        }

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }
}
