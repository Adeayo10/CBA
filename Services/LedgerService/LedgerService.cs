using CBA.Context;
using Microsoft.EntityFrameworkCore;

namespace CBA.Services;
public class LedgerService : ILedgerService
{
    private readonly UserDataContext _context;
    public LedgerService(UserDataContext context)
    {
        _context = context;
    }
    public async Task<string> GenerateAccountNumber(string category)
    {
        var totalRowCount = await _context.GLAccounts.CountAsync(x => x.AccountCategory == category);
        category = category switch
        {
            "Asset" => "1",
            "Liability" => "2",
            "Capital" => "3",
            "Income" => "4",
            "Expense" => "5",
            _ => "0",
        };
        var accountNumber = string.Format("{0}{1}", $"{category}", $"{totalRowCount + 1}".PadLeft(4, '0'));

        return accountNumber;
    }

    public async Task<bool> IsGLAccountExist(string accountNumber, string accountName)
    {
        var isGLAccountExist = await _context.GLAccounts.AnyAsync(x => x.AccountNumber == accountNumber.ToLower() || x.AccountName == accountName);
        return isGLAccountExist;
    }

}