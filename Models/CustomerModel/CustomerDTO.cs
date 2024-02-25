namespace CBA.Models
{
    public class CustomerDTO
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }       
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? Status { get; set; }
        public CustomerAccountType? AccountType { get; set; }
        public string? State { get; set; }
    }
}