using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CBA.Models;
 public class BranchUser
    {
        public BranchUser()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        
        public int BranchId { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(BranchId))]
        public BankBranch Branch { get; set; }
        
    }