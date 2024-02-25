using System.ComponentModel.DataAnnotations.Schema;
namespace CBA.Models;
public class GLAccounts
{
    public int Id { get; set; }
    public string? AccountName { get; set; }   
    public string? AccountCategory { get; set; }
    public string? AccountDescription { get; set; }
    public DateTime? TransactionDate { get; set; } 
    public string? AccountNumber { get; set; }
    public string? AccountStatus { get; set; } = "Active";
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    
    //public ICollection<LedgerTransaction> LedgerTransactions { get; set; }

}

/*public class LedgerTransaction
{
    public Guid Id { get; set; }
    public string? TransactionType { get; set; }
    public string? TransactionDescription { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public Guid GLAccountId { get; set; }
    public GLAccounts GLAccounts { get; set; }
    public Guid CustomerId { get; set; }
    public CustomerEntity CustomerEntity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}*/