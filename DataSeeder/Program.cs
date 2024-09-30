using DatabaseLib;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string bankWebServiceDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "BankWebService"); // '..' to cd out of current directory
            string dbPath = Path.Combine(bankWebServiceDir, "BankDB.db"); // generates an absolute path for database location


            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(bankWebServiceDir) // should be a path to BankWebService (different project, same solution)
                .AddJsonFile("appsettings.json") // will look at base path to determine where the appsettings.json is for connection strings
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"DbPath", dbPath} // needs to be added such that the configuration knows where database actually is
                })
                .Build();

            var dataAccess = new SqliteDataAccess(configuration); // new data access initialised with the built config
            Console.WriteLine($"Configuration loaded from {bankWebServiceDir}. Ready to seed data.");

            // clear db for new data
            dataAccess.ClearDatabase();
            // generate + add profiles
            SeedProfiles(dataAccess);
            Console.WriteLine("Profiles seeded");
            // generate + add accounts for the profiles
            SeedAccounts(dataAccess);
            Console.WriteLine("Accounts seeded");
            // generate and add deposits/withdrawals for the accounts
            SeedTransactions(dataAccess);
            Console.WriteLine("Transactions seeded");

            // here to ensure the console does not close automatically
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }

        static void SeedProfiles(SqliteDataAccess dataAccess)
        {
            var profiles = ProfileGenerator.Generate(10);
            foreach (var profile in profiles)
            {
                try
                {
                    dataAccess.AddProfile(profile);
                    Console.WriteLine($"Added profile: {profile.Username}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding profile {profile.Username}: {ex.Message}");
                }
            }
        }

        static void SeedAccounts(SqliteDataAccess dataAccess)
        {
            var profiles = GetProfiles(dataAccess);
            if (profiles != null)
            {
                var accounts = AccountGenerator.Generate(20, profiles);
                foreach (var account in accounts)
                {
                    try
                    {
                        dataAccess.AddAccount(account);
                        Console.WriteLine($"Current account balance: {account.balance}"); // used to show account balance before transaction
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error adding account {account.account_number}: {ex.Message}");
                    }
                }
                
            }
            else
            {
                Console.WriteLine($"Error adding accounts: could not retrieve profiles");
            }
            
        }

        static void SeedTransactions(SqliteDataAccess dataAccess) 
        {
            var accounts = GetAccounts(dataAccess);
            if (accounts != null)
            {
                int numTransactions = 50;
                var random = new Random();

                int numWithdrawals = random.Next(1, numTransactions);
                int numDeposits = numTransactions - numWithdrawals;

                Console.WriteLine($"Seeding {numWithdrawals} withdrawals...");
                SeedWithdrawals(dataAccess, accounts, numWithdrawals);

                Console.WriteLine($"Seeding {numDeposits} deposits...");
                SeedDeposits(dataAccess, accounts, numDeposits);
            }
            else
            {
                Console.WriteLine($"Error adding transactions: could not retrieve accounts");
            }

        }

        // The reason this method exists is to separate try-catch statements and provide more specific error messages for accounts
        // accounts table has a foreign key for profiles so account holdernames should match profile usernames
        static List<Profile> GetProfiles(SqliteDataAccess dataAccess)
        {
            List<Profile> profiles = null;
            try
            {
                profiles = dataAccess.AllProfiles();
                Console.WriteLine("Retrieved profiles successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving profiles: {ex.Message}");
            }
            return profiles;

        }

        static List<Account> GetAccounts(SqliteDataAccess dataAccess)
        {
            List<Account> accounts = null;
            try
            {
                accounts = dataAccess.AllAccounts();
                Console.WriteLine("Retrieved accounts successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving accounts: {ex.Message}");
            }
            return accounts;

        }

        static void SeedWithdrawals(SqliteDataAccess dataAccess, List<Account> accounts, int numWithdrawals)
        {
            var transactions = TransactionGenerator.GenerateWithdrawals(accounts, numWithdrawals);

            foreach (Transaction transaction in transactions)
            {
                try 
                {
                    dataAccess.Withdraw(transaction.account_number, transaction.amount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error withdrawing from account {transaction.account_number}: {ex.Message}");
                }
                
            }
        }

        static void SeedDeposits(SqliteDataAccess dataAccess, List<Account> accounts, int numDeposits)
        {
            var transactions = TransactionGenerator.GenerateDeposits(accounts, numDeposits);

            foreach (Transaction transaction in transactions)
            {
                try
                {
                    dataAccess.Deposit(transaction.account_number, transaction.amount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding deposit to account {transaction.account_number}: {ex.Message}");
                }

            }
        }
    }
}
