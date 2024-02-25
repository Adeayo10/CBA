using CBA.Models;
using CBA.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CBA.Services;

public class CustomerService : ICustomerService
{
    private readonly UserDataContext _context;
    private readonly IValidator<CustomerEntity> _CustomerValidator;
    private readonly ILogger<CustomerService> _logger;
    public CustomerService(UserDataContext context, IValidator<CustomerEntity> customerValidator, ILogger<CustomerService> logger)
    {
        _context = context;
        _CustomerValidator = customerValidator;
        _logger = logger;
        _logger.LogInformation("Customer Service has been created");
    }
    public async Task<CustomerResponse> CreateCustomer(CustomerDTO customer)
    {
        _logger.LogInformation("Creating customer");
        var CustomerExists = _context.CustomerEntity.Where(x => x.PhoneNumber == customer.PhoneNumber || x.Email == customer.Email).FirstOrDefault();
        if (CustomerExists != null)
        {
            _logger
            .LogInformation("Customer with this phonenumber or Email already exists");
            return new CustomerResponse
            {
                Message = "Customer with this phonenumber or Email already exists",
                Status = false,
                Errors = new List<string> { "Customer already exists" }
            };
        }
        var validationResult = _CustomerValidator.Validate(CreateCustomerEntity(customer));
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Customer validation failed");
            return new CustomerResponse
            {
                Message = "Customer validation failed",
                Status = false,
                Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList()
            };
        }
        var customerEntity = CreateCustomerEntity(customer);
        var customerBalance = CreateCustomerBalanceEntity(customerEntity);
        _logger.LogInformation("Creating customer");
        _logger.LogInformation("Creating customer balance");
        await _context.CustomerEntity.AddAsync(customerEntity);
        await _context.CustomerBalance.AddAsync(customerBalance);
        await _context.SaveChangesAsync();
        await _context.SaveChangesAsync();

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
        var customerEntity = new CustomerEntity
        {
            FullName = customer.FullName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            AccountNumber = GenerateAccountNumber(customer.PhoneNumber!),
            Balance = 0,
            Gender = customer.Gender,
            Branch = customer.Branch,
            AccountType = customer.AccountType!.Value,
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
            AccountType = customerEntity.AccountType.ToString(),
            LedgerBalance = 0,
            AvailableBalance = 0,
            WithdrawableBalance = 0,
            CustomerId = customerEntity.Id
        };
        return customerBalance;
    }
    private static string GenerateAccountNumber(string PhoneNumber)
    {
        return PhoneNumber.Substring(1, 10);
    }
    public async Task<CustomerResponse> UpdateCustomerDetails(CustomerDTO customer)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(customer.Id);
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return new CustomerResponse
            {
                Message = "Customer not found",
                Status = false,
                Errors = new List<string> { "Customer not found" }
            };
        }
        var validationResult = _CustomerValidator.Validate(CreateCustomerEntity(customer));
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Customer validation failed");
            return new CustomerResponse
            {
                Message = "Customer validation failed",
                Status = false,
                Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList()
            };
        }
        _logger.LogInformation("Updating customer details");
        customerEntity.FullName = customer.FullName;
        customerEntity.Email = customer.Email;
        customerEntity.PhoneNumber = customer.PhoneNumber;
        customerEntity.Address = customer.Address;
        _context.CustomerEntity.Update(customerEntity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Customer details updated successfully");
        return new CustomerResponse
        {
            Message = "Customer details updated successfully",
            Status = true
        };
    }
    public async Task<CustomerResponse> GetCustomerById(Guid id)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(id);
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return new CustomerResponse
            {
                Message = "Customer not found",
                Status = false,
                Errors = new List<string> { "Customer not found" }
            };
        }
        _logger.LogInformation("Customer found");
        return new CustomerResponse
        {
            Message = "Customer found",
            Status = true,
            Customer = customerEntity
        };
    }
    public async Task<CustomerResponse> ValidateCustomerByAccountNumber(string accountNumber)
    {
        var customerEntity = await _context.CustomerEntity.Where(x => x.AccountNumber == accountNumber).FirstOrDefaultAsync();
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return new CustomerResponse
            {
                Message = "Customer not found",
                Status = false,
                Errors = new List<string> { "Customer not found" }
            };
        }
        _logger.LogInformation("Customer found");
        return new CustomerResponse
        {
            Message = "Customer found",
            Status = true,
            Customer = customerEntity
        };
    }
    public async Task<IEnumerable<CustomerEntity>> GetCustomers()
    {
        _logger.LogInformation("Getting customers");
        var customers = await _context.CustomerEntity.ToListAsync();
        var filteredcustomersCurrentAndSavings = customers.Where(x => x.AccountType == CustomerAccountType.Current || x.AccountType == CustomerAccountType.Savings).ToList();
        var filteredcustomers = filteredcustomersCurrentAndSavings.Select(x => new CustomerEntity
        {
            Id = x.Id,
            FullName = x.FullName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            Address = x.Address,
            AccountNumber = x.AccountNumber,
            Balance = x.Balance
        }).ToList();
        _logger.LogInformation("Customers found");
        return  filteredcustomers;
    }
    public async Task<CustomerResponse> GetCustomerAccountBalance(Guid id)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(id);
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return new CustomerResponse
            {
                Message = "Customer not found",
                Status = false,
                Errors = new List<string> { "Customer not found" }
            };
        }
        var customerBalance = await _context.CustomerBalance.Where(x => x.AccountNumber == customerEntity.AccountNumber).FirstOrDefaultAsync();
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
            Data =  customerBalanceDTO
        };
    }
    public async Task<CustomerResponse> ChangeAccountStatus(Guid id)
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
    public  object GetAccountTypes()
    {
        var accountTypes = Enum.GetValues(typeof(CustomerAccountType)).Cast<CustomerAccountType>().ToList();
        var mappedAccountTypes = accountTypes.Select(x => new
        {
            Id = (int)x,
            Name = x.ToString()
        }).ToList();
        return mappedAccountTypes;
    }
    public async Task<CustomerResponse> GetTransactions(TransactionDTO transaction)
    {
        var customerEntity = await _context.CustomerEntity.Where(x => x.AccountNumber == transaction.AccountNumber).FirstOrDefaultAsync();
        if (customerEntity is null)
        {
            _logger.LogInformation("Customer not found");
            return new CustomerResponse
            {
                Message = "Customer not found",
                Status = false,
                Errors = new List<string> { "Customer not found" }
            };
        }
        var transactions = await _context.Transaction.Where(x => x.CustomerId == customerEntity.Id).ToListAsync();
        var filteredTransactions = transactions.Where(x => x.TransactionDate >= transaction.StartDate && x.TransactionDate <= transaction.EndDate).ToList();
        _logger.LogInformation("Transactions found");
        return new CustomerResponse
        {
            Message = "Transactions found",
            Status = true,
            Transaction = filteredTransactions
        };
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