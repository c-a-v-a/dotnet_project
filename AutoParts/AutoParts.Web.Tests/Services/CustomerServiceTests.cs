using System.Collections.Generic;
using System.Threading.Tasks;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AutoParts.Web.Tests.Services
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private ApplicationDbContext _context = null!;
        private CustomerMapper _mapper = null!;
        private CustomerService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CustomerTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _mapper = new CustomerMapper();
            _service = new CustomerService(_context, _mapper);

            // Clear database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllCustomers()
        {
            var customer1 = new Customer { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" };
            var customer2 = new Customer { FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" };
            _context.Customers.AddRange(customer1, customer2);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].FirstName, Is.EqualTo("Alice"));
            Assert.That(result[1].FirstName, Is.EqualTo("Bob"));
        }

        [Test]
        public async Task GetAsync_ExistingId_ReturnsCustomer()
        {
            var customer = new Customer { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var result = await _service.GetAsync(customer.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FirstName, Is.EqualTo("Alice"));
        }

        [Test]
        public async Task GetAsync_NonExistingId_ReturnsNull()
        {
            var result = await _service.GetAsync(9999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsEntityAndReturnsMappedModel()
        {
            var model = new CustomerModel
            {
                FirstName = "Charlie",
                LastName = "Brown",
                Email = "charlie@example.com",
                PhoneNumber = "123456789"
            };

            var result = await _service.CreateAsync(model);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.FirstName, Is.EqualTo("Charlie"));

            var customerInDb = await _context.Customers.FindAsync(result.Id);
            Assert.That(customerInDb, Is.Not.Null);
            Assert.That(customerInDb!.FirstName, Is.EqualTo("Charlie"));
        }

        [Test]
        public async Task UpdateAsync_ExistingCustomer_UpdatesAndReturnsModel()
        {
            var customer = new Customer { FirstName = "Daisy", LastName = "Miller", Email = "daisy@example.com" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var updateModel = new CustomerModel
            {
                Id = customer.Id,
                FirstName = "DaisyUpdated",
                LastName = "MillerUpdated",
                Email = "daisyupdated@example.com",
                PhoneNumber = "987654321"
            };

            var result = await _service.UpdateAsync(updateModel);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FirstName, Is.EqualTo("DaisyUpdated"));

            var customerInDb = await _context.Customers.FindAsync(customer.Id);
            Assert.That(customerInDb, Is.Not.Null);
            Assert.That(customerInDb!.FirstName, Is.EqualTo("DaisyUpdated"));
        }

        [Test]
        public async Task UpdateAsync_NonExistingCustomer_ReturnsNull()
        {
            var updateModel = new CustomerModel
            {
                Id = 9999,
                FirstName = "NonExisting",
                LastName = "User",
                Email = "nonexisting@example.com"
            };

            var result = await _service.UpdateAsync(updateModel);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_ExistingCustomer_RemovesEntityAndReturnsId()
        {
            var customer = new Customer { FirstName = "Eve", LastName = "Adams", Email = "eve@example.com" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var deletedId = await _service.DeleteAsync(customer.Id);

            Assert.That(deletedId, Is.EqualTo(customer.Id));

            var customerInDb = await _context.Customers.FindAsync(customer.Id);
            Assert.That(customerInDb, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_NonExistingCustomer_ReturnsMinusOne()
        {
            var deletedId = await _service.DeleteAsync(9999);
            Assert.That(deletedId, Is.EqualTo(-1));
        }
    }
}

