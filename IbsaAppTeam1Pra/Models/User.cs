﻿namespace IbsaAppTeam1Pra.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Oib { get; set; }
        public Account AccountOne { get; set; }
        public Account AccountTwo { get; set; }

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
