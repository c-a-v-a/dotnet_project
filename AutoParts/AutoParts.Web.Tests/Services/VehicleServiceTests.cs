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

namespace AutoParts.Web.Tests.Services
{
    [TestFixture]
    public class VehicleServiceTests
    {
        private ApplicationDbContext _context = null!;
        private VehicleMapper _mapper = null!;
        private VehicleService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("VehicleTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _mapper = new VehicleMapper();
            _service = new VehicleService(_context, _mapper);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed one customer for tests
            _context.Customers.Add(new Customer
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                PhoneNumber = "1234567890"
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedVehicles()
        {
            _context.Vehicles.AddRange(new[]
            {
                new Vehicle { Make = "Toyota", ModelName = "Corolla", Year = 2010, CustomerId = 1 },
                new Vehicle { Make = "Honda", ModelName = "Civic", Year = 2012, CustomerId = 1 }
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(v => v.Make == "Toyota"), Is.True);
        }

        [Test]
        public async Task GetAsync_WhenExists_ReturnsMappedModel()
        {
            var vehicle = new Vehicle
            {
                Make = "Mazda",
                ModelName = "3",
                Year = 2020,
                CustomerId = 1
            };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var result = await _service.GetAsync(vehicle.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Make, Is.EqualTo("Mazda"));
            Assert.That(result.Customer, Is.Not.Null);
            Assert.That(result.Customer!.FirstName, Is.EqualTo("Jane"));
        }

        [Test]
        public async Task GetAsync_WhenNotFound_ReturnsNull()
        {
            var result = await _service.GetAsync(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsVehicleAndReturnsMappedModel()
        {
            var model = new VehicleModel
            {
                Make = "Subaru",
                ModelName = "Forester",
                Year = 2018,
                CustomerId = 1
            };

            var result = await _service.CreateAsync(model);
            var entity = await _context.Vehicles.FindAsync(result.Id);

            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity!.Make, Is.EqualTo("Subaru"));
        }

        [Test]
        public async Task UpdateAsync_WhenExists_UpdatesVehicle()
        {
            var vehicle = new Vehicle
            {
                Make = "Nissan",
                ModelName = "Sentra",
                Year = 2015,
                CustomerId = 1,
            };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var updated = new VehicleModel
            {
                Id = vehicle.Id,
                Make = "Nissan",
                ModelName = "Altima",
                Year = 2015,
                CustomerId = 1
            };

            var result = await _service.UpdateAsync(updated);
            var entity = await _context.Vehicles.FindAsync(vehicle.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(entity!.ModelName, Is.EqualTo("Altima"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotFound_ReturnsNull()
        {
            var model = new VehicleModel
            {
                Id = 999,
                Make = "Ghost",
                ModelName = "Phantom",
                Year = 2000,
                CustomerId = 1
            };

            var result = await _service.UpdateAsync(model);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_RemovesVehicle()
        {
            var vehicle = new Vehicle
            {
                Make = "Kia",
                ModelName = "Soul",
                Year = 2017,
                CustomerId = 1
            };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(vehicle.Id);
            var deleted = await _context.Vehicles.FindAsync(vehicle.Id);

            Assert.That(result, Is.EqualTo(vehicle.Id));
            Assert.That(deleted, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenNotFound_ReturnsMinusOne()
        {
            var result = await _service.DeleteAsync(12345);

            Assert.That(result, Is.EqualTo(-1));
        }
    }
}

