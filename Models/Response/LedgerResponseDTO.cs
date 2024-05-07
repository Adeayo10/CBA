namespace CBA.Models;

public class LedgerResponse: Response
{
    public  new string? Message { get; set; }
    public new bool Status { get; set; }
    public LedgerData? Data { get; set; }
    public List<GLAccounts>? DataList { get; set; }
    public int LedgerId { get; set; }

}