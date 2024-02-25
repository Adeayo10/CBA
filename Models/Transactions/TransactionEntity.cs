namespace CBA.Models;
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
    }