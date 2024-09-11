using IbsaAppTeam1Pra.Models;
using IbsaAppTeam1Pra.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace IbsaAppTeam1Pra.Controllers
{
    public class UserController : Controller
    {
        public static List<User> users = new List<User>();
        public static List<LoginVM> loginUserData = new List<LoginVM>();
        public static List<Transaction> transactions = new List<Transaction>();
        public static List<TransactionHistory> transactionHistories = new List<TransactionHistory>();
        
        public static string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "transactions.txt");
        private const string DEL = ",";

        public static string usersPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "users.txt");

        public static string userDetailsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "userDetails.txt");
        

            public void SaveUsersToFile()
        {
            using (StreamWriter sw = new StreamWriter(usersPath))
            {
                foreach (var user in loginUserData)
                {
                    sw.WriteLine(user.Username + DEL + user.Password);
                }
            }
        }

        public void ReadUsersFromFile()
        {
            if (System.IO.File.Exists(usersPath))
            {
                using (StreamReader sr = new StreamReader(usersPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(DEL);
                        LoginVM loginVM = new LoginVM
                        {
                            Username = parts[0],
                            Password = parts[1]
                        };
                        loginUserData.Add(loginVM);
                    }
                }
            }
        }
            public void SaveTransactionsToFile()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var transaction in transactions)
                {
                    sw.WriteLine(transaction.Sender.FirstName + DEL + transaction.Sender.LastName + DEL + transaction.ReceiverName + DEL + transaction.ReceiverAccountNumber + DEL + transaction.Amount + DEL + transaction.Description + DEL + transaction.Date);
                }
            }
        }
        // create method for reading transactions from the file
        public void ReadTransactionsFromFile()
        {
            if (System.IO.File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(DEL);
                        Transaction transaction = new Transaction
                        {
                            Sender = new User { FirstName = parts[0], LastName = parts[1] },
                            ReceiverName = parts[2],
                            ReceiverAccountNumber = parts[3],
                            Amount = decimal.Parse(parts[4]),
                            Description = parts[5],
                            Date = DateTime.Parse(parts[6])
                        };
                        transactions.Add(transaction);
                    }
                }
            }
        }
        LoginVM bankar = new LoginVM
        {
            Username = "bankar@algebra.hr",
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
            ReadUsersFromFile();
            // check users and if user doesnt exist, add user and redirect to create profile view
            if (!loginUserData.Any(u => u.Username == user.Username))
            {
                users.Add(user);
                SaveUsersToFile();
                HttpContext.Session.SetString("UserLoggedIn", user.Username);
                HttpContext.Session.SetString("ProfileCreated", "false");
                return View("CreateProfile");
            }
            // if user exists, redirect to dashboard view
            HttpContext.Session.SetString("UserLoggedIn", user.Username);
            HttpContext.Session.SetString("ProfileCreated", "true");
            
            return RedirectToAction("Dashboard", user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            
            return RedirectToAction("Login", "User");
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

        public IActionResult BankarDashboard(List<Transaction> transactionsList)
        {
            // call ReadTransactionsFromFile(); and save them to transactions list
            ReadTransactionsFromFile();
            // sort transactions by date
            transactionsList = transactions.OrderByDescending(t => t.Date).ToList();
            return View(transactionsList);
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
            // add transaction to transactions.txt file
            SaveTransactionsToFile();
            transactionHistories.Add(transactionHistory); // Add to history

            return RedirectToAction("FilledDashboard");
        }

        public IActionResult FilledDashboard()
        {
            return View(transactionHistories);
        }
        
    }
}
