using CBA.Models;

namespace CBA.Services;

public interface ILedgerService
{
Task<LedgerResponse> AddGLAccountAsync(LedgerRequestDTO ledgerRequestDTO);
Task<LedgerResponse> GetGlAccountsAsync(int pageNumber, int pageSize /*string filter*/);
Task<LedgerResponse> UpdateGLAccountAsync(LedgerRequestDTO ledgerRequestDTO);
Task<decimal> GetMostRecentLedgerEnteryBalanceAsync(string accountNumber);
Task<CustomerResponse> LinkUserToGLAccountAsync(UserLedgerDto userLedger);
Task<CustomerResponse> UnLinkUserToGLAccountAsync(UserLedgerid userLedger);
Task<LedgerResponse> ChangeAccountStatusAsync(int id);

Task<LedgerResponse> ViewLedgerAccountBalanceAsync(string accountNumber);
Task<LedgerResponse> GetGLAccountByIdAsync(int id);
object GetLedgerAccountByCategory();

}