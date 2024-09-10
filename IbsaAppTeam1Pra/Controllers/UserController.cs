using IbsaAppTeam1Pra.Models;
using IbsaAppTeam1Pra.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IbsaAppTeam1Pra.Controllers
{
    public class UserController : Controller
    {
        public static List<User> users = new List<User>();
        public static List<Transaction> transactions = new List<Transaction>();
        public static List<TransactionHistory> transactionHistories = new List<TransactionHistory>();
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVM)
        {
           
            var user = new User
            {
                Username = loginVM.Username,
                Password = loginVM.Password
            };
            users.Add(user);
            return View("CreateProfile");
        }

        // create profile view where user enters his data and creates account

        public IActionResult CreateProfile()
        {
            return View();
        }

        [HttpPost]
        // CreateProfile action which receives the data from the form and creates the user account
        public IActionResult CreateProfile(UserAccountVM userAccount)
        {
            var account = new Account
            {
                AccountNumber = userAccount.AccountNumber,
                currency = userAccount.Currency == "EUR" ? Account.Currency.EUR : Account.Currency.HRK
            };
            // new user object with data from the form

            var user = new User
            {
                Username = userAccount.Username,
                Password = userAccount.Password,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                Email = userAccount.Email,
                Oib = userAccount.Oib,
                AccountOne = account
            };

            users.Add(user);
            return View("AccountOptions");
        }

        public IActionResult AccountOptions()
        {
            User user = users.Last();
            return View(user);
            
        }
        [HttpPost]
        public IActionResult AccountOptions(User user)
        {
            // check form and see if user clicked button with name Uplati or button with name createAccount
            var balance = user.AccountOne.Balance;
            if (Request.Form.ContainsKey("Uplati"))
            {
                // if user clicked Uplati button, add balance to the user account
                var account = user.AccountOne;
                account.Balance += balance;

            }
            else if (Request.Form.ContainsKey("createAccount"))
            {
                // create second account for this user
                var account = new Account
                {
                    AccountNumber = user.AccountOne.ToString(),
                    currency = Account.Currency.HRK
                };
                user.AccountTwo = account;
            }

            return RedirectToAction("Dashboard", user);

        }

        public IActionResult Dashboard(User user)
        {
            Transaction transaction = new Transaction();
            transaction.Sender = user;
            return View(transaction);

            
        }

        [HttpPost]
        public IActionResult Dashboard(Transaction transaction)
        {
            var sender = transaction.Sender;
            
            var trans = new Transaction
            {
                Sender = sender,
                ReceiverName = transaction.ReceiverName,
                ReceiverAccountNumber = transaction.ReceiverAccountNumber,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = DateTime.Now
            };
            // Add transaction to the list of transactions
            transactions.Add(trans);
            var transactionHistory = new TransactionHistory
            {
                SenderName = sender.FirstName + " " + sender.LastName,
                ReceiverName = transaction.ReceiverName,
                ReceiverAccountNumber = transaction.ReceiverAccountNumber,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = trans.Date
            };

            transactionHistories.Add(transactionHistory); // Add to history

            return RedirectToAction("FilledDashboard");
        }

        public IActionResult FilledDashboard()
        {
            return View(transactionHistories);
        }
        
    }
}
