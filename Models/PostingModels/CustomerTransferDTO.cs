namespace CBA.Models;
    public class CustomerTransferDTO
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public string? Narration { get; set; }
        public string? SenderAccountNumber { get; set; }
        public string? ReceiverAccountNumber { get; set; }
    }
