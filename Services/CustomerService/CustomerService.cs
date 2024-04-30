using CBA.Models;
using CBA.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CBA.Services;
public class CustomerService : ICustomerService
{
    private readonly UserDataContext _context;
    private readonly IValidator<CustomerEntity> _CustomerValidator;
    private readonly ILogger<CustomerService> _logger;
    private readonly IPdfService _pdfService;
    public CustomerService(UserDataContext context, IValidator<CustomerEntity> customerValidator,
    ILogger<CustomerService> logger, IPdfService pdfService)
    {
        _context = context;
        _CustomerValidator = customerValidator;
        _logger = logger;
        _pdfService = pdfService;
        _logger.LogInformation("Customer Service has been created");
    }
    public async Task<CustomerResponse> CreateCustomerAsync(CustomerDTO customer)
    {
        async Task SaveCustomer(CustomerEntity customerEntity, CustomerBalance customerBalance)
        {
            await _context.CustomerEntity.AddAsync(customerEntity);
            await _context.CustomerBalance.AddAsync(customerBalance);
        }

        return await ProcessCustomerAsync(customer, SaveCustomer);
    }
    private async Task<CustomerResponse> ProcessCustomerAsync(CustomerDTO customer, Func<CustomerEntity, CustomerBalance, Task> saveCustomer)
    {
        var customerExists = await _context.CustomerEntity
            .FirstOrDefaultAsync(x => x.PhoneNumber == customer.PhoneNumber || x.Email == customer.Email);

        if (customerExists is not null)
        {
            return CreateCustomerExistsResponse();
        }

        var validationResult = ValidateCustomer(customer);
        if (!validationResult.Status)
        {
            return validationResult;
        }

        var customerEntity = CreateCustomerEntity(customer);
        var customerBalance = CreateCustomerBalanceEntity(customerEntity);

        _logger.LogInformation("Creating customer");
        _logger.LogInformation("Creating customer balance");

        await saveCustomer(customerEntity, customerBalance);
        await _context.SaveChangesAsync();

        return CreateCustomerCreatedResponse();
    }
    private CustomerResponse ValidateCustomer(CustomerDTO customer)
    {
        CustomerEntity customerEntity = CreateCustomerEntity(customer);

        _logger.LogInformation($"Customer phone: {customerEntity.PhoneNumber}");
        _logger.LogInformation($"Customer status: {customerEntity.Status}");

        var validationResult = _CustomerValidator.Validate(customerEntity);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            return CreateCustomerValidationFailedResponse(errorMessages);
        }

