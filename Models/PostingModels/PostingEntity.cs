namespace CBA.Models;
public class PostingEntity
{
    public PostingEntity()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionType { get; set; }
    public string? Narration { get; set; }
    public DateTime DatePosted { get; set; } = DateTime.Now;
    public string? Status { get; set; } = "Active";
    public string? Branch { get; set; }
    public string? Teller { get; set; }
    public string? AccountType { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAccountNumber { get; set; }
    public string? CustomerAccountType { get; set; }
    public string? CustomerBranch { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public string? CustomerStatus { get; set; }
    public string? CustomerGender { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerState { get; set; }
    public string? CustomerDateCreated { get; set; }
    public string? CustomerBalance { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CustomerTeller { get; set; }
    public string? CustomerTransactionType { get; set; }
    public string? CustomerNarration { get; set; }
    }