using CBA.Models;
using Microsoft.AspNetCore.Mvc;
namespace CBA.Services;
public interface ICustomerService
{
    Task<CustomerResponse> CreateCustomerAsync(CustomerDTO customer);
    Task<CustomerResponse> GetCustomerByIdAsync(Guid id);
    Task <CustomerResponse> ValidateCustomerByAccountNumberAsync(string accountNumber);
    Task<dynamic> GetCustomersAsync(int pageNumber, int pageSize, string? filterValue);
    Task<CustomerResponse> UpdateCustomerAsync(CustomerDTO customer);
    Task<CustomerResponse> GetCustomerAccountBalanceAsync(Guid id);
    Task<CustomerResponse> ChangeAccountStatusAsync(Guid id);
    object GetAccountTypes();
    Task<TransactionResponse> GetTransactionsAsync(TransactionDTO transaction);
    Task<FileContentResult> GetAccountStatementPdfAsync(TransactionDTO transaction);

}
