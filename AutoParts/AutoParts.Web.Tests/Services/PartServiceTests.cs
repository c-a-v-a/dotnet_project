namespace AutoParts.Web.Tests.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

[TestFixture]
public class PartServiceTests
{
    private ApplicationDbContext _context = null!;
    private PartMapper _mapper = null!;
    private PartService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("PartTestDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _mapper = new PartMapper();
        _service = new PartService(_context, _mapper);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsMappedAndOrderedParts()
    {
        _context.Parts.AddRange(new[]
        {
            new Part { Name = "Filter", Type = "Engine", UnitPrice = 10.5m },
            new Part { Name = "Brake Pad", Type = "Brakes", UnitPrice = 25.0m }
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Type, Is.EqualTo("Brakes"));
        Assert.That(result[1].Type, Is.EqualTo("Engine"));
    }

    [Test]
    public async Task GetAsync_WhenExists_ReturnsMappedModel()
    {
        var part = new Part { Name = "Spark Plug", Type = "Engine", UnitPrice = 5.0m };
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(part.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Spark Plug"));
    }

    [Test]
    public async Task GetAsync_WhenNotFound_ReturnsNull()
    {
        var result = await _service.GetAsync(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_AddsEntityAndReturnsModel()
    {
        var model = new PartModel { Name = "Air Filter", Type = "Engine", UnitPrice = 12.99m };

        var result = await _service.CreateAsync(model);

        var entity = await _context.Parts.FindAsync(result.Id);

        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity!.Name, Is.EqualTo("Air Filter"));
    }

    [Test]
    public async Task UpdateAsync_WhenExists_UpdatesEntityAndReturnsModel()
    {
        var part = new Part { Name = "Old Name", Type = "Suspension", UnitPrice = 100m };
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();

        var updatedModel = new PartModel
        {
            Id = part.Id,
            Name = "New Name",
            Type = "Suspension",
            UnitPrice = 120m
        };

        var result = await _service.UpdateAsync(updatedModel);
        var updatedEntity = await _context.Parts.FindAsync(part.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(updatedEntity!.Name, Is.EqualTo("New Name"));
        Assert.That(updatedEntity.UnitPrice, Is.EqualTo(120m));
    }

    [Test]
    public async Task UpdateAsync_WhenNotFound_ReturnsNull()
    {
        var model = new PartModel
        {
            Id = 12345,
            Name = "Ghost",
            Type = "Unknown",
            UnitPrice = 1.0m
        };

        var result = await _service.UpdateAsync(model);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenExists_RemovesEntityAndReturnsId()
    {
        var part = new Part { Name = "DeleteMe", Type = "Misc", UnitPrice = 1.0m };
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(part.Id);
        var deleted = await _context.Parts.FindAsync(part.Id);

        Assert.That(result, Is.EqualTo(part.Id));
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenNotFound_ReturnsMinusOne()
    {
        var result = await _service.DeleteAsync(9999);

        Assert.That(result, Is.EqualTo(-1));
    }
}
