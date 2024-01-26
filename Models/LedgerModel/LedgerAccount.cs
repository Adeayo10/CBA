namespace CBA.Models;

public class GLAccounts
{
    public int Id { get; set; }
    public string? AccountName { get; set; }   
    public string? AccountCategory { get; set; }
    public string? AccountDescription { get; set; }
  
    public string? AccountNumber { get; set; }

    public string? AccountStatus { get; set; } = "Active";

}