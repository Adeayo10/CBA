using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CBA.Models
{
    public class CustomerBalance
    {
        public int Id { get; set; }
        public Guid? CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        
        public decimal LedgerBalance { get; set; }

         public decimal AvailableBalance { get; set; }

        public decimal WithdrawableBalance { get; set; }

        [ForeignKey ("CustomerId")] 
        public CustomerEntity? CustomerEntity { get; set; }
            
    }
   

    public class CustomerBalanceDTO
    {
        public string? CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal WithdrawableBalance { get; set; }
    }
}