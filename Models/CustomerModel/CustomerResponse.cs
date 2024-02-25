namespace CBA.Models
{
    public class CustomerResponse
    {
        public string? Message { get; set; }
        public bool Status { get; set; }
        public List<string>? Errors { get; set; }      
        public CustomerEntity? Customer { get; set; } 
        public CustomerBalanceDTO? Data { get; set; }
        public List<Transaction>? Transaction { get; set; }
        
    }
}