namespace IbsaAppTeam1Pra.Models
{
    public class Account
    {
        public enum Currency
        {
            HRK,
            EUR
        }
        public string AccountNumber { get; set; }
        public Currency currency { get; set; }
        public decimal Balance { get; set; }
    }
}