using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class CreditAccount : Account
    {
        private decimal debtInterest;
        private decimal savingsInterest;
        private decimal creditLimit;

        public CreditAccount(int accNumber) : base(accNumber)
        {
            AccType = AccountType.Credit;
            savingsInterest = 1.005m;
            debtInterest = 1.07m;
            creditLimit = 5000m;
            if(Balance < 0)
                InterestRate = debtInterest;
            else 
                InterestRate = savingsInterest;
        }

        public override bool Deposit(decimal amountToDeposit)
        {
            base.Deposit(amountToDeposit);

            if (Balance >= 0)
                InterestRate = savingsInterest;
            return true;
        }

        public override bool Withdraw(decimal amountToWithdraw)
        {
            if (amountToWithdraw <= (Balance + creditLimit))
            {
                Balance -= amountToWithdraw;
                if (Balance < 0)
                    InterestRate = debtInterest;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
