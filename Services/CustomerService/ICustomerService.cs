using CBA.Models;
namespace CBA.Services;
public interface ICustomerService
{
    Task<CustomerResponse> CreateCustomer(CustomerDTO customer);
    Task<CustomerResponse> GetCustomerById(Guid id);
    Task <CustomerResponse> ValidateCustomerByAccountNumber(string accountNumber);
    Task<IEnumerable<CustomerEntity>> GetCustomers(int pageNumber, int pageSize, string filterValue);
    Task<CustomerResponse> UpdateCustomerDetails(CustomerDTO customer);
    Task<CustomerResponse> GetCustomerAccountBalance(Guid id);
    Task<CustomerResponse> ChangeAccountStatus(Guid id);
    object GetAccountTypes();
    Task<CustomerResponse> GetTransactions(TransactionDTO transaction);

}
