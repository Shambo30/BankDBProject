using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataSeeder
{
    public class ProfileGenerator
    {
        public static List<Profile> Generate(int numProfiles)
        {
            var random = new Random();
            var profiles = new List<Profile>();

            string[] firstNames = { "Joey", "Benjamin", "Louis", "Tianna", "Lily", "Natalie" };
            string[] lastNames = { "Reynolds", "Greene", "Snow", "Rowe", "Hogan", "Stewart" };
            string[] emailDomains = { "gmail.com", "yahoo.com", "outlook.com", "bigpond.com", "aol.com"};

            var admin = new Profile
            {
                Name = "Administrator",
                Email = "admin@admin.com",
                Address = "999 Admin Street",
                Phone = "0400000000",
                Picture = "404",
                Password = "admin",
                Username = "admin"
            };
            profiles.Add(admin);

            for (int i = 0; i < numProfiles; i++) 
            {
                string fName = firstNames[random.Next(firstNames.Length)];
                string lName = lastNames[random.Next(lastNames.Length)];
                string email = $"{fName.ToLower()}.{lName.ToLower()}{random.Next(100, 999)}@{emailDomains[random.Next(emailDomains.Length)]}";

                var profile = new Profile
                {
                    Name = $"{fName} {lName}",
                    Email = email,
                    Address = $"{random.Next(1, 1000)} Main St, Perth, WA 6000",
                    Phone = $"04{random.Next(10,99)}{random.Next(100,999)}{random.Next(100, 999)}",
                    Picture = "404",
                    Password = $"Pass{random.Next(1000, 9999)}",
                    Username = $"{fName.ToLower()}{lName.ToLower()}{random.Next(10, 99)}"

                };

                profiles.Add(profile);
            
            }

            return profiles;

        }
    }
}
