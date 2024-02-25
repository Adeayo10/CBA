namespace CBA.Models
{
    public class CustomerEntity
    {
        public CustomerEntity()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public CustomerAccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public string? Status { get; set; } = "Active";
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string? State { get; set; }
    }
 
}