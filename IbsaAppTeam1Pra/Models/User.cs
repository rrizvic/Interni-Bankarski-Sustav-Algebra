namespace IbsaAppTeam1Pra.Models
{
    public class User
    {
        public string FirstName { get; set; } = "John";
        public string LastName { get; set; } = "Doe";
        public string Email { get; set; } = "default@email.hr";
        public string Password { get; set; }
        public string Username { get; set; }
        public string Oib { get; set; } = "12345678901";
        public Account AccountOne { get; set; } = null;
        public Account AccountTwo { get; set; } = null;

        public User()
        {
        }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public User(string firstName, string lastName, string email, string password, string username, string oib, Account accountOne)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Username = username;
            Oib = oib;
            AccountOne = accountOne;
        }
        public User(string firstName, string lastName, string email, string password, string username, string oib, Account accountOne, Account accountTwo)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Username = username;
            Oib = oib;
            AccountOne = accountOne;
            AccountTwo = accountTwo;
        }
    }
}
