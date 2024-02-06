namespace CBA.Models;

public class LedgerResponse
{
    public string? Message { get; set; }
    public bool Status { get; set; }
    public LedgerData? Data { get; set; }

}