
using CBA.Models.AuthModel;

namespace CBA.Models;

    public class UserResponse
    {
        public List<string>? Errors {get; set;}

        public bool Success {get; set;}
        public List<ApplicationUser>? Users {get; set;}
        public List<BankBranch>? UserBranch {get; set;}
        public int? TotalUsers {get; set;}
        
    }

  


