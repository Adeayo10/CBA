using CBA.Models;

namespace CBA.Services;

public interface ILedgerService
{
Task<LedgerResponse> AddGLAccount(LedgerRequestDTO ledgerRequestDTO);
Task<LedgerResponse> GetGlAccount(int pageNumber, int pageSize /*string filter*/);
Task<LedgerResponse> UpdateGLAccount(LedgerRequestDTO ledgerRequestDTO);
Task<decimal> GetMostRecentLedgerEnteryBalance(string accountNumber);
Task<CustomerResponse> LinkUserToGLAccount(UserLedgerDto userLedger);
Task<CustomerResponse> UnLinkUserToGLAccount(UserLedgerDto userLedger);
Task<LedgerResponse> ChangeAccountStatus(int id);

Task<LedgerResponse> ViewLedgerAccountBalance(string accountNumber);
Task<LedgerResponse> GetGLAccountById(int id);
}