namespace CBA.Models;
public class TransactionResponse
{
    public string? Id { get; set; }
    public string? Message { get; set; }
    public bool Status { get; set; }
    public List<Transaction>? Transactions { get; set;}
    public List<string>? Errors { get; set; }

    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
   
}