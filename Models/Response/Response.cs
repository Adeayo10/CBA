namespace CBA.Models;
public class Response
{
    public string? Message { get; set; }
    public bool Status { get; set; }
    public LedgerData? Data { get; set; }
}