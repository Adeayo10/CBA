using CBA.Models;

namespace CBA.Services;

public interface ILedgerService
{
Task<LedgerResponse> AddGLAccount(LedgerRequestDTO ledgerRequestDTO);
Task<LedgerResponse> GetGlAccount();
Task<LedgerResponse> UpdateGLAccount(LedgerRequestDTO ledgerRequestDTO);
Task<decimal> GetMostRecentLedgerEnteryBalance();
Task<CustomerResponse> LinkUserToGLAccount(UserLedgerDto userLedger);
Task<CustomerResponse> UnLinkUserToGLAccount(UserLedgerDto userLedger);
Task<LedgerResponse> ChangeAccountStatus(int id);

Task<LedgerResponse> ViewLedgerAccountBalance(string accountNumber);
Task<LedgerResponse> GetGLAccountById(int id);
}