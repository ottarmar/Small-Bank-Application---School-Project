using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class BankLogic
    {
        public static BankLogic instance;

        public Account ActiveAccount { get; set; }
        public Customer ActiveCustomer { get; set; }

        private Repository repository;
        //private List<Customer> customers;
        
        private BankLogic()
        {
            repository = Repository.GetInstance();
            //customers = repository.Customers;
        }

        public static BankLogic GetInstance()
        {
            if (instance == null)
            {
                instance = new BankLogic();
            }
            return instance;
        }

        public bool Withdraw(decimal amountToWithdraw)
        {
            DateTime tempDate = ActiveAccount.FreeWithdrawDate;

            if (ActiveAccount.Withdraw(amountToWithdraw))
            {
                repository.AddTransaction(new Transaction(ActiveAccount.AccountNumber, DateTime.Now, 
                    TransactionType.Withdraw, ActiveAccount.Balance, amountToWithdraw));

                if (ActiveAccount is SavingsAccount)
                    if (tempDate != ActiveAccount.FreeWithdrawDate)
                        repository.UpdateWithdrawDate((SavingsAccount) ActiveAccount);

                return true;
            }
            return false;
        }

        //public void Deposit(string accountNumber, int amount)
        public bool Deposit(decimal amountToDeposit)
        {
            if (ActiveAccount.Deposit(amountToDeposit))
            {
                repository.AddTransaction(new Transaction(ActiveAccount.AccountNumber,
                    DateTime.Now, TransactionType.Deposit, ActiveAccount.Balance, amountToDeposit));
                   return true;
            }
            else
                return false;
        }

        public IList<string> DisplayAccountInfo()
        {
            List<string> accInfo = new List<string>();
            accInfo.Add(ActiveAccount.GetAccountInfo());

            return accInfo; //PRINTAR UT KONTOINFORMATION
        }

        public int AddSavingsAccount()
        {
            return ActiveCustomer.AddSavingsAccount(repository.GetAccountNumber());
        }

        public int AddCreditAccount()
        {
            return ActiveCustomer.AddCreditAccount();
        }

        public string CloseAccount()
        {
            return ActiveCustomer.CloseAccount(ActiveAccount);
        }

        public void SetActiveAccount(int index)
        {
            ActiveAccount = ActiveCustomer.Accounts[index];
        }

        //CUSTOMER THINGS

        public IList<string> GetCustomers()
        {
            return repository.GetCustomerList();
        }

        public bool AddCustomer(string fname, string lname, string socialSecurityNr)
        {
            return repository.AddNewCustomer(fname, lname, socialSecurityNr);
        }

        public IList<string> GetCustomerInfo()
        {
            return ActiveCustomer.CustomerAndAccountInfo();
        }

        public bool ChangeCustomerName(string firstName, string lastName)
        {
            return ActiveCustomer.UpdateName(firstName, lastName);
        }

        public IList<string> RemoveCustomer()
        {
            //repository.RemoveAllAccounts(ActiveCustomer.SocialSecurityNr);

            IList<string> customerInfo = repository.RemoveCustomer(ActiveCustomer.SocialSecurityNr);
            ActiveCustomer = null;
            return customerInfo;
        }

        public void SetActiveCustomer(int index)
        {
            ActiveCustomer = repository.GetCustomer(index);
        } 

        public IList<string> GetTransactions()
        {
            return repository.GetTransactions(ActiveCustomer.SocialSecurityNr, ActiveAccount.AccountNumber);
        }
    }
}
