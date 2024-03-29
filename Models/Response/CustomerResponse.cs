namespace CBA.Models
{
    public class CustomerResponse: Response
    {
        public  new string? Message { get; set; }
        public new bool Status { get; set; }
        public List<string>? Errors { get; set; }      
        public CustomerEntity? Customer { get; set; } 
        public CustomerBalanceDTO? Data { get; set; }
        
    }
}