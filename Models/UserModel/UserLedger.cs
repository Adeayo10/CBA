using System.ComponentModel.DataAnnotations.Schema;

namespace CBA.Models;
public class UserLedger
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string? LedgerId { get; set; }
    public string? UserName { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCategory { get; set; }
    public string? AccountNumber { get; set; }

}
