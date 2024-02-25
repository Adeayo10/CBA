namespace CBA.Models;
public class LedgerRequestDTO
{
    public int Id { get; set; }
    public string? AccountName { get; set; }

    public string? AccountCategory { get; set; }

    public string? AccountDescription { get; set; }
}