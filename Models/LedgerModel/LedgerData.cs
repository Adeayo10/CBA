namespace CBA.Models
{
    public class LedgerData
    {
        public int Id { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountCategory { get; set; }
        public string? AccountDescription { get; set; }
        public string? AccountStatus { get; set; }
        public decimal? Balance { get; set; }

    }
}