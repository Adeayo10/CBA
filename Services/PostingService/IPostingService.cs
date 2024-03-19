
using CBA.Models;
namespace CBA.Services;
public interface IPostingService
{
    Task<CustomerResponse> DepositAsync(PostingDTO customerDeposit);
    Task<CustomerResponse> WithdrawAsync(PostingDTO customerWithdraw);
    Task<CustomerResponse> TransferAsync(CustomerTransferDTO customerTransfer);
}