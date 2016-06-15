using BankApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public enum TransactionType
    {
        Deposit,
        Withdraw
    }

    public enum AccountType
    {
        Savings,
        Credit
    }

    public abstract class Account
    {
        //check need for public variables later
        public decimal InterestRate { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccType { get; set; }
        public int AccountNumber { get; }
        public DateTime FreeWithdrawDate { get; set; }

        private decimal maxDepositAmount = 5000000000m;

        public Account(int accountNumber)
        {
            AccountNumber = accountNumber;
            SetStartBalance();
        }

        private void SetStartBalance()
        {
            Balance = 0;

            using (var db = new BankContextNew())
            {
                var queryAccount = from a in db.Accounts
                                   where a.AccountNumber == AccountNumber
                                   select a;

                if (queryAccount.Count() > 0)
                {
                    if(this is SavingsAccount && queryAccount.First().FreeWithdrawDate != null)
                        FreeWithdrawDate = (DateTime) queryAccount.First().FreeWithdrawDate;

                    Balance = queryAccount.First().Balance;
                }
            }
        }

        public abstract bool Withdraw(decimal amount);

        public virtual bool Deposit(decimal amountToDeposit)
        {
            if (amountToDeposit <= maxDepositAmount)
            {
                Balance += amountToDeposit;
                return true;
            }
            else
                return false;
        }

        public string GetAccountInfo()
        {
            return "Account Number: " + AccountNumber + "\nBalance: " + Balance + "\nAccount Type: " +
                    AccType.ToString() + " account" + "\nInterest Rate: " + InterestRate;
        }

        public string CloseAccountInfo()
        {
            return "Balance: " + Balance + "\nBalance with Interest: " + Balance * InterestRate;
        }
    }
}
