using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBA.Models;

public class BankBranch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public string? UserId { get; set; } // Linked to the AspNet Identity User Id    
    public required string Name { get; set; }
    public required string Region { get; set; }
    public required string Code { get; set; }
    public required string Description { get; set; }

}

