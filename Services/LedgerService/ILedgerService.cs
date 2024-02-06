using CBA.Models;

namespace CBA.Services;

public interface ILedgerService
{
Task<LedgerResponse> AddGLAccount(LedgerRequestDTO ledgerRequestDTO);
Task<LedgerResponse> GetGlAccount();
Task<LedgerResponse> UpdateGLAccount(LedgerRequestDTO ledgerRequestDTO);
}