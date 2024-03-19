using CBA.Models;
using CBA.Context;

namespace CBA.Services;
public class PostingService : IPostingService
{
    private readonly UserDataContext _context;
    private readonly ILogger<PostingService> _logger;
    private readonly ILedgerService _ledgerService;
    private readonly IEmailService _emailService;
    public PostingService(UserDataContext context, ILogger<PostingService> logger, ILedgerService ledgerService, IEmailService emailService)
    {
        _context = context;
        _logger = logger;
        _ledgerService = ledgerService;
        _emailService = emailService;
    }
    public async Task<CustomerResponse> DepositAsync(PostingDTO customerDeposit)
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

        var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerDeposit.AccountNumber);
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
        await PerformDepositAsync(LedgerBalance, customerDeposit, customerEntity, LedgerEntity, customerBalance);
        _logger.LogInformation("Deposit successful");
        await SendEmailReceiptAsync(customerEntity);
        _logger.LogInformation("Email sent");
        return new CustomerResponse
        {
            Message = "Deposit successful",
            Status = true
        };
    }
    private async Task PerformDepositAsync(decimal LedgerBalance, PostingDTO customerDeposit, CustomerEntity customerEntity, GLAccounts LedgerEntity, CustomerBalance customerBalance)
    {
        customerEntity.Balance += customerDeposit.Amount;
        LedgerBalance -= customerDeposit.Amount;
        customerBalance.LedgerBalance += LedgerBalance;
        customerBalance.AvailableBalance += customerDeposit.Amount;
        customerBalance.WithdrawableBalance += customerDeposit.Amount;

        PostingEntity postingEntity = PostingEntityForDeposit(customerDeposit, customerEntity, LedgerEntity);
        Transaction transaction = TransactionEntityForDeposit(customerDeposit, customerEntity, LedgerEntity);
        await DatabasePersistenceForDepositAsync(customerEntity, customerBalance, postingEntity, transaction);
    }

    private async Task DatabasePersistenceForDepositAsync(CustomerEntity? customerEntity, CustomerBalance? customerBalance, PostingEntity postingEntity, Transaction transaction)
    {
        await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(postingEntity);
        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerBalance.Update(customerBalance);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deposit successful");
    }

    private static Transaction TransactionEntityForDeposit(PostingDTO customerDeposit, CustomerEntity? customerEntity, GLAccounts? LedgerEntity)
    {
        return new Transaction
        {
            TransactionType = "Deposit",
            TransactionDescription = customerDeposit.Narration,
            Amount = customerDeposit.Amount,
            GLAccountId = LedgerEntity.Id,
            CustomerId = customerEntity.Id,
        };
    }

    private static PostingEntity PostingEntityForDeposit(PostingDTO customerDeposit, CustomerEntity? customerEntity, GLAccounts? LedgerEntity)
    {
        return new PostingEntity
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
    }

    private static string GenerateReceiptTableRo(CustomerEntity customer)
    {
        return $@"
        <tr>
            <td>{customer.AccountNumber}</td>
            <td>{customer.FullName}</td>
            <td>{customer.Balance}</td>
            <td>{customer.Branch}</td>
            <td>{DateTime.Now}</td>
        </tr>";
    }

    private async Task SendEmailReceiptAsync(CustomerEntity customerEntity)
    {
        var receiptTableRows = GenerateReceiptTableRo(customerEntity);
        
        var htmlReceiptTable = $@"
        <table>
            <thead>
                <tr>
                    <th>Account Number</th>
                    <th>Full Name</th>
                    <th>Balance</th>
                    <th>Branch</th>
                    <th>Date</th>
                </tr>
            </thead>
            <tbody>
                {receiptTableRows}
            </tbody>
        </table>";

        var message = new Message(new string[] { customerEntity.Email }, "Transaction Receipt", htmlReceiptTable);
        await _emailService.SendEmail(message);
    }
    public async Task<CustomerResponse> WithdrawAsync(PostingDTO customerWithdraw)
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

        var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerWithdraw.AccountNumber);
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
        _logger.LogInformation("Withdrawing from customer account");
        await PerformWithdrawAsync(LedgerBalance, customerWithdraw, customerEntity, customerBalance, LedgerEntity);
        _logger.LogInformation("Withdrawal successful");
        await SendEmailReceiptAsync(customerEntity);
        _logger.LogInformation("Email sent");
        return new CustomerResponse
        {
            Message = "Withdrawal successful",
            Status = true
        };
    }
    private async Task PerformWithdrawAsync(decimal LedgerBalance, PostingDTO customerWithdraw, CustomerEntity customerEntity, CustomerBalance customerBalance, GLAccounts LedgerEntity)
    {
        customerEntity.Balance -= customerWithdraw.Amount;
        LedgerBalance += customerWithdraw.Amount;
        customerBalance.LedgerBalance += LedgerBalance;
        customerBalance.AvailableBalance -= customerWithdraw.Amount;
        customerBalance.WithdrawableBalance -= customerWithdraw.Amount;

        PostingEntity postingEntity = PostingEntityForWithdraw(customerWithdraw, customerEntity, LedgerEntity);
        Transaction transaction = TransactionEntityForWithdraw(customerWithdraw, customerEntity, LedgerEntity);
        await DatabasePersistenceForWithdrawAsync(customerEntity, customerBalance, postingEntity, transaction);
    }
    private async Task DatabasePersistenceForWithdrawAsync(CustomerEntity? customerEntity, CustomerBalance? customerBalance, PostingEntity postingEntity, Transaction transaction)
    {
        await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(postingEntity);
        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerBalance.Update(customerBalance);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Withdrawal successful");
    }
    private static Transaction TransactionEntityForWithdraw(PostingDTO customerWithdraw, CustomerEntity? customerEntity, GLAccounts? LedgerEntity)
    {
        return new Transaction
        {
            TransactionType = "Withdrawal",
            TransactionDescription = customerWithdraw.Narration,
            Amount = customerWithdraw.Amount,
            GLAccountId = LedgerEntity.Id,
            CustomerId = customerEntity.Id,
        };
    }
    private static PostingEntity PostingEntityForWithdraw(PostingDTO customerWithdraw, CustomerEntity customerEntity, GLAccounts LedgerEntity)
    {
        return new PostingEntity
        {
            AccountName = LedgerEntity.AccountName,
            AccountNumber = LedgerEntity.AccountNumber,
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
            CustomerState = customerEntity.State
        };
    }
    public async Task<CustomerResponse> TransferAsync(CustomerTransferDTO customerTransfer)
    {
        var customerEntity = await _context.CustomerEntity.FindAsync(customerTransfer.SenderAccountNumber);
        var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerTransfer.SenderAccountNumber);
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
//pagination for posting service
