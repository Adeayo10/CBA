
using CBA.Models;
namespace CBA.Services;
public interface IPostingService
{
    Task<CustomerResponse> Deposit(PostingDTO customerDeposit);
    Task<CustomerResponse> Withdraw(PostingDTO customerWithdraw);
    Task<CustomerResponse> Transfer(CustomerTransferDTO customerTransfer);
}