using CBA.Models;

namespace CBA.Services;
public interface IPdfService
{
    Task<string> CreateAccountStatementPdfAsync(List<Transaction> transactions, string customerId, string filePath, string startDate, string endDate);
}
