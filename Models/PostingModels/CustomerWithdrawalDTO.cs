namespace CBA.Models;
    public class CustomerWithdrawDTO 
    {
        public Guid Id { get; set; }
        public string? AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
