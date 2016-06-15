using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class Customer
    {
        public string SocialSecurityNr { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public List<Account> Accounts { get; set; }

        private int maxCreditAccounts = 2;

        public Customer(string socialSecurity, string fName, string lName)
        {
            SocialSecurityNr = socialSecurity;
            FName = fName;
            LName = lName;

            Accounts = Repository.GetInstance().GetCustomerAccounts(SocialSecurityNr);

            if (Accounts == null)
                Accounts = new List<Account>();
        }

        public override string ToString()
        {
            return $"{FName} {LName}, {SocialSecurityNr}";
        }

        public IList<string> CustomerAndAccountInfo()
        {
            List<string> info = new List<string>();

            info.Add(ToString());

            foreach (Account acc in Accounts)
            {
                info.Add(acc.GetAccountInfo());
            }

            return info;
        }

        public bool UpdateName(string firstName, string lastName)
        {
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                FName = firstName;
                LName = lastName;

                Repository.GetInstance().UpdateCustomerName(new Customer(SocialSecurityNr, FName, LName));

                return true;
            }
            else
                return false;
        }

        public string CloseAccount(Account activeAccount)
        {
            string info = activeAccount.CloseAccountInfo();

            Repository.GetInstance().RemoveAccount(activeAccount);
            Accounts.Remove(activeAccount);
            return info;
        }

        public int AddSavingsAccount(int newAccNumber)
        {
            SavingsAccount acc = new SavingsAccount(newAccNumber);

            if (acc != null)
            {
                Accounts.Add(acc);
                Repository.GetInstance().AddNewAccount(acc, SocialSecurityNr);

                return acc.AccountNumber;
            }
            else
                return -1;
        }

        public int AddCreditAccount()
        {
            int creditAccounts = 0;

            foreach (var a in Accounts)
            {
                if (a.AccType == AccountType.Credit)
                    creditAccounts++;
            }

            if (creditAccounts == maxCreditAccounts)
                return -1;

            int newAccNumber = Repository.GetInstance().GetAccountNumber();
            CreditAccount acc = new CreditAccount(newAccNumber);

            if (acc != null)
            {
                Accounts.Add(acc);
                Repository.GetInstance().AddNewAccount(acc, SocialSecurityNr);

                return acc.AccountNumber;
            }
            else
                return -1;
        }

        public IList<string> GetAccounts()
        {
            List<string> accList = new List<string>();

            foreach (Account acc in Accounts)
            {
                accList.Add(acc.AccType.ToString() + " account");
            }

            return accList;
        }
    }
}
