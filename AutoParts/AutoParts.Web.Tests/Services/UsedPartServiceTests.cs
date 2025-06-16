namespace AutoParts.Tests.Services;

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
public class UsedPartServiceTests
{
    private ApplicationDbContext _context = null!;
    private UsedPartService _service = null!;
    private UsedPartMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"UsedPartsDb_{System.Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _mapper = new UsedPartMapper();
        _service = new UsedPartService(_context, _mapper);

        SeedData();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedData()
    {
        var part = new Part
        {
            Id = 1,
            Name = "Oil Filter",
            Type = "Filter",
            UnitPrice = 15.99m
        };

        var serviceTask = new ServiceTask
        {
            Id = 1,
            Description = "Oil change"
        };

        _context.Parts.Add(part);
        _context.ServiceTasks.Add(serviceTask);
        _context.SaveChanges();
    }

    [Test]
    public async Task CreateAsync_ShouldAddUsedPart()
    {
        var model = new UsedPartModel
        {
            PartId = 1,
            ServiceTaskId = 1,
            Quantity = 2
        };

        var result = await _service.CreateAsync(model);

        var entity = await _context.UsedParts.Include(x => x.Part).FirstOrDefaultAsync();

        Assert.That(entity, Is.Not.Null);
        Assert.That(result.PartId, Is.EqualTo(1));
        Assert.That(result.Quantity, Is.EqualTo(2));
        Assert.That(result.TotalPrice, Is.EqualTo(31.98).Within(0.01));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllUsedParts()
    {
        _context.UsedParts.Add(new UsedPart { PartId = 1, ServiceTaskId = 1, Quantity = 1 });
        _context.SaveChanges();

        var result = await _service.GetAllAsync();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().Quantity, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAsync_ShouldReturnUsedPartById()
    {
        var usedPart = new UsedPart { PartId = 1, ServiceTaskId = 1, Quantity = 3 };
        _context.UsedParts.Add(usedPart);
        _context.SaveChanges();

        var result = await _service.GetAsync(usedPart.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Quantity, Is.EqualTo(3));
        Assert.That(result.TotalPrice, Is.EqualTo(47.97).Within(0.01));
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyExistingUsedPart()
    {
        var usedPart = new UsedPart { PartId = 1, ServiceTaskId = 1, Quantity = 1 };
        _context.UsedParts.Add(usedPart);
        _context.SaveChanges();

        var updatedModel = new UsedPartModel
        {
            Id = usedPart.Id,
            PartId = 1,
            ServiceTaskId = 1,
            Quantity = 4
        };

        var result = await _service.UpdateAsync(updatedModel);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Quantity, Is.EqualTo(4));
        Assert.That(result.TotalPrice, Is.EqualTo(63.96).Within(0.01));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveUsedPart()
    {
        var usedPart = new UsedPart { PartId = 1, ServiceTaskId = 1, Quantity = 1 };
        _context.UsedParts.Add(usedPart);
        _context.SaveChanges();

        var result = await _service.DeleteAsync(usedPart.Id);
        var entity = await _context.UsedParts.FindAsync(usedPart.Id);

        Assert.That(result, Is.EqualTo(usedPart.Id));
        Assert.That(entity, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnMinusOneIfNotFound()
    {
        var result = await _service.DeleteAsync(999);

        Assert.That(result, Is.EqualTo(-1));
    }
}

