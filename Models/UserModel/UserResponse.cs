
using CBA.Models.AuthModel;

namespace CBA.Models;

    public class UserResponse
    {
        public List<string>? Errors {get; set;}

        public bool Success {get; set;}
        public List<ApplicationUser>? User {get; set;}
        public List<BankBranch>? UserBranch {get; set;}
        
    }

  


