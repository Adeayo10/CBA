using System.ComponentModel.DataAnnotations.Schema;

namespace CBA.Models;
[Table("Transactions")]
public class Transaction
{
    public Guid Id { get; set; }
    public string? TransactionType { get; set; }
    public string? TransactionDescription { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public int GLAccountId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public decimal MoneyIn { get; set; }
    public decimal MoneyOut { get; set; }
    public decimal Balance { get; set; }
}

partial class TransferTransaction : Transaction
{
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
}