        return new CustomerResponse
        {
            Message = "Customer validation successful",
            Status = true
        };
    }
    private CustomerResponse CreateCustomerValidationFailedResponse(List<string> errorMessages)
    {
        _logger.LogInformation("Customer validation failed");
        return new CustomerResponse
        {
            Message = "Customer validation failed",
            Status = false,
            Errors = errorMessages
        };
    }
    private CustomerResponse CreateCustomerExistsResponse()
    {
        _logger.LogInformation("Customer with this phonenumber or Email already exists");
        return new CustomerResponse
        {
            Message = "Customer with this phonenumber or Email already exists",
            Status = false,
            Errors = new List<string> { "Customer already exists" }
        };
    }
    private CustomerResponse CreateCustomerCreatedResponse()
    {
        _logger.LogInformation("Customer created successfully");
        _logger.LogInformation("Customer balance created successfully");

        return new CustomerResponse
        {
            Message = "Customer created successfully",
            Status = true
        };
    }
    private static CustomerEntity CreateCustomerEntity(CustomerDTO customer)
    {

        var accountNumber = GenerateAccountNumber(customer.PhoneNumber!);
        var accountType = Enum.GetName(typeof(CustomerAccountType), customer.AccountType!.Value) ?? throw new ArgumentNullException(nameof(customer.AccountType));
        var customerEntity = new CustomerEntity
        {
            FullName = customer.FullName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            AccountNumber = accountNumber,
            Balance = 0,
            Gender = customer.Gender,
            Branch = customer.Branch,
            AccountType = accountType,
            Status = customer.Status,
            State = customer.State,
        };
        return customerEntity;
    }
    private static CustomerBalance CreateCustomerBalanceEntity(CustomerEntity customerEntity)
    {
        var customerBalance = new CustomerBalance
        {
            AccountName = customerEntity.FullName,
            AccountNumber = customerEntity.AccountNumber,
            AccountType = customerEntity.AccountType?.ToString(),
            LedgerBalance = 0,
            AvailableBalance = 0,
            WithdrawableBalance = 0,
            CustomerId = customerEntity.Id
        };
        return customerBalance;
    }
    private static string GenerateAccountNumber(string PhoneNumber)
    {
        return PhoneNumber[^10..];
    }
    public async Task<CustomerResponse> UpdateCustomerAsync(CustomerDTO customer)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(customer.Id);
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return CreateCustomerNotFoundResponse();
        }
        var validationResult = ValidateCustomer(customer);
        if (!validationResult.Status)
        {
            return validationResult;
        }
        UpdateCustomerEntityAsync(customerEntity, customer);

        _logger.LogInformation("Customer details updated successfully");

        return CreateCustomerDetailsUpdatedResponse();
    }
    private static CustomerResponse CreateCustomerNotFoundResponse() => new()
    {
        Message = "Customer not found",
        Status = false,
        Errors = new List<string> { "Customer not found" }
    };
    private static TransactionResponse TransactionNotFoundResponse() => new()
    {
        Message = "Customer not found",
        Status = false,
        Errors = new List<string> { "Customer not found" }
    };
    private async void UpdateCustomerEntityAsync(CustomerEntity customerEntity, CustomerDTO customer)
    {
        customerEntity.FullName = customer.FullName;
        customerEntity.Email = customer.Email;
        customerEntity.PhoneNumber = customer.PhoneNumber;
        customerEntity.Address = customer.Address;
        await _context.SaveChangesAsync();

    }
    private static CustomerResponse CreateCustomerDetailsUpdatedResponse() => new()
    {
        Message = "Customer details updated successfully",
        Status = true
    };
    public async Task<CustomerResponse> GetCustomerByIdAsync(Guid id)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(id);
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return CreateCustomerNotFoundResponse();
        }
        _logger.LogInformation("Customer found");
        return new CustomerResponse
        {
            Message = "Customer found",
            Status = true,
            Customer = customerEntity
        };
    }
    public async Task<CustomerResponse> ValidateCustomerByAccountNumberAsync(string accountNumber)
    {
        var customerEntity = await _context.CustomerEntity.Where(x => x.AccountNumber == accountNumber).SingleAsync();
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return CreateCustomerNotFoundResponse();
        }
        _logger.LogInformation("Customer found");
        return new CustomerResponse
        {
            Message = "Customer found",
            Status = true,
            Customer = customerEntity
        };
    }
    public async Task<dynamic> GetCustomersAsync(int pageNumber, int pageSize, string? filterValue)
    {
        _logger.LogInformation("Getting customers");

        var totalCustomersTask = await GetTotalCustomersAsync();
        var customersPerAccountTypeTask = await GetCustomersPerAccountTypeAsync();
        var customersTask = await _context.CustomerEntity
            .OrderBy(x => x.DateCreated)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCustomers = totalCustomersTask;
        var customersPerAccountType = customersPerAccountTypeTask;
        var customers = customersTask;

        var filteredCustomers = customers
            .Where(x => x.AccountType != null && x.AccountType.ToString().Equals(filterValue, StringComparison.OrdinalIgnoreCase))
            .Select(x => new CustomerEntity
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address,
                AccountNumber = x.AccountNumber,
                Balance = x.Balance,
                AccountType = x.AccountType,
                Status = x.Status,
                DateCreated = x.DateCreated,
                State = x.State,
                Gender = x.Gender,
                Branch = x.Branch

            })
            .ToList();
                
        _logger.LogInformation($"Filter Value: {filterValue}");
        _logger.LogInformation($"Number of customers: {customers.Count}");
        _logger.LogInformation($"Account Types: {string.Join(", ", customers.Select(c => c.AccountType))}");

        _logger.LogInformation($"Total customers: {filteredCustomers.Count}");
        var result = new
        {
            TotalCustomers = totalCustomers,
            CustomersPerAccountType = customersPerAccountType,
            FilteredCustomers = filteredCustomers
        };

        return result;
    }
    private async Task<int> GetTotalCustomersAsync()
    {
        return await _context.CustomerEntity.CountAsync();
    }
    private async Task<Dictionary<string, int>> GetCustomersPerAccountTypeAsync()
    {
        return await _context.CustomerEntity
            .GroupBy(x => x.AccountType)
            .Select(x => new
            {
                AccountType = x.Key,
                Count = x.Count()
            })
            .ToDictionaryAsync(x => x.AccountType!, x => x.Count);
    }
    public async Task<CustomerResponse> GetCustomerAccountBalanceAsync(Guid id)
    {
        var getCustomer = await GetCustomerByIdAsync(id);
        var customerEntity = getCustomer.Customer;
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return CreateCustomerNotFoundResponse();
        }

        var customerBalance = await _context.CustomerBalance.SingleAsync(x => x.AccountNumber == customerEntity.AccountNumber);
        if (customerBalance is null)
        {
            _logger.LogInformation("Customer balance not found");
            return new CustomerResponse
            {
                Message = "Customer balance not found",
                Status = false
            };
        }

        var customerBalanceDTO = new CustomerBalanceDTO
        {
            AccountName = customerBalance.AccountName,
            AccountNumber = customerBalance.AccountNumber,
            AccountType = customerBalance.AccountType,
            LedgerBalance = customerBalance.LedgerBalance,
            AvailableBalance = customerBalance.AvailableBalance,
            WithdrawableBalance = customerBalance.WithdrawableBalance
        };

        _logger.LogInformation("Customer balance found");
        return new CustomerResponse
        {
            Message = "Customer balance found",
            Status = true,
            Data = customerBalanceDTO
        };
    }
    public async Task<CustomerResponse> ChangeAccountStatusAsync(Guid id)
    {
        var customerEntity = await _context.CustomerEntity.FirstOrDefaultAsync(x => x.Id == id);
        if (customerEntity == null)
        {
            return new CustomerResponse()
            {
                Message = "Account does not exist",
                Status = false
            };
        }

        customerEntity.Status = customerEntity.Status == "Active" ? "Inactive" : "Active";
        await _context.SaveChangesAsync();

        return new CustomerResponse()
        {
            Message = "Account status changed successfully",
            Status = true
        };
    }
    public object GetAccountTypes()
    {
        var accountTypes = Enum.GetValues(typeof(CustomerAccountType)).Cast<CustomerAccountType>().ToList();
        var mappedAccountTypes = accountTypes.Select(accountTypes => new
        {
            Id = (int)accountTypes,
            Name = accountTypes.ToString()
        }).ToList();
        return mappedAccountTypes;
    }
    public async Task<TransactionResponse> GetTransactionsAsync(TransactionDTO transaction)
    {
        var customerEntity = await _context.CustomerEntity.SingleOrDefaultAsync(x => x.AccountNumber == transaction.AccountNumber);
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return TransactionNotFoundResponse();
        }

        var transactions = await _context.Transaction
            .Where(x => x.CustomerId == customerEntity.Id && x.TransactionDate >= transaction.StartDate && x.TransactionDate <= transaction.EndDate)
            .ToListAsync();
        var startDate = transaction.StartDate.ToString() ?? throw new ArgumentNullException(nameof(transaction.StartDate));
        var startDateInYYYYMMDD = DateTime.Parse(startDate).ToString("yyyy-MM-dd");
        var endDate = transaction.EndDate.ToString() ?? throw new ArgumentNullException(nameof(transaction.EndDate));
        var endDateInYYYYMMDD = DateTime.Parse(endDate).ToString("yyyy-MM-dd");

        bool transactionsExists = transactions.Count > 0;

        string message = transactionsExists ? "Transactions found" : "No Transactions Found";

        _logger.LogInformation(message);
        _logger.LogInformation($"Transactions: {transactions.ToArray()}");

        return new TransactionResponse
        {
            Id = customerEntity.Id.ToString(),
            Message = message,
            Status = transactionsExists,
            Transactions = transactions,
            StartDate = startDateInYYYYMMDD,
            EndDate = endDateInYYYYMMDD
        };

    }
    public async Task<FileContentResult> GetAccountStatementPdfAsync(TransactionDTO transaction)
    {
        var transactions = await GetTransactionsAsync(transaction);
        if (transactions.Transactions == null || transactions.Transactions.Count == 0) { throw new Exception(transactions.Message); }
        var filePath = Path.Combine(Path.GetTempPath(), $"{transaction.AccountNumber}.pdf");
        _logger.LogInformation($"File path: {filePath} ");
        _logger.LogInformation($"transactions: {transactions.Transactions.ToArray()} ");
        await _pdfService.CreateAccountStatementPdfAsync(transactions.Transactions, transactions.Id, filePath, transactions.StartDate, transactions.EndDate);

        using var memory = new MemoryStream();
        await using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;
        var file = new FileContentResult(memory.ToArray(), "application/pdf")
        {
            FileDownloadName = Path.GetFileName(filePath)
        };
        _logger.LogInformation("File created and downloaded successfully");

        return file;


    }

    //public async Task<CustomerResponse> UpdateCustomerBalance(Guid id, decimal amount)
    //     {
    //         var customerEntity = await _context.CustomerEntity.FindAsync(id);
    //         if (customerEntity is null)
    //         {
    //             _logger.LogInformation("Customer not found");
    //             return new CustomerResponse
    //             {
    //                 Message = "Customer not found",
    //                 Status = false,
    //                 Errors = new List<string> { "Customer not found" }
    //             };
    //         }
    //         _logger.LogInformation("Updating customer balance");
    //         customerEntity.Balance = amount;
    //         _context.CustomerEntity.Update(customerEntity);
    //         await _context.SaveChangesAsync();
    //         _logger.LogInformation("Customer balance updated successfully");
    //         return new CustomerResponse
    //         {
    //             Message = "Customer balance updated successfully",
    //             Status = true
    //         };
    //     } 

}