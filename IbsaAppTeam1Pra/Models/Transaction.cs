namespace IbsaAppTeam1Pra.Models
{
    public class Transaction
    {
        public User Sender { get; set; } = null!;
        public string ReceiverName { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

    }
}
