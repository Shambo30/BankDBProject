using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Principal;

namespace DatabaseLib
{
    public class SqliteDataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly string _dbPath;

        public SqliteDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbPath = _configuration["DbPath"]; // needed to add this to locate the db source in the data seeding script

        }

        public List<Profile> AllProfiles() 
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    return cnn.Query<Profile>("SELECT * FROM profiles").ToList();
                }
               
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating profile: {ex.Message}");
                throw;
            }
        }

        public List<Account> AllAccounts()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    return cnn.Query<Account>("SELECT * FROM accounts").ToList();
                }

            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating profile: {ex.Message}");
                throw;
            }
        }

        public List<Transaction> AllTransactions() 
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    return cnn.Query<Transaction>("SELECT * FROM transactions").ToList();
                }

            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating profile: {ex.Message}");
                throw;
            }
        }


        public void AddProfile(Profile profile)
        {
            try
            {
                if (RetrieveProfileByUsername(profile.Username) == null)
                {
                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        cnn.Execute("INSERT INTO profiles (name, email, address, phone, picture, password, username) VALUES (@Name, @Email, @Address, @Phone, @Picture, @Password, @Username)", profile);
                    }
                }
                else 
                {
                    throw new InvalidOperationException("Username already exists");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding profile: {ex.Message}");
                throw;
            }
        }
        public Profile RetrieveProfileByUsername(string username)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var response = cnn.QuerySingleOrDefault<Profile>("SELECT * FROM profiles WHERE username LIKE @username", new { username = $"%{username}%" });
                    return response;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding profile: {ex.Message}");
                return null;
            }

        }
        public void UpdateProfile(Profile profile)
        {
            try
            {
                var existingProfile = RetrieveProfileByUsername(profile.Username);
                if (existingProfile == null)
                {
                    throw new InvalidOperationException("Profile not found");
                } 
                else 
                {
                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        string sql = @"
                        UPDATE profiles 
                        SET name = @Name, 
                            email = @Email, 
                            address = @Address, 
                            phone = @Phone, 
                            picture = @Picture, 
                            password = @Password 
                        WHERE username = @Username";

                        int rowsAffected = cnn.Execute(sql, profile);

                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException("No changes were made to the profile");
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating profile: {ex.Message}");
                throw;
            }
        }

        public void DeleteProfile(string username)
        {
            try
            {
                if(RetrieveProfileByUsername(username) != null)
                {
                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        cnn.Execute("DELETE FROM profiles WHERE username LIKE @username", new { username = $"%{username}%" });
                    }
                }
                else 
                {
                    throw new InvalidOperationException("Username does not exist in database");
                }
                
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding profile: {ex.Message}");
                throw;
            }

        }

        public void AddAccount(Account account)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT INTO accounts (holder_username, balance) VALUES (@holder_username, @balance)", account);
                    Console.WriteLine($"Account added for {account.holder_username}");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding account: {ex.Message}");
                throw;
            }
        }

        public Account GetAccountById(int accountId)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    // Query the account by account_number
                    return cnn.QuerySingleOrDefault<Account>(
                        "SELECT * FROM accounts WHERE account_number = @account_number",
                        new { account_number = accountId });
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the account: {ex.Message}");
                throw;
            }
        }

        public List<Account> GetAccountsByUsername(string username)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    // Query to get all accounts associated with the given username
                    return cnn.Query<Account>(
                        "SELECT * FROM accounts WHERE holder_username = @holder_username",
                        new { holder_username = username }).ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving accounts: {ex.Message}");
                throw;
            }
        }


        public void UpdateAccount(Account account)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    // Update the account by account_number
                    cnn.Execute("UPDATE accounts SET holder_username = @holder_username, balance = @balance WHERE account_number = @account_number", account);
                    Console.WriteLine($"Account {account.holder_username} updated successfully.");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the account: {ex.Message}");
                throw;
            }
        }

        public void DeleteAccount(int accountId)
        {
            try
            {
                if (GetAccountById(accountId) != null)
                {
                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        // Delete the account by account_number
                        cnn.Execute("DELETE FROM accounts WHERE account_number = @account_number", new { account_number = accountId });
                        Console.WriteLine($"Account {accountId} deleted successfully.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Account does not exist in the database.");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the account: {ex.Message}");
                throw;
            }
        }

        public void Deposit(int accountNumber, double amount)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    // Update the account balance
                    cnn.Execute("UPDATE accounts SET balance = balance + @amount WHERE account_number = @account_number",
                                new { amount, account_number = accountNumber });

                    // Format the DateTime to exclude fractional seconds
                    string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Insert the transaction record with formatted date and time
                    cnn.Execute("INSERT INTO transactions (account_number, amount, transaction_date) VALUES (@account_number, @amount, @transaction_date)",
                                new { account_number = accountNumber, amount, transaction_date = currentDateTime });

                    Console.WriteLine($"Deposit of {amount} to account {accountNumber} was successful.");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the deposit: {ex.Message}");
                throw;
            }
        }

        public void Withdraw(int accountNumber, double amount)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    // Check the current balance
                    var balance = cnn.QuerySingle<double>("SELECT balance FROM accounts WHERE account_number = @account_number",
                                                          new { account_number = accountNumber });
                    // If valid compared to amount proceed
                    if (balance >= amount)
                    {
                        // Update the balance
                        cnn.Execute("UPDATE accounts SET balance = balance - @amount WHERE account_number = @account_number",
                                    new { amount, account_number = accountNumber });

                        // Format the DateTime to exclude fractional seconds
                        string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        // Insert the transaction record with formatted date and time
                        cnn.Execute("INSERT INTO transactions (account_number, amount, transaction_date) VALUES (@account_number, @amount, @transaction_date)",
                                    new { account_number = accountNumber, amount = -amount, transaction_date = currentDateTime });

                        Console.WriteLine($"Withdrawal of {amount} from account {accountNumber} was successful.");
                    }
                    else
                    {
                        throw new InvalidOperationException("Insufficient funds for this withdrawal.");
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the withdrawal: {ex.Message}");
                throw;
            }
        }

        //Retrieves list of transactions from table for account id
        public List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    return cnn.Query<Transaction>( "SELECT * FROM transactions WHERE account_number = @account_number ORDER BY transaction_date DESC",
                        new { account_number = accountId }).ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the transaction history: {ex.Message}");
                throw;
            }
        }

        public void ClearDatabase()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    // Delete profiles
                    cnn.Execute("DELETE FROM profiles");
                    // Delete accounts
                    cnn.Execute("DELETE FROM accounts");
                    // Delete transactions
                    cnn.Execute("DELETE FROM transactions");

                    Console.WriteLine($"Database cleared successfully.");
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
                Console.WriteLine($"SQLite error code: {ex.ResultCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the account: {ex.Message}");
                throw;
            }

        }

        private string LoadConnectionString(string id = "DefaultConnection")
        {
            var connectionString = _configuration.GetConnectionString(id);
            if (String.IsNullOrEmpty(_dbPath))
            {
                return connectionString;
            }
            else
            {
                return connectionString.Replace("Data Source=.\\BankDB.db", $"Data Source={_dbPath}");
            }
        }
    }
}
