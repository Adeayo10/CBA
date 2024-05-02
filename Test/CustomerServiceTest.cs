using CBA.Context;
using CBA.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentValidation.Results;

namespace CBA.Services.Tests;
public class CustomerServiceTest
{
    private readonly DbContextOptions<UserDataContext> _options;
    public CustomerServiceTest()
    {
        _options = new DbContextOptionsBuilder<UserDataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }

    [Fact]
    public async Task CreateCustomerAsync_Should_Return_CustomerResponse()
    {
        // Arrange
        using (var context = new UserDataContext(_options))
        {
            var customerValidator = new Mock<IValidator<CustomerEntity>>();
            var logger = new Mock<ILogger<CustomerService>>();
            var pdfService = new Mock<IPdfService>();

            customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
                .Returns(new ValidationResult());

            var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
            var customer = new CustomerDTO
            {
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                AccountType = CustomerAccountType.Savings,
                Status = "Active",
                Gender = "Male",
                Branch = "Main Branch",
                State = "California"
            };


            // Act
            var result = await service.CreateCustomerAsync(customer);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Status);
            Assert.Equal("Customer created successfully", result.Message);
            context.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task ChangeCustomerStatusAsync_Should_Return_CustomerResponse()
    {
        // Arrange
        using (var context = new UserDataContext(_options))
        {
            var customerValidator = new Mock<IValidator<CustomerEntity>>();
            var logger = new Mock<ILogger<CustomerService>>();
            var pdfService = new Mock<IPdfService>();

            customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
                .Returns(new ValidationResult());

            var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
            var customer = context.CustomerEntity.Add(new CustomerEntity
            {
                Id = new Guid(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main",
                AccountType = CustomerAccountType.Savings.ToString(),
                Status = "Active",
                State = "California",
                Gender = "Male",
                Branch = "Main Branch"
            });
            await context.SaveChangesAsync();
            var storedCustomer = await context.CustomerEntity.FindAsync(customer.Entity.Id);

            // Act
            var result = await service.ChangeAccountStatusAsync(storedCustomer.Id);

            // Assert
            Assert.NotNull(storedCustomer);
            Assert.NotNull(result);
            Assert.True(result.Status);
            Assert.Equal("Account status changed successfully", result.Message);

            context.Database.EnsureDeleted();

        }
    }

    [Fact]
    public async Task GetCustomerByIdAsync_Should_Return_CustomerResponse()
    {
        // Arrange
        using (var context = new UserDataContext(_options))
        {
            var customerValidator = new Mock<IValidator<CustomerEntity>>();
            var logger = new Mock<ILogger<CustomerService>>();
            var pdfService = new Mock<IPdfService>();

            customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
                .Returns(new ValidationResult());

            var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
            var customer = context.CustomerEntity.Add(new CustomerEntity
            {
                Id = new Guid("f5b1f1b1-1b1b-1b1b-1b1b-1b1b1b1b1b1b"),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main",
                AccountType = CustomerAccountType.Savings.ToString(),
                Status = "Active",
                State = "California",
                Gender = "Male",
                Branch = "Main Branch"
            });
            await context.SaveChangesAsync();
            var storedCustomer = await context.CustomerEntity.FindAsync(customer.Entity.Id);

            // Act
            var result = await service.GetCustomerByIdAsync(storedCustomer.Id);

            // Assert
            Assert.NotNull(storedCustomer);
            Assert.NotNull(result);
            Assert.Equal(storedCustomer.Id, new Guid("f5b1f1b1-1b1b-1b1b-1b1b-1b1b1b1b1b1b"));
            Assert.Equal(storedCustomer, result.Customer);
            Assert.Equal("Customer found", result.Message);

            context.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task GetCustomersAsync_Should_Return_CustomerResponse()
    {
        // Arrange
        using (var context = new UserDataContext(_options))
        {
            var customerValidator = new Mock<IValidator<CustomerEntity>>();
            var logger = new Mock<ILogger<CustomerService>>();
            var pdfService = new Mock<IPdfService>();

            customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
                .Returns(new ValidationResult());

            var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
            var customer = context.CustomerEntity.Add(new CustomerEntity
            {
                Id = new Guid(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main",
                AccountType = CustomerAccountType.Current.ToString(),
                Status = "Active",
                State = "California",
                Gender = "Male",
                Branch = "Main Branch"
            });
            var customer1 = context.CustomerEntity.Add(new CustomerEntity
            {
                Id = new Guid(),
                FullName = "John Paul",
                Email = "john.paul@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main",
                AccountType = CustomerAccountType.Current.ToString(),
                Status = "Active",
                State = "California",
                Gender = "Male",
                Branch = "Main Branch"
            });

            await context.SaveChangesAsync();
            var storedCustomer = await context.CustomerEntity.FindAsync(customer.Entity.Id);
            var storedCustomer1 = await context.CustomerEntity.FindAsync(customer1.Entity.Id);

            // Act
            var result = await service.GetCustomersAsync(1, 10, filterValue: "Current");


            // Assert
            Assert.NotNull(storedCustomer);
            Assert.NotNull(storedCustomer1);
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCustomers);
            Assert.Equal("Current", result.FilteredCustomers[0].AccountType);
            Assert.Equal("Current", result.FilteredCustomers[1].AccountType);
            Assert.Equal(2, result.CustomersPerAccountType["Current"]);
            context.Database.EnsureDeleted();

        }
    }

    [Fact]
    public async Task UpdateCustomerAsync_Should_Return_CustomerResponse()
    {
        // Arrange
        using (var context = new UserDataContext(_options))
        {
            var customerValidator = new Mock<IValidator<CustomerEntity>>();
            var logger = new Mock<ILogger<CustomerService>>();
            var pdfService = new Mock<IPdfService>();

            customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
                .Returns(new ValidationResult());

            var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
            var customer = context.CustomerEntity.Add(new CustomerEntity
            {
                Id = new Guid(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main",
                AccountType = CustomerAccountType.Current.ToString(),
                Status = "Active",
                State = "California",
                Gender = "Male",
                Branch = "Main Branch"
            });
            await context.SaveChangesAsync();
            var storedCustomer = await context.CustomerEntity.FindAsync(customer.Entity.Id);

            var updatedCustomer = new CustomerDTO
            {
                Id = storedCustomer.Id,
                FullName = "John Doe",
                Email = "adeayo@gmail.com",
                PhoneNumber = "1243567890",
                Address = "123 Mains",
                AccountType = CustomerAccountType.Current,
                Status = "Active",
                Gender = "Female",
                Branch = "Main Branch",
                State = "California"
            };

            // Act
            var result = await service.UpdateCustomerAsync(updatedCustomer);

            // Assert
            Assert.NotNull(storedCustomer);
            Assert.NotNull(result);
            Assert.True(result.Status);
            Assert.Equal("Customer details updated successfully", result.Message);
            context.Database.EnsureDeleted();

        }
    }

    [Fact]
    public async Task GetCustomerTransactionsAsync_Should_Return_CustomerResponse()
    {
        // Arrange
        using (var context = new UserDataContext(_options))
        {
            var customerValidator = new Mock<IValidator<CustomerEntity>>();
            var logger = new Mock<ILogger<CustomerService>>();
            var pdfService = new Mock<IPdfService>();

            customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
                .Returns(new ValidationResult());

            var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
            var customer = context.CustomerEntity.Add(new CustomerEntity
            {
                Id = new Guid(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main",
                AccountType = CustomerAccountType.Current.ToString(),
                Status = "Active",
                State = "California",
                Gender = "Male",
                Branch = "Main Branch",
                AccountNumber = "1234567890"

            });
            var transaction = context.Transaction.Add(new Transaction
            {
                Id = new Guid(),
                CustomerId = customer.Entity.Id,
                Amount = 1000,
                TransactionType = "Deposit",
                TransactionDescription = "Deposit",
                GLAccountId = 1,
                MoneyIn = 1000,
                MoneyOut = 0,
                Balance = 1000,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                TransactionDate = DateTime.Now.AddDays(-1),
            });
            var TransactionDTO = new TransactionDTO
            {
                AccountNumber = customer.Entity.AccountNumber,
                StartDate = transaction.Entity.TransactionDate,
                EndDate = DateTime.Now.AddDays(1)
            };
            await context.SaveChangesAsync();

            var storedCustomer = await context.CustomerEntity.FindAsync(customer.Entity.Id);
            var getTransaction = context.Transaction
                .Where(x => x.CustomerId == storedCustomer.Id && x.TransactionDate >= TransactionDTO.StartDate && x.TransactionDate <= TransactionDTO.EndDate).ToList();
           
        

            // Act
            var result = await service.GetTransactionsAsync(TransactionDTO);

            // Assert
            Assert.NotNull(storedCustomer);
            Assert.NotNull(result);
            Assert.Equal(1, result.Transactions.Count);
            Assert.True(result.Status);
            Assert.Equal("Transactions found", result.Message);
            Assert.Equal(1000,getTransaction[0].Amount);
            Assert.Equal(1000, getTransaction[0].MoneyIn);
            Assert.Equal(0, getTransaction[0].MoneyOut);
            Assert.Equal(1000, getTransaction[0].Balance);
            Assert.Equal("Deposit", getTransaction[0].TransactionType);
            Assert.Single(getTransaction);
            context.Database.EnsureDeleted();
        }
    }

    // [Fact]
    // public async Task GetCustomerStatementAsync_Should_Return_CustomerResponse()
    // {
    //     // Arrange
    //     using (var context = new UserDataContext(_options))
    //     {
    //         var customerValidator = new Mock<IValidator<CustomerEntity>>();
    //         var logger = new Mock<ILogger<CustomerService>>();
    //         var pdfService = new Mock<IPdfService>();

    //         customerValidator.Setup(x => x.Validate(It.IsAny<CustomerEntity>()))
    //             .Returns(new ValidationResult());

    //         var service = new CustomerService(context, customerValidator.Object, logger.Object, pdfService.Object);
    //         var customer = context.CustomerEntity.Add(new CustomerEntity
    //         {
    //             Id = new Guid(),
    //             FullName = "John Doe",
    //             Email = "john.doe@example.com",
    //             PhoneNumber = "1234567890",
    //             Address = "123 Main",
    //             AccountType = CustomerAccountType.Current.ToString(),
    //             Status = "Active",
    //             State = "California",
    //             Gender = "Male",
    //             Branch = "Main Branch",
    //             AccountNumber = "1234567890"

    //         });
    //         var transaction = context.Transaction.Add(new Transaction
    //         {
    //             Id = new Guid(),
    //             CustomerId = customer.Entity.Id,
    //             Amount = 1000,
    //             TransactionType = "Deposit",
    //             TransactionDescription = "Deposit",
    //             GLAccountId = 1,
    //             MoneyIn = 1000,
    //             MoneyOut = 0,
    //             Balance = 1000,
    //             CreatedAt = DateTime.Now,
    //             UpdatedAt = DateTime.Now,
    //             TransactionDate = DateTime.Now.AddDays(-1),
    //         });
    //         var TransactionDTO = new TransactionDTO
    //         {
    //             AccountNumber = customer.Entity.AccountNumber,
    //             StartDate = transaction.Entity.TransactionDate,
    //             EndDate = DateTime.Now.AddDays(1)
    //         };
    //         await context.SaveChangesAsync();

    //         var storedCustomer = await context.CustomerEntity.FindAsync(customer.Entity.Id);
    //         var getTransaction = context.Transaction
    //             .Where(x => x.CustomerId == storedCustomer.Id && x.TransactionDate >= TransactionDTO.StartDate && x.TransactionDate <= TransactionDTO.EndDate).ToList();

    //         // Act
    //         var result = await service.GetAccountStatementPdfAsync(TransactionDTO);

    //         // Assert
    //         Assert.NotNull(storedCustomer);
    //         Assert.NotNull(result);
    //         Assert.True(result.FileDownloadName.Contains("AccountStatement"));
    //         Assert.True(result.FileDownloadName.Contains("pdf"));
    //         Assert.True(result.FileDownloadName.Contains("1234567890"));
    //         Assert.True(result.FileDownloadName.Contains(DateTime.Now.ToString("dd-MM-yyyy")));
    //         context.Database.EnsureDeleted();

           
    //     }
    // }


}
