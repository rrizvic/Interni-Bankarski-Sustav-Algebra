using IbsaAppTeam1Pra.Models;
using IbsaAppTeam1Pra.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Transactions;
using Transaction = IbsaAppTeam1Pra.Models.Transaction;

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

            var existingUser = loginUserData.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (existingUser == null)
            {
                loginUserData.Add(loginVM);
                SaveUsersToFile();
                HttpContext.Session.SetString("UserLoggedIn", user.Username);
                HttpContext.Session.SetString("ProfileCreated", "false");
                var profileCreated = users.Any(u => u.Username == user.Username);

                if (!profileCreated)
                {
                    return View("CreateProfile");
                }
            }

            HttpContext.Session.SetString("UserLoggedIn", user.Username);
            HttpContext.Session.SetString("ProfileCreated", "true");
            
            return RedirectToAction("Dashboard", user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            
            return RedirectToAction("Login", "User");
        }

        public IActionResult CreateProfile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProfile(UserAccountVM userAccount)
        {
            var loggedInUsername = HttpContext.Session.GetString("UserLoggedIn");

            var existingUser = loginUserData.FirstOrDefault(u => u.Username == loggedInUsername);

            if (existingUser == null)
            {
                return RedirectToAction("Login");
            }

            var account = new Account
            {
                AccountNumber = userAccount.AccountNumber,
                currency = userAccount.Currency == "EUR" ? Account.Currency.EUR : Account.Currency.HRK,
                Balance = 0
            };
            
            var user = new User
            {
                Username = loggedInUsername,
                Password = existingUser.Password,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                Email = userAccount.Email,
                Oib = userAccount.Oib,
                AccountOne = account
            };
            users.Add(user);
            SaveUserProfileToFile();

            HttpContext.Session.SetString("ProfileCreated", "true");

            TempData["AccountNumber"] = userAccount.AccountNumber;
            return RedirectToAction("AccountOptions");
        }

        public void SaveUserProfileToFile()
        {
            using (StreamWriter sw = new StreamWriter(usersPath))
            {
                foreach (var user in users)
                {
                    sw.WriteLine($"{user.Username}{DEL}{user.Password}{DEL}{user.FirstName}{DEL}{user.LastName}{DEL}{user.Email}{DEL}{user.Oib}{DEL}{user.AccountOne.AccountNumber}{DEL}{user.AccountOne.currency}{DEL}{user.AccountOne.Balance}");
                }
            }
        }

        public void ReadUserProfilesFromFile()
        {
            if (System.IO.File.Exists(usersPath))
            {
                using (StreamReader sr = new StreamReader(usersPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(DEL);
                        var user = new User
                        {
                            Username = parts[0],
                            Password = parts[1],
                            FirstName = parts[2],
                            LastName = parts[3],
                            Email = parts[4],
                            Oib = parts[5],
                            AccountOne = new Account
                            {
                                AccountNumber = parts[6],
                                currency = parts[7] == "EUR" ? Account.Currency.EUR : Account.Currency.HRK,
                                Balance = decimal.Parse(parts[8])
                            }
                        };
                        users.Add(user);
                    }
                }
            }
        }

        public IActionResult AccountOptions()
        {
            //string accountNumber = TempData["AccountNumber"] as string;
            //User user = new User();
            //user.AccountOne = new Account
            //{
            //    AccountNumber = accountNumber,
            //    Balance = 0
            //};

            //return View(user);
            string loggedInUsername = HttpContext.Session.GetString("UserLoggedIn");

            // Find the user in the list of users
            var user = users.FirstOrDefault(u => u.Username == loggedInUsername);

            if (user == null)
            {
                // Handle case where user is not found (e.g., session expired)
                return RedirectToAction("Login");
            }

            // Pass the user object to the view to display account info
            return View(user);

        }
        [HttpPost]
        public IActionResult AccountOptions(User user)
        {
            //var balance = user.AccountOne.Balance;
            //if (Request.Form.ContainsKey("Uplati"))
            //{
            //    var account = user.AccountOne;
            //    account.Balance += balance;

            //}
            //else if (Request.Form.ContainsKey("createAccount"))
            //{
            //    // create second account for this user
            //    var account = new Account
            //    {
            //        AccountNumber = user.AccountOne.ToString(),
            //        currency = Account.Currency.HRK
            //    };
            //    user.AccountTwo = account;
            //}

            //return RedirectToAction("Dashboard", user);
            var loggedInUsername = HttpContext.Session.GetString("UserLoggedIn");
            var existingUser = users.FirstOrDefault(u => u.Username == loggedInUsername);

            if (existingUser == null)
            {
                // Handle case where user is not found
                return RedirectToAction("Login");
            }

            if (Request.Form.ContainsKey("Uplati"))
            {
                // Handle deposit to the account
                existingUser.AccountOne.Balance += user.AccountOne.Balance; // Add deposit to the existing balance
            }
            else if (Request.Form.ContainsKey("createAccount"))
            {
                // Handle creating a second account for the user
                if (existingUser.AccountTwo == null)
                {
                    existingUser.AccountTwo = new Account
                    {
                        AccountNumber = Guid.NewGuid().ToString(), // Generate new account number
                        currency = Account.Currency.HRK,
                        Balance = 0
                    };
                }
            }

            // Save the updated user details to the file
            SaveUserProfileToFile();

            return RedirectToAction("Dashboard", existingUser);

        }

        public IActionResult BankarDashboard(List<Transaction> transactionsList)
        {
            ReadTransactionsFromFile();
            
            transactionsList = transactions.OrderByDescending(t => t.Date).ToList();
            return View(transactionsList);
        }

        public IActionResult BankarPayment(User user)
        {
            Models.Transaction transaction = new();
            transaction.Sender = user;
            return View(transaction);
        }
        [HttpPost]
        public IActionResult BankarPayment(Models.Transaction transaction)
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
            transactionHistories.Add(transactionHistory);
            transactions.Add(transaction);

            return RedirectToAction("BankarDashboard");
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
            transactionHistories.Add(transactionHistory);

            return RedirectToAction("FilledDashboard");
        }

        public IActionResult FilledDashboard()
        {
            return View(transactionHistories);
        }
        
    }
}
