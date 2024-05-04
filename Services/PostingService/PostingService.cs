using CBA.Models;
using CBA.Context;
using Microsoft.EntityFrameworkCore;

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
        var customerEntity = await _context.CustomerEntity.SingleOrDefaultAsync(x => x.AccountNumber == customerDeposit.CustomerAccountNumber);
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

        var ledgerEntity = await _context.GLAccounts.SingleOrDefaultAsync(x => x.AccountNumber == customerDeposit.LedgerAccountNumber);
        if (ledgerEntity is null)
        {
            _logger.LogInformation("Ledger not found");
            return new CustomerResponse
            {
                Message = "Ledger not found",
                Status = false,
                Errors = new List<string> { "Ledger not found" }
            };
        }

        var ledgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerDeposit.LedgerAccountNumber!);
        var customerBalance = await _context.CustomerBalance.SingleOrDefaultAsync(x => x.AccountNumber == customerDeposit.CustomerAccountNumber);

        if (ledgerBalance < customerDeposit.Amount)
        {
            _logger.LogInformation("Insufficient funds in Ledger balance");
            return new CustomerResponse
            {
                Message = "Insufficient funds",
                Status = false,
                Errors = new List<string> { "Insufficient funds in ledger balance" }
            };
        }

        _logger.LogInformation("Depositing into customer account");
        await PerformDepositAsync(customerDeposit, customerEntity, ledgerEntity!, customerBalance!/*, ledgerBalance,*/ );
        _logger.LogInformation("Deposit successful");
        await SendEmailReceiptAsync(customerEntity, customerDeposit.Amount);
        _logger.LogInformation("Email sent");
        return new CustomerResponse
        {
            Message = "Deposit successful",
            Status = true
        };
    }
    private async Task PerformDepositAsync(PostingDTO customerDeposit, CustomerEntity customerEntity, GLAccounts LedgerEntity, CustomerBalance customerBalance, decimal LedgerBalance = 0.0m)
    {
        customerEntity.Balance += customerDeposit.Amount; 
        LedgerEntity.Balance -= customerDeposit.Amount;  

        // var ledger = await _context.GLAccounts.SingleAsync(x=> x.AccountNumber == customerDeposit.LedgerAccountNumber);
        //ledger.Balance = LedgerBalance;
        //_context.GLAccounts.Update(ledger);
        var customerLedgerBalance = _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerDeposit.LedgerAccountNumber!);
        customerBalance.LedgerBalance = customerLedgerBalance.Result - customerDeposit.Amount;
        customerBalance.AvailableBalance += customerDeposit.Amount;
        customerBalance.WithdrawableBalance += customerDeposit.Amount;

        PostingEntity postingEntity = PostingEntityForDeposit(customerDeposit, customerEntity, LedgerEntity);
        Transaction transaction = TransactionEntityForDeposit(customerDeposit, customerEntity, LedgerEntity);

        _context.GLAccounts.Update(LedgerEntity);
        await SaveDepositAsync(customerEntity, customerBalance, postingEntity, transaction);
    }
    private async Task SaveDepositAsync(CustomerEntity customerEntity, CustomerBalance customerBalance, PostingEntity postingEntity, Transaction transaction)
    {
        await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(postingEntity);
       
        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerBalance.Update(customerBalance);
        await SaveChangesAsync();
        _logger.LogInformation("Deposit successful");
    }
    private static Transaction TransactionEntityForDeposit(PostingDTO customerDeposit, CustomerEntity customerEntity, GLAccounts LedgerEntity)
    {
        var moneyin = customerDeposit.Amount;
        var transactionBalance = customerEntity.Balance;

        return new Transaction
        {
            TransactionType = "Deposit",
            TransactionDescription = customerDeposit.Narration,
            Amount = customerDeposit.Amount,
            GLAccountId = LedgerEntity.Id,
            CustomerId = customerEntity.Id,
            MoneyIn = moneyin,
            Balance = transactionBalance,
        };
    }
    private static PostingEntity PostingEntityForDeposit(PostingDTO customerDeposit, CustomerEntity customerEntity, GLAccounts LedgerEntity)
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
            Branch = customerEntity.Branch,
            //Teller = ""Pass in the teller name as the user"",
            AccountType = customerEntity.AccountType.ToString(),
            CustomerFullName = customerEntity.FullName,
            CustomerBalance = customerEntity.Balance.ToString(),
        };
    }
    private static string GenerateReceiptTableRow(CustomerEntity customer)
    {
        return $@"
            <tr>
                <td style='border: 1px solid black; padding: 5px;'>{customer.AccountNumber}</td>
                <td style='border: 1px solid black; padding: 5px;'>{customer.FullName}</td>
                <td style='border: 1px solid black; padding: 5px;'>{customer.Balance}</td>
                <td style='border: 1px solid black; padding: 5px;'>{customer.Branch}</td>
                <td style='border: 1px solid black; padding: 5px;'>{DateTime.Now}</td>
            </tr>";
    }
    private async Task SendEmailReceiptAsync(CustomerEntity customerEntity, decimal amount, string transactionType = "Deposit")
    {
        var receiptTableRows = GenerateReceiptTableRow(customerEntity);

        var htmlReceiptTable = $@"
            <h1 style='color: blue;'>Transaction Receipt</h1>
            <p style='font-size: 16px;'>Dear {customerEntity.FullName},</p>
            <p> A {transactionType}  transaction of N{amount}.00 was made on your account.</p>
            <p style='font-size: 14px;'>Your transaction was successful. Below is the receipt of your transaction.</p>
            <p style='font-size: 14px;'>Thank you for banking with us.</p>
            <p style='font-size: 14px;'>Best regards,</p>
            <p style='font-size: 14px;'>Banking Team</p>
            <br>
            <br>
            <table style='border-collapse: collapse;'>
                <thead>
                <tr>
                    <th style='border: 1px solid black; padding: 5px;'>Account Number</th>
                    <th style='border: 1px solid black; padding: 5px;'>Full Name</th>
                    <th style='border: 1px solid black; padding: 5px;'>Balance</th>
                    <th style='border: 1px solid black; padding: 5px;'>Branch</th>
                    <th style='border: 1px solid black; padding: 5px;'>Date</th>
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
        var customerEntity = await _context.CustomerEntity.Where(x => x.AccountNumber == customerWithdraw.CustomerAccountNumber).SingleAsync();
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
        var LedgerEntity = await _context.GLAccounts.Where(x => x.AccountNumber == customerWithdraw.LedgerAccountNumber).SingleAsync();
        if (LedgerEntity is null)
        {
            _logger.LogInformation("Ledger not found");
            return new CustomerResponse
            {
                Message = "Ledger not found",
                Status = false,
                Errors = new List<string> { "Ledger not found" }
            };
        }

        //var LedgerBalance = await _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerWithdraw.LedgerAccountNumber!);
        var customerBalance = await _context.CustomerBalance.Where(x => x.AccountNumber == customerWithdraw.CustomerAccountNumber).SingleAsync();

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
        await PerformWithdralAsync(customerWithdraw, customerEntity, customerBalance, LedgerEntity /*, LedgerBalance*/);
        _logger.LogInformation("Withdrawal successful");
        await SendEmailReceiptAsync(customerEntity, customerWithdraw.Amount, "Withdrawal");
        _logger.LogInformation("Email sent");
        return new CustomerResponse
        {
            Message = "Withdrawal successful",
            Status = true
        };
    }
    private async Task PerformWithdralAsync(PostingDTO customerWithdraw, CustomerEntity customerEntity, CustomerBalance customerBalance, GLAccounts LedgerEntity/*, decimal LedgerBalance=0.0m*/ )
    {
        customerEntity.Balance -= customerWithdraw.Amount;
        LedgerEntity.Balance += customerWithdraw.Amount;
        //var ledger = await _context.GLAccounts.Where(x => x.AccountNumber == customerWithdraw.LedgerAccountNumber).SingleAsync();
        //ledger.Balance = LedgerBalance;
        //_context.GLAccounts.Update(ledger);
        var customerLedgerBalance = _ledgerService.GetMostRecentLedgerEnteryBalanceAsync(customerWithdraw.LedgerAccountNumber!);
        customerBalance.LedgerBalance = customerLedgerBalance.Result  + customerWithdraw.Amount;
        customerBalance.AvailableBalance -= customerWithdraw.Amount;
        customerBalance.WithdrawableBalance -= customerWithdraw.Amount;

        PostingEntity postingEntity = PostingEntityForWithdraw(customerWithdraw, customerEntity, LedgerEntity);
        Transaction transaction = TransactionEntityForWithdraw(customerWithdraw, customerEntity, LedgerEntity);
       
       _context.GLAccounts.Update(LedgerEntity);
        await SaveWithdrawalAsync(customerEntity, customerBalance, postingEntity, transaction);
    }
    private async Task SaveWithdrawalAsync(CustomerEntity customerEntity, CustomerBalance customerBalance, PostingEntity postingEntity, Transaction transaction)
    {
        await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(postingEntity);
        _context.CustomerEntity.Update(customerEntity);
        _context.CustomerBalance.Update(customerBalance);
        await SaveChangesAsync();
        _logger.LogInformation("Withdrawal successful");
    }
    private static Transaction TransactionEntityForWithdraw(PostingDTO customerWithdraw, CustomerEntity customerEntity, GLAccounts LedgerEntity)
    {
        var moneyOut = customerWithdraw.Amount;
        var transactionBalance = customerEntity.Balance;
        return new Transaction
        {
            TransactionType = "Withdrawal",
            TransactionDescription = customerWithdraw.Narration,
            Amount = customerWithdraw.Amount,
            GLAccountId = LedgerEntity.Id,
            CustomerId = customerEntity.Id,
            MoneyOut = moneyOut,
            Balance = transactionBalance
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
        var sender = await GetCustomerByAccountNumberAsync(customerTransfer.SenderAccountNumber);
        if (sender == null)
        {
            _logger.LogInformation("Sender not found");
            return new CustomerResponse
            {
                Message = "Sender not found",
                Status = false,
                Errors = new List<string> { "Sender not found" }
            };
        }

        var receiver = await GetCustomerByAccountNumberAsync(customerTransfer.ReceiverAccountNumber);
        if (receiver == null)
        {
            _logger.LogInformation("Receiver not found");
            return new CustomerResponse
            {
                Message = "Receiver not found",
                Status = false,
                Errors = new List<string> { "Receiver not found" }
            };
        }

        if (!HasSufficientFunds(sender, customerTransfer.Amount))
        {
            _logger.LogInformation("Insufficient funds");
            return new CustomerResponse
            {
                Message = "Insufficient funds",
                Status = false,
                Errors = new List<string> { "Insufficient funds" }
            };
        }

        UpdateSenderBalance(sender, customerTransfer.Amount);
        UpdateReceiverBalance(receiver, customerTransfer.Amount);

        await UpdateSenderBalanceTable(sender, customerTransfer.Amount);
        await UpdateReceiverBalanceTable(receiver, customerTransfer.Amount);

        //var transaction = TransactionEntityForTransfer(customerTransfer, sender, receiver);
        var posting = PostingEntityForTransfer(customerTransfer, sender, receiver);

        //await _context.Transaction.AddAsync(transaction);
        await _context.PostingEntities.AddAsync(posting);
        _context.CustomerEntity.Update(sender);
        _context.CustomerEntity.Update(receiver);

        await SaveChangesAsync();

        _logger.LogInformation("Transfer successful");

        return new CustomerResponse
        {
            Message = "Transfer successful",
            Status = true
        };
    }
    
    private static Transaction TransactionEntityForTransfer(CustomerTransferDTO customerTransfer, CustomerEntity sender, CustomerEntity receiver)
    {
        var moneyOut = customerTransfer.Amount;
        var moneyin = customerTransfer.Amount;
        var transactionBalance = sender.Balance;
        return new TransferTransaction
        {
            TransactionType = "Transfer",
            TransactionDescription = customerTransfer.Narration,
            Amount = customerTransfer.Amount,
            // GLAccountId = sender.Id,
            // GLAccountId = receiver.Id,
            SourceAccountId = sender.Id,
            DestinationAccountId = receiver.Id,

            MoneyOut = moneyOut,
            MoneyIn = moneyin,
            Balance = transactionBalance
        };
    }
    
    private static PostingEntity PostingEntityForTransfer(CustomerTransferDTO customerTransfer, CustomerEntity sender, CustomerEntity receiver)
    {
        return new PostingEntity
        {
            AccountName = sender.FullName,
            AccountNumber = sender.AccountNumber,
            Amount = customerTransfer.Amount,
            TransactionType = "Transfer",
            Narration = customerTransfer.Narration,
            CustomerId = sender.Id.ToString(),
            CustomerName = sender.FullName,
            CustomerAccountNumber = sender.AccountNumber,
            CustomerAccountType = sender.AccountType!.ToString(),
            CustomerBranch = sender.Branch,
            CustomerEmail = sender.Email,
            CustomerPhoneNumber = sender.PhoneNumber,
            CustomerStatus = sender.Status,
            CustomerGender = sender.Gender,
            // ReceiverName = receiver.FullName,
            // ReceiverAccountNumber = receiver.AccountNumber,
            // ReceiverEmail = receiver.Email,
            // ReceiverPhoneNumber = receiver.PhoneNumber
        };
    }

    
   
    private static bool HasSufficientFunds(CustomerEntity customer, decimal amount)
    {
        return customer.Balance >= amount;
    }
    private static void UpdateSenderBalance(CustomerEntity sender, decimal amount)
    {
        sender.Balance -= amount;
    }
    private static void UpdateReceiverBalance(CustomerEntity receiver, decimal amount)
    {
        receiver.Balance += amount;
    }
    private async Task UpdateSenderBalanceTable(CustomerEntity sender, decimal amount)
    {
        var senderBalanceTable = await _context.CustomerBalance.SingleAsync(x => x.AccountNumber == sender.AccountNumber);
        senderBalanceTable.AvailableBalance -= amount;
        senderBalanceTable.WithdrawableBalance -= amount;
        senderBalanceTable.LedgerBalance += amount;
        _context.CustomerBalance.Update(senderBalanceTable);
    }
    private async Task UpdateReceiverBalanceTable(CustomerEntity receiver, decimal amount)
    {
        var receiverBalanceTable = await _context.CustomerBalance.SingleAsync(x => x.AccountNumber == receiver.AccountNumber);
        receiverBalanceTable.AvailableBalance += amount;
        receiverBalanceTable.WithdrawableBalance += amount;
        receiverBalanceTable.LedgerBalance += amount;
        _context.CustomerBalance.Update(receiverBalanceTable);
    }
    private async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<dynamic> GetPostingsAsync(int pageNumber, int pageSize, string? filterValue)
    {
        _logger.LogInformation("Getting all postings");

        var totalPostings = await GetTotalPostingsAsync();
        var totalPostingsByType = await GetTotalPostingsByTypeAsync();
        var postingsTask = await _context.PostingEntities
            .OrderByDescending(x => x.DatePosted)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var postings = postingsTask;

        var filteredPostings = postings
         .Where(x => x.TransactionType != null && x.TransactionType.ToString().Equals(filterValue, StringComparison.OrdinalIgnoreCase))
         .Select(x => new
         {
             x.AccountName,
             x.AccountNumber,
             x.Amount,
             x.TransactionType,
             x.Narration,
             x.CustomerId,
             x.CustomerName,
             x.CustomerAccountNumber,
             x.CustomerAccountType,
             x.CustomerBranch,
             x.CustomerEmail,
             x.CustomerPhoneNumber,
             x.CustomerStatus,
             x.CustomerGender,
             x.CustomerAddress,
             x.CustomerState,
             x.DatePosted
         })
         .ToList();

        _logger.LogInformation($"Filter Value: {filterValue}");
        _logger.LogInformation($"Number of postings: {postings.Count}");
        _logger.LogInformation($"Total customers: {filteredPostings.Count}");

        var result = new
        {

            TotalPostings = totalPostings,
            TotalPostingsByType = totalPostingsByType,
            FilteredPostings = filteredPostings
        };

        return result;
    }
    private async Task<int> GetTotalPostingsAsync()
    {
        return await _context.PostingEntities.CountAsync();
    }
    private async Task<Dictionary<string, int>> GetTotalPostingsByTypeAsync()
    {
        var postings = await _context.PostingEntities.ToListAsync();
        var postingTypes = postings.Select(p => p.TransactionType).Distinct();
        var postingTypeCount = new Dictionary<string, int>();
        foreach (var type in postingTypes)
        {
            if (type != null && !postingTypeCount.ContainsKey(type))
            {
                postingTypeCount.Add(type, postings.Count(p => p.TransactionType == type));
            }
        }
        return postingTypeCount;
    }
    private async Task<CustomerEntity> GetCustomerByAccountNumberAsync(string accountNumber)
    {
        return await _context.CustomerEntity.SingleAsync(x => x.AccountNumber == accountNumber);
    }
}
