using CBA.Models;
using CBA.Context;
using Microsoft.EntityFrameworkCore;

namespace CBA.Services;
public class PostingService : IPostingService
{
    private readonly UserDataContext _context;
    private readonly ILogger<PostingService> _logger;
    private readonly ILedgerService _ledgerService;
    public PostingService(UserDataContext context, ILogger<PostingService> logger, ILedgerService ledgerService)
    {
        _context = context;
        _logger = logger;
        _ledgerService = ledgerService;
    }
    public async Task<CustomerResponse> Deposit(PostingDTO customerDeposit)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(customerDeposit.CustomerAccountNumber);
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
        var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalance();
        var LedgerEntity = await _context.GLAccounts.FindAsync(customerDeposit.AccountNumber);
        var customerBalance = await _context.CustomerBalance.FindAsync(customerEntity.AccountNumber);
        if (LedgerBalance < customerDeposit.Amount)
        {
            _logger.LogInformation("Insufficient funds in Ledger balance");
            return new CustomerResponse
            {
                Message = "Insufficient funds",
                Status = false,
                Errors = new List<string> { "Insufficient funds in ledgerbalance" }
            };
        }
        _logger.LogInformation("Depositing into customer account");
        customerEntity.Balance += customerDeposit.Amount;
        LedgerBalance -= customerDeposit.Amount;

        customerBalance.LedgerBalance += LedgerBalance;
        customerBalance.AvailableBalance += customerDeposit.Amount;
        customerBalance.WithdrawableBalance += customerDeposit.Amount;

        var postingEntity = new PostingEntity
        {
            AccountName = LedgerEntity.AccountName,
            AccountNumber = LedgerEntity.AccountNumber,
            Amount = customerDeposit.Amount,
            TransactionType = "Deposit",
            Narration = customerDeposit.Narration,
            CustomerId = customerEntity.Id.ToString(),
            CustomerName = customerEntity.FullName,
            CustomerAccountNumber = customerEntity.AccountNumber,
            CustomerAccountType = customerEntity.AccountType.ToString(),
            CustomerBranch = customerEntity.Branch,
            CustomerEmail = customerEntity.Email,
            CustomerPhoneNumber = customerEntity.PhoneNumber,
            CustomerStatus = customerEntity.Status,
            CustomerGender = customerEntity.Gender,
            CustomerAddress = customerEntity.Address,
            CustomerState = customerEntity.State,
        };

        var transaction = new Transaction
        {
            TransactionType = "Deposit",
            TransactionDescription = customerDeposit.Narration,
            Amount = customerDeposit.Amount,
            GLAccountId = LedgerEntity.Id,
            CustomerId = customerEntity.Id,
        };
        await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(postingEntity);  
        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerBalance.Update(customerBalance);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deposit successful");
        return new CustomerResponse
        {
            Message = "Deposit successful",
            Status = true
        };
    }

    public async Task<CustomerResponse> Withdraw(PostingDTO customerWithdraw)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(customerWithdraw.CustomerAccountNumber);
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
        var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalance();
        var LedgerEntity = await _context.GLAccounts.FindAsync(customerWithdraw.AccountNumber);
        var customerBalance = await _context.CustomerBalance.FindAsync(customerEntity.AccountNumber);
        if (customerEntity.Balance < customerWithdraw.Amount)
        {
            _logger.LogInformation("Insufficient funds");
            return new CustomerResponse
            {
                Message = "Insufficient funds",
                Status = false,
                Errors = new List<string> { "Insufficient funds" }
            };
        }
        if (LedgerBalance < customerWithdraw.Amount)
        {
            _logger.LogInformation("Insufficient funds in Ledger balance");
            return new CustomerResponse
            {
                Message = "Insufficient funds",
                Status = false,
                Errors = new List<string> { "Insufficient funds in ledgerbalance" }
            };
        }
        _logger.LogInformation("Withdrawing from customer account");
        customerEntity.Balance -= customerWithdraw.Amount;
        LedgerBalance += customerWithdraw.Amount;

        customerBalance.LedgerBalance += LedgerBalance;
        customerBalance.AvailableBalance -= customerWithdraw.Amount;
        customerBalance.WithdrawableBalance -= customerWithdraw.Amount;

        var postingEntity = new PostingEntity
        {
            AccountName = customerEntity.FullName,
            AccountNumber = customerEntity.AccountNumber,
            Amount = customerWithdraw.Amount,
            TransactionType = "Withdrawal",
            Narration = customerWithdraw.Narration,
            CustomerId = customerEntity.Id.ToString(),
            CustomerName = customerEntity.FullName,
            CustomerAccountNumber = customerEntity.AccountNumber,
            CustomerAccountType = customerEntity.AccountType.ToString(),
            CustomerBranch = customerEntity.Branch,
            CustomerEmail = customerEntity.Email,
            CustomerPhoneNumber = customerEntity.PhoneNumber,
            CustomerStatus = customerEntity.Status,
            CustomerGender = customerEntity.Gender,
            CustomerAddress = customerEntity.Address,
            CustomerState = customerEntity.State,
        };
        var transaction = new Transaction
        {
            TransactionType = "Withdrawal",
            TransactionDescription = customerWithdraw.Narration,
            Amount = customerWithdraw.Amount,
            GLAccountId = LedgerEntity.Id,
            CustomerId = customerEntity.Id,
        };
        await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(postingEntity);
        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerBalance.Update(customerBalance);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Withdrawal successful");
        return new CustomerResponse
        {
            Message = "Withdrawal successful",
            Status = true
        };
    }
    public async Task<CustomerResponse> Transfer(CustomerTransferDTO customerTransfer)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(customerTransfer.SenderAccountNumber);
        var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalance();
        var customerBalance = await _context.CustomerBalance.FindAsync(customerEntity.AccountNumber);
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
        var recipient = await _context.CustomerEntity.FindAsync(customerTransfer.ReceiverAccountNumber);
        if (recipient is null)
        {
            _logger.LogInformation("Recipient not found");
            return new CustomerResponse
            {
                Message = "Recipient not found",
                Status = false,
                Errors = new List<string> { "Recipient not found" }
            };
        }
        _logger.LogInformation("Transferring from customer account");
        if (customerEntity.Balance < customerTransfer.Amount)
        {
            _logger.LogInformation("Insufficient funds");
            return new CustomerResponse
            {
                Message = "Insufficient funds",
                Status = false,
                Errors = new List<string> { "Insufficient funds" }
            };
        }
        customerEntity.Balance -= customerTransfer.Amount;
        recipient.Balance += customerTransfer.Amount;
        LedgerBalance += customerTransfer.Amount;

        customerBalance.LedgerBalance += LedgerBalance;
        customerBalance.AvailableBalance -= customerTransfer.Amount;
        customerBalance.WithdrawableBalance -= customerTransfer.Amount;

        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerEntity.Update(recipient);
        _context.CustomerBalance.Update(customerBalance);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Transfer successful");
        return new CustomerResponse
        {
            Message = "Transfer successful",
            Status = true
        };
    }
}




