namespace AutoParts.Web.Tests.Services;

using System;
using System.Threading.Tasks;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.DTOs;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

[TestFixture]
public class CommentServiceTests
{
    private ApplicationDbContext _context;
    private CommentMapper _mapper;
    private CommentService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _context = new ApplicationDbContext(options);

        // Seed a ServiceOrder for tests
        _context.ServiceOrders.Add(new ServiceOrder { Id = 1 });
        _context.SaveChanges();

        _context.Users.Add(new User
        {
            Id = "author1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        });
        _context.SaveChanges();

        _mapper = new CommentMapper(); // Real mapper instance

        _service = new CommentService(_context, _mapper);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateAsync_WithValidServiceOrder_CreatesCommentAndReturnsModel()
    {
        // Arrange
        var commentModel = new CommentModel
        {
            Text = "Test comment",
            ServiceOrderId = 1,
            AuthorId = "author1"
        };

        // Act
        var result = await _service.CreateAsync(commentModel);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.AuthorId, Is.EqualTo("author1"));
        Assert.That(result.Author, Is.Not.Null);
        Assert.That(result.Text, Is.EqualTo("Test comment"));
        Assert.That(result.ServiceOrderId, Is.EqualTo(1));
        Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));

        var savedEntity = await _context.Comments.FindAsync(result.Id);
        Assert.That(savedEntity, Is.Not.Null);
        Assert.That(savedEntity.AuthorId, Is.EqualTo("author1"));
        Assert.That(savedEntity.ServiceOrderId, Is.EqualTo(1));
        Assert.That(savedEntity.Text, Is.EqualTo("Test comment"));
    }

    [Test]
    public async Task CreateAsync_WithInvalidServiceOrder_ReturnsNull()
    {
        // Arrange
        var commentModel = new CommentModel
        {
            Text = "Test comment",
            ServiceOrderId = 999, // No such order
            AuthorId = "author1"
        };

        // Act
        var result = await _service.CreateAsync(commentModel);

        // Assert
        Assert.That(result, Is.Null);
    }
}
