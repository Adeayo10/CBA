using CBA.Context;
using CBA.Models;
using Microsoft.EntityFrameworkCore;

namespace CBA.Services;
public class LedgerService : ILedgerService
{
    private readonly UserDataContext _context;
    private readonly ILogger<LedgerService> _logger;
    public LedgerService(UserDataContext context, ILogger<LedgerService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<LedgerResponse> AddGLAccountAsync(LedgerRequestDTO ledgerRequestDTO)
    {
        _logger.LogInformation("Creating GLAccount");
        var autoGeneratedAccountNumber = await GenerateAccountNumberAsync(ledgerRequestDTO.AccountCategory!);

        var isGLAccountExist = await IsGLAccountExistAsync(autoGeneratedAccountNumber, ledgerRequestDTO.AccountName!);
        if (isGLAccountExist)
        {
            _logger.LogInformation("Account already exists");
            return new LedgerResponse
            {
                Message = "Account already exists",
                Status = false
            };
        }

        _logger.LogInformation("Creating GLAccount entity");
        var glAccount = await CreateGLAccountEntity(ledgerRequestDTO, autoGeneratedAccountNumber);
        await _context.GLAccounts.AddAsync(glAccount);

        await LinkUserToGLAccountAsync(new UserLedgerDto
        {
            Userid = ledgerRequestDTO.UserId,
            GLAccountid = glAccount.Id
        });


        await _context.SaveChangesAsync();
        _logger.LogInformation("Account created successfully");

        return new LedgerResponse
        {
            Message = "Account created successfully",
            Status = true,
            LedgerId = glAccount.Id            
        };
    }
    private static Task<GLAccounts> CreateGLAccountEntity(LedgerRequestDTO ledgerRequestDTO, string autoGeneratedAccountNumber)
    {
        var glAccount = new GLAccounts
        {
            AccountName = ledgerRequestDTO.AccountName,
            AccountNumber = autoGeneratedAccountNumber,
            AccountCategory = ledgerRequestDTO.AccountCategory,
            AccountDescription = ledgerRequestDTO.AccountDescription,
            Balance = 1000,
            AccountStatus = "Active",
            TransactionDate = DateTime.Now
        };

        return Task.FromResult(glAccount);
    }
    public async Task<LedgerResponse> GetGlAccountsAsync(int pageNumber, int pageSize /*string filterValue*/)
    {
        _logger.LogInformation("Fetching GLAccount");
        var glAccount = await _context.GLAccounts/*.Where(x => x.AccountName.Contains(filterValue) || x.AccountNumber.Contains(filterValue))*/
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
        if (glAccount.Count == 0)
        {
            _logger.LogInformation("No account found");
            return new LedgerResponse
            {
                Message = "No account found",
                Status = false
            };
        }
        var totalRowCount = await GetTotalCountOfGLAccountsAsync();
        _logger.LogInformation("TOTAL ROW COUNT: " + totalRowCount);    
        _logger.LogInformation("Account found");
        var mappedData = glAccount.Select(account => new GLAccounts
        {
            Id= account.Id,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            AccountCategory = account.AccountCategory,
            AccountDescription = account.AccountDescription,
            AccountStatus = account.AccountStatus
        }).ToList();
        _logger.LogInformation("Account found", mappedData);
        
        return new LedgerResponse
        {
            Message = "Account found",
            Status = true,
            DataList = mappedData,
            TotalRowCount = totalRowCount
        };
        
    }
    private async Task<int> GetTotalCountOfGLAccountsAsync()
    {
        return await _context.GLAccounts.CountAsync();
    }
    // private static LedgerData? MapCollectionOfLedgerData(List<GLAccounts> glAccount)
    // {
    //     var mappedData = glAccount.Select(account => new LedgerData
    //     {
    //         AccountName = account.AccountName,
    //         AccountNumber = account.AccountNumber,
    //         AccountCategory = account.AccountCategory,
    //         AccountDescription = account.AccountDescription,
    //         AccountStatus = account.AccountStatus
    //     }).ToList();

    //     return mappedData ;
    // }
    public async Task<LedgerResponse> GetGLAccountByIdAsync(int id)
    {
        _logger.LogInformation("Fetching GLAccount");
        var glAccount = await _context.GLAccounts.SingleOrDefaultAsync(x => x.Id == id);
        if (glAccount is null)
        {
            _logger.LogInformation("Account not found");
            return new LedgerResponse
            {
                Message = "Account not found",
                Status = false
            };
    
        }

        LedgerData mappedData = MapLedgerData(glAccount);
        
        _logger.LogInformation("Account found");
        return new LedgerResponse
        {
            Message = "Account found",
            Status = true,
            Data = mappedData
        };
    
    }
    private static LedgerData MapLedgerData(GLAccounts glAccount)
    {
        return new LedgerData
        {
            Id = glAccount.Id,
            AccountName = glAccount.AccountName,
            AccountNumber = glAccount.AccountNumber,
            AccountCategory = glAccount.AccountCategory,
            AccountDescription = glAccount.AccountDescription,
            AccountStatus = glAccount.AccountStatus
        };
    }
    public async Task<LedgerResponse> UpdateGLAccountAsync(LedgerRequestDTO ledgerRequestDTO)
    {
        _logger.LogInformation("Updating GLAccount");
        var glAccount = await _context.GLAccounts.FirstOrDefaultAsync(x => x.Id == ledgerRequestDTO.Id);
        if (glAccount is null)
        {
            _logger.LogInformation("Account does not exist");
            return new LedgerResponse
            {
                Message = "Account does not exist",
                Status = false
            };
        }

        _logger.LogInformation("Updating GLAccount details");

        UpdateAccountDetailsAsync(glAccount, ledgerRequestDTO);

        _logger.LogInformation("Account updated successfully");


        return   new LedgerResponse
        {
            Message = "Account updated successfully",
            Status = true
        };
    }
    public object GetLedgerAccountByCategory()
    {
        var LedgerCategory = Enum.GetValues(typeof(LedgerCategory)).Cast<LedgerCategory>().ToList();
        var mappedCategory = LedgerCategory.Select(category => new
        {
            Id = (int)category,
            Category = category.ToString()
        });
        return mappedCategory;
    }
    private async void UpdateAccountDetailsAsync(GLAccounts glAccount, LedgerRequestDTO ledgerRequestDTO)
    {
        glAccount.AccountName = ledgerRequestDTO.AccountName;
        glAccount.AccountDescription = ledgerRequestDTO.AccountDescription;
        glAccount.AccountCategory = ledgerRequestDTO.AccountCategory;
        await _context.SaveChangesAsync();
    }
    private async Task<string> GenerateAccountNumberAsync(string category)
    {
        var totalRowCount = await _context.GLAccounts.CountAsync(x => x.AccountCategory == category);

        var accountNumber = $"{GetCategoryCode(category)}{GetFormattedRowCount(totalRowCount + 1)}";

        return accountNumber;
    }
    private static string GetCategoryCode(string category)
    {
        return category switch
        {
            "Asset" => "1",
            "Liability" => "2",
            "Capital" => "3",
            "Income" => "4",
            "Expense" => "5",
            _ => "0",
        };
    }
    private static string GetFormattedRowCount(int rowCount)
    {
        return rowCount.ToString().PadLeft(4, '0');
    }
    private async Task<bool>  IsGLAccountExistAsync(string accountNumber, string accountName)
    {
        var isGLAccountExist = await _context.GLAccounts.AnyAsync(x => x.AccountNumber == accountNumber || x.AccountName == accountName.ToLower());
        return isGLAccountExist;
    }
    public async Task<decimal> GetMostRecentLedgerEnteryBalanceAsync(string accountNumber)
    {
        var LedgerBalance = await _context.GLAccounts.Where(x => x.AccountNumber == accountNumber).OrderByDescending(x => x.TransactionDate).Select(x => x.Balance).SingleAsync();
        return LedgerBalance;
    }
    public async Task<LedgerResponse> ChangeAccountStatusAsync(int id)
    {
        var glAccount = _context.GLAccounts.Find(id);
        if (glAccount == null)
        {
            _logger.LogInformation("Account not found");
            return new LedgerResponse
            {
                Message = "Account not found",
                Status = false
            };
        }
        glAccount.AccountStatus = glAccount.AccountStatus == "Active" ? "Inactive" : "Active";
        await _context.SaveChangesAsync();
        return new LedgerResponse
        {
            Message = "Account status changed successfully",
            Status = true
        };
    }
    public async Task<LedgerResponse> LinkUserToGLAccountAsync(UserLedgerDto userLedger)
    {
        var user = await _context.Users.FindAsync(userLedger.Userid);
        if (user is null)
        {
            _logger.LogInformation("User not found");
            return new LedgerResponse
            {
                Message = "User not found",
                Status = false
            };
            
        }

        var glAccount = await _context.GLAccounts.FindAsync(userLedger.GLAccountid);
        if (glAccount is null)
        {
            _logger.LogInformation("GLAccount not found");
            return new LedgerResponse
            {
                Message = "GLAccount not found",
                Status = false
            };
        }
        
       
        var isUserLedgerExist = await _context.UserLedger.AnyAsync(x => x.UserId == user.Id || x.LedgerId == glAccount.Id.ToString());
        if (isUserLedgerExist)
        {
            _logger.LogInformation("UserLedger already exists");
            return new LedgerResponse
            {
                Message = "UserLedger already exists",
                Status = false
            };
        }
        _logger.LogInformation("Linking user to GLAccount");
        var userLedgerEntity = CreateUserLedgerEntity(user, glAccount);

        await _context.UserLedger.AddAsync(userLedgerEntity);
        await _context.SaveChangesAsync();

        return new LedgerResponse
        {
            Message = "Linking successful",
            Status = true
        };
    }
    private static UserLedger CreateUserLedgerEntity( ApplicationUser user, GLAccounts glAccount)
    {
        return new UserLedger
        {
            UserId = user.Id,
            LedgerId = glAccount.Id.ToString(),
            UserName = user.UserName,
            AccountName = glAccount.AccountName,
            AccountCategory = glAccount.AccountCategory,
            AccountNumber = glAccount.AccountNumber
        };
    }
    public async Task<LedgerResponse> UnLinkUserToGLAccountAsync(UserLedgerid userLedgerid)
    {
        var userLedger = await _context.UserLedger.SingleOrDefaultAsync(x => x.Id == userLedgerid.Userid);
        if (userLedger is null)
        {
            _logger.LogInformation("UserLedger not found");
            return new LedgerResponse
            {
                Message = "UserLedger not found",
                Status = false
            };
        }

        _context.UserLedger.Remove(userLedger);
        await _context.SaveChangesAsync();

        _logger.LogInformation("UnLinking successful");
        return new LedgerResponse
        {
            Message = "UnLinking successful",
            Status = true
        }; 
    }
    private static T CreateErrorResponse<T>(string errorMessage) where T : Response, new()
    {
        return new T
        {
            Message = errorMessage,
            Status = false,           
        };
    } 
    private static T CreateSuccessResponse<T>(string successMessage, LedgerData? ledgerData = null, int? rowCount=null) where T : Response, new()
    {
        return new T
        {
            Message = successMessage,
            Status = true,
            Data = ledgerData,
            TotalRowCount = rowCount
        };
    }
    public async Task<LedgerResponse> ViewLedgerAccountBalanceAsync(string accountNumber)
    {
        var glAccount = await _context.GLAccounts.SingleAsync(x => x.AccountNumber == accountNumber);
        if (glAccount is null)
        {
            _logger.LogInformation("Account not found");
            return new LedgerResponse
            {
                Message = "Account not found",
                Status = false
            };
        }
        var mappedData = new LedgerData
        {
            AccountName = glAccount.AccountName,
            AccountNumber = glAccount.AccountNumber,
            AccountCategory = glAccount.AccountCategory,
            AccountDescription = glAccount.AccountDescription,
            AccountStatus = glAccount.AccountStatus,
            Balance = glAccount.Balance
        };
        _logger.LogInformation("Account found");
        return new LedgerResponse
        {
            Message = "Account found",
            Status = true,
            Data = mappedData
        };
    }   
    public async Task<bool> ValidateLinkedUser(ValidateLinkedUserDTO validateLinkedUserDTO)
    {
        var userLedger = await _context.UserLedger.SingleOrDefaultAsync(x => x.UserId == validateLinkedUserDTO.UserId && x.LedgerId == validateLinkedUserDTO.LedgerId);
        if (userLedger is null)
        {
            return false;
        }
        return true;
    }

    
    /*public async Task<decimal> CalculateLedgerAccountBalance()
    {
        var glAccount = await _context.GLAccounts.ToListAsync();
        glAccount.filter(x => x.AccountCategory == "Assets");
        decimal totalBalance = 0;
        foreach (var account in glAccount)
        {
            totalBalance += account.Balance;
        }
        return totalBalance;
    }*/

}