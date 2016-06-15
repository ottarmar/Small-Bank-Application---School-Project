using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Data
{
    public class CustomerDB
    {
        [Key]
        public string SocialSecurityNr { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CustomerDB() { }

        public CustomerDB(string socNr, string firstName, string lastName)
        {
            SocialSecurityNr = socNr;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
