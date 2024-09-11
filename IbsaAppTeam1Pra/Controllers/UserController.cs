using IbsaAppTeam1Pra.Models;
using IbsaAppTeam1Pra.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace IbsaAppTeam1Pra.Controllers
{
    public class UserController : Controller
    {
        public static List<User> users = new List<User>();
        public static List<Transaction> transactions = new List<Transaction>();
        public static List<TransactionHistory> transactionHistories = new List<TransactionHistory>();
        LoginVM bankar = new LoginVM
        {
            Username = "bankar",
            Password = "bankar123"
        };
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
            if (string.IsNullOrEmpty(loginVM.Username) || !loginVM.Username.Contains("@algebra.hr"))
            {
                ModelState.AddModelError("", "Invalid username. It must contain '@algebra.hr'.");
            }

            if (string.IsNullOrEmpty(loginVM.Password) || loginVM.Password.Length < 8)
            {
                ModelState.AddModelError("", "Invalid password. It must be at least 8 characters long.");
            }

            if (!ModelState.IsValid)
            {
                // If any of the above conditions are met, re-render the view with errors
                return View(loginVM);
            }
            var user = new User
            {
                Username = loginVM.Username,
                Password = loginVM.Password
            };
            if (user.Username == bankar.Username && user.Password == bankar.Password)
            {
                return RedirectToAction("BankarDashboard");
            }
            // check users and if user doesnt exist, add user and redirect to create profile view
            if (!users.Any(u => u.Username == user.Username))
            {
                users.Add(user);
                return View("CreateProfile");
            }
            // if user exists, redirect to dashboard view
            return RedirectToAction("Dashboard", user);
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
                currency = userAccount.Currency == "EUR" ? Account.Currency.EUR : Account.Currency.HRK,
                Balance = 0
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

            TempData["AccountNumber"] = userAccount.AccountNumber;
            return RedirectToAction("AccountOptions");
        }

        public IActionResult AccountOptions()
        {
            string accountNumber = TempData["AccountNumber"] as string;
            User user = new User();
            user.AccountOne = new Account
            {
                AccountNumber = accountNumber,  // Set the account number from TempData
                Balance = 0
            };

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
