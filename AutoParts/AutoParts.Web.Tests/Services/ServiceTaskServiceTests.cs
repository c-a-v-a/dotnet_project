namespace AutoParts.Tests.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

[TestFixture]
public class ServiceTaskServiceTests
{
    private ApplicationDbContext _context = null!;
    private ServiceTaskService _service = null!;
    private ServiceTaskMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"ServiceTaskDb_{System.Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _mapper = new ServiceTaskMapper();
        _service = new ServiceTaskService(_context, _mapper);

        SeedData().GetAwaiter().GetResult();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private async Task SeedData()
    {
        var serviceOrder = new ServiceOrder
        {
            Id = 1,
            Description = "Order 1",
            Status = OrderStatus.New,
            VehicleId = 1
        };

        var part = new Part
        {
            Id = 1,
            Name = "Brake Pad",
            Type = "Brake",
            UnitPrice = 50.00m
        };

        var usedPart = new UsedPart
        {
            Id = 1,
            PartId = part.Id,
            Part = part,
            Quantity = 2
        };

        var serviceTask = new ServiceTask
        {
            Id = 1,
            Name = "Brake Replacement",
            Description = "Replace brake pads",
            LaborCost = 100.00m,
            ServiceOrderId = serviceOrder.Id,
            ServiceOrder = serviceOrder,
            UsedParts = new List<UsedPart> { usedPart }
        };

        serviceOrder.Tasks.Add(serviceTask);

        await _context.ServiceOrders.AddAsync(serviceOrder);
        await _context.Parts.AddAsync(part);
        await _context.UsedParts.AddAsync(usedPart);
        await _context.ServiceTasks.AddAsync(serviceTask);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllServiceTasks()
    {
        var result = await _service.GetAllAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Brake Replacement"));
        Assert.That(result[0].UsedParts.Count, Is.EqualTo(1));
        Assert.That(result[0].TotalCost, Is.EqualTo(200m).Within(0.01m)); // 2 * 50 + 100 labor
    }

    [Test]
    public async Task GetAsync_ShouldReturnServiceTaskById()
    {
        var result = await _service.GetAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Brake Replacement"));
        Assert.That(result.UsedParts.Count, Is.EqualTo(1));
        Assert.That(result.TotalCost, Is.EqualTo(200m).Within(0.01m));
    }

    [Test]
    public async Task GetAsync_ShouldReturnNullIfNotFound()
    {
        var result = await _service.GetAsync(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_ShouldAddServiceTask()
    {
        var model = new ServiceTaskModel
        {
            Name = "Engine Tune-Up",
            Description = "Tune engine components",
            LaborCost = 150m,
            ServiceOrderId = 1,
            UsedParts = new List<UsedPartModel>()
        };

        var created = await _service.CreateAsync(model);

        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.Name, Is.EqualTo("Engine Tune-Up"));

        var entity = await _context.ServiceTasks.FindAsync(created.Id);
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity!.Name, Is.EqualTo("Engine Tune-Up"));
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyExistingServiceTask()
    {
        var model = await _service.GetAsync(1);
        Assert.That(model, Is.Not.Null);

        model!.Name = "Updated Brake Replacement";
        model.LaborCost = 120m;

        var updated = await _service.UpdateAsync(model);

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Name, Is.EqualTo("Updated Brake Replacement"));
        Assert.That(updated.LaborCost, Is.EqualTo(120m));
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnNullIfNotFound()
    {
        var model = new ServiceTaskModel
        {
            Id = 999,
            Name = "Non-existing",
            Description = "Does not exist",
            LaborCost = 10m,
            ServiceOrderId = 1
        };

        var updated = await _service.UpdateAsync(model);

        Assert.That(updated, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveServiceTask()
    {
        var idToDelete = 1;

        var result = await _service.DeleteAsync(idToDelete);

        Assert.That(result, Is.EqualTo(idToDelete));

        var entity = await _context.ServiceTasks.FindAsync(idToDelete);
        Assert.That(entity, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnMinusOneIfNotFound()
    {
        var result = await _service.DeleteAsync(999);

        Assert.That(result, Is.EqualTo(-1));
    }
}

