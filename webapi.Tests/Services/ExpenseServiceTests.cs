using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;
using webapi.Services;
using Xunit;

namespace WebApiTests.Services;

public class ExpenseServiceTests
{
    private ReceiptifyContext CreateContextWithData()
    {                
        var options = new DbContextOptionsBuilder<ReceiptifyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ReceiptifyContext(options);

        // Seed sample data
        context.Expenses.AddRange(
            new Expense
            {
                ExpenseId = 1,
                TotalAmount = 100,
                ExpenseDate = DateTime.UtcNow,
                Uid = "user1",
                SupplierId = 1,
                Supplier = new Supplier { SupplierId = 1, SupplierName = "Staples" },
                Receipt = new Receipt
                {
                    ReceiptId = 1,
                    ReceiptUrl = "http://test.com/receipt1.jpg",
                    UploadDate = DateTime.UtcNow
                }
            },
            new Expense
            {
                ExpenseId = 2,
                TotalAmount = 200,
                ExpenseDate = DateTime.UtcNow,
                Uid = "user2",
                SupplierId = 2,
                Supplier = new Supplier { SupplierId = 2, SupplierName = "Best Buy" },
                Receipt = null
            }
        );

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllExpenses()
    {
        // Arrange
        var context = CreateContextWithData();
        var service = new ExpenseService(context);

        // Act
        var result = await service.GetAll();

        // Assert
        result.Should().HaveCount(2);
        result.First().ExpenseId.Should().Be(1);
        result.Last().ExpenseId.Should().Be(2);
    }
    
    [Fact]
    public async Task GetById_ExistingId_ShouldReturnExpenseWithRelatedData()
    {
        var context = CreateContextWithData();
        var service = new ExpenseService(context);

        var result = await service.GetById(1);

        result.Should().NotBeNull();
        result.ExpenseId.Should().Be(1);
        result!.Supplier!.SupplierName.Should().Be("Staples");
        result.Receipt!.ReceiptUrl.Should().Be("http://test.com/receipt1.jpg");
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturnNull()
    {
        var context = CreateContextWithData();
        var service = new ExpenseService(context);

        var result = await service.GetById(3);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldAddNewExpense()
    {
        var options = new DbContextOptionsBuilder<ReceiptifyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ReceiptifyContext(options);
        var service = new ExpenseService(context);

        var newExpense = new Expense
        {
            ExpenseId = 3,
            TotalAmount = 50,
            ExpenseDate = DateTime.UtcNow,
            Uid = "user3",
            SupplierId = 3,
        };

        var result = await service.Create(newExpense);

        result.Should().NotBeNull();
        (await context.Expenses.CountAsync()).Should().Be(1);
        context.Expenses.First().Uid.Should().Be("user3");
    }    

    [Fact]
    public async Task DeleteById_ExistingId_ShouldRemoveExpense()
    {
        var context = CreateContextWithData();
        var service = new ExpenseService(context);

        await service.DeleteById(1);

        var expense = await context.Expenses.FindAsync(1);
        expense.Should().BeNull();
        (await context.Expenses.CountAsync()).Should().Be(1);
        (await context.Expenses.FirstAsync()).ExpenseId.Should().Be(2);
    }

    [Fact]
    public async Task DeleteById_NonExistingId_ShouldDoNothing()
    {
        var context = CreateContextWithData();
        var service = new ExpenseService(context);

        await service.DeleteById(999);

        (await context.Expenses.CountAsync()).Should().Be(2); // unchanged
    }
    
}
