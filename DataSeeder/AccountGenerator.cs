using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSeeder
{
    public class AccountGenerator
    {
        public static List<Account> Generate(int numAccs, List<Profile> profiles)
        {
            var random = new Random();
            var accounts = new List<Account>();

            for(int i = 0; i < numAccs; i++) 
            {
                double balance = random.Next(0, 50000);
                string username = profiles[random.Next(profiles.Count)].Username;

                var account = new Account
                {
                    balance = balance,
                    holder_username = username
                };

                accounts.Add(account);

            }

            return accounts;
        } 
    }
}
