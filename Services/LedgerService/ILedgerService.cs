namespace CBA.Services;

public interface ILedgerService
{
Task<string> GenerateAccountNumber(string category);
Task<bool> IsGLAccountExist(string accountNumber, string accountName);
}