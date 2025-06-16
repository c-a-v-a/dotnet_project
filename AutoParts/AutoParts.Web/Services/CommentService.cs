namespace AutoParts.Web.Services;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.EntityFrameworkCore;

public class CommentService
{
    private readonly ApplicationDbContext _context;
    private readonly CommentMapper _mapper;

    public CommentService(ApplicationDbContext context, CommentMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CommentModel?> CreateAsync(CommentModel model)
    {
        ServiceOrder? order = await _context.ServiceOrders.FindAsync(model.ServiceOrderId);

        if (order == null)
        {
            return null;
        }

        model.CreatedAt = DateTime.Now;

        Comment entity = _mapper.ToEntity(model);

        _context.Comments.Add(entity);
        await _context.SaveChangesAsync();

        model.AuthorId = entity.AuthorId;
        model.Author = _mapper.ToShortDto(entity.Author);
        model.Id = entity.Id;

        return model;
    }
}
