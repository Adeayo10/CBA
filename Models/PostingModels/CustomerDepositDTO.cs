namespace CBA.Models;

public class CustomerDepositDTO
{    
    public Guid Id { get; set; }
    public string? AccountNumber { get; set; }  
    public decimal Amount { get; set; }

